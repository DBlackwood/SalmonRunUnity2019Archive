using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class FishSchool : MonoBehaviour, IPausable {

    [Header("References")]
    // fish prefab config gameobject
    public FishPrefabConfig fishPrefabConfig;

    // controller through which vector field can be accessed
    public WaterGridController controller;

    [Header("School Info")]
    // how big this school is
    public int initialNumFish;

    // minimum and maximum number of children that a pair of salmon can generate during reproduction
    public int minOffspring;
    public int maxOffspring;

    [Header("Spawning")]
    // describe area in which fish can spawn
    public float spawnAreaWidth;
    public float spawnAreaHeight;

    // z plane on which fish should spawn
    public float spawnZ;

    // how long it takes between groups of fish being spawned
    public float timeBetweenWaves;

    // how many fish should be spawned in each group
    public int fishPerWave;

    [Header("Movement Settings")]
    // value describining how large the random movement will be in comparison to the movement from the vector field
    public float randomMovementMultiplier;

    // true if the fish are paused
    private bool paused = false;

    // all fish in the school
    private List<Fish> fishList;

    // fish that have made it to the end of the level
    private List<Fish> successfulFishList;

    // fish that have died
    private List<Fish> deadFishList;

    // list of fish genomes that will be used in the next generation
    private List<FishGenome> nextGenerationGenomes;

    // corners of the spawn area, for drawing and calculating locations
    private Vector3 bottomLeft;
    private Vector3 bottomRight;
    private Vector3 topLeft;
    private Vector3 topRight;

    #region Major Monobehaviour Functions

    /**
     * Initialization function
     */
    private void Start () {
        // make sure the spawn area is set up
        CalculateSpawnAreaBoundaries();

        // initialize lists
        fishList = new List<Fish>();
        successfulFishList = new List<Fish>();
        deadFishList = new List<Fish>();

        // register with time manager
        TimeManager.Instance.RegisterPausable(this);

        // register listeners with game manager events
        GameEvents.onEndRun.AddListener(CreateNewGeneration);
        GameEvents.onStartRun.AddListener(Spawn);

        // create initial generation of fish
        CreateNewGeneration();
    }
	
	/**
	 * Called on a fixed time interval to update the school
	 */
	private void FixedUpdate () {
        // only do update stuff if the school is not paused
        if (!paused)
        {
            // cull all fish who have used up all of their energy
            List<Fish> fishToCull = fishList.FindAll((Fish fish) =>
            {
                return fish.OutOfEnergy();
            });
            foreach (Fish fish in fishToCull)
            {
                fish.Catch();
            }

            // loop through all remaining fish for movement
            foreach (Fish fish in fishList)
            {
                // get current vector field value at fish's position
                Vector2 vectorFromField = controller.GetVectorAtWorldPosition(fish.transform.position);

                // make the fish move
                fish.Swim(vectorFromField, randomMovementMultiplier, controller.grid.transform.localScale.x);
            }
        }
	}

    /**
     * Draw gizmo representation of spawning area
     */
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }

    /**
     * Handle changes to values in inspector by recalculating spawn area boundaries
     */
    private void OnValidate()
    {
        CalculateSpawnAreaBoundaries();
    }

    #endregion

    #region Fish Management

    /**
     * Remove a fish from the school (presumably because it has died/been caught/etc.)
     */
    public void FishKilled(Fish f)
    {
        fishList.Remove(f);
        deadFishList.Add(f);

        PopulationChanged();

        CheckForEndOfRun();
    }

    /**
     * Called when a fish has succeeded in reaching the end of the level
     */
    public void FishSucceeded(Fish f)
    {
        successfulFishList.Add(f);
        fishList.Remove(f);

        PopulationChanged();

        CheckForEndOfRun();
    }

    /**
     * Kill all fish that are not already dead or at a spawning ground
     * 
     * Primarily for purposes of preventing soft-lock of gameplay if a fish won't die
     */
    public void KillAllActive()
    {
        while (fishList.Count > 0)
        {
            fishList[0].Catch();
        }
    }

    /**
     * Check if there are any more fish still trying to reach the goal -- if not, end the run
     */
    private void CheckForEndOfRun()
    {
        if (fishList.Count <= 0)
        {
            GameManager.Instance.SetState(new RunStatsState());
        }
    }

    /**
     * Remove all fish from the scene/school
     */
    private void DeleteOldFish()
    {
        // destroy gameobjects
        foreach(Fish deadFish in deadFishList)
        {
            Destroy(deadFish.transform.root.gameObject);
        }
        foreach(Fish successfulFish in successfulFishList)
        {
            Destroy(successfulFish.transform.root.gameObject);
        }

        // clear out lists
        deadFishList.Clear();
        successfulFishList.Clear();
        fishList.Clear();
    }

    /**
     * Fire event to inform listeners that fish population has changed in some way
     */
    private void PopulationChanged()
    {
        List<FishGenome> activeGenomes = fishList.Select(fish => fish.GetGenome()).ToList();
        List<FishGenome> successfulGenomes = successfulFishList.Select(fish => fish.GetGenome()).ToList();
        List<FishGenome> deadGenomes = deadFishList.Select(fish => fish.GetGenome()).ToList();
        GameEvents.onFishPopulationChanged.Invoke(activeGenomes, successfulGenomes, deadGenomes);
    }

    #endregion

    #region IPausable Implementation

    /**
     * Pause the school
     */
    public void Pause()
    {
        // pause all the fish
        foreach (Fish f in fishList)
        {
            f.CacheAndPauseMotion();
        }

        // pause the school
        paused = true;
    }

    /**
     * Resume the school from paused
     */
    public void Resume()
    {
        // resume all the fish
        foreach (Fish f in fishList)
        {
            f.RestoreAndResumeMotion();
        }

        // resume the school
        paused = false;
    }

    #endregion

    #region Spawning

    /**
     * Create a new generation of fish from the old generation
     */
    public void CreateNewGeneration()
    {
        // make variable for holding parent genomes
        List<FishGenome> parentGenomes = null;

        // if it's the 0th turn, generate a full set of genomes from nothing
        if (GameManager.Instance.Turn == 1)
        {
            nextGenerationGenomes = FishGenomeUtilities.MakeNewGeneration(initialNumFish, true, true);
        }
        // otherwise, need to make new genomes from the succesful fish's genomes
        // also need to clean out the old fish
        else
        {
            // make new genomes
            parentGenomes = successfulFishList.Select(fish => fish.GetGenome()).ToList();
            nextGenerationGenomes = FishGenomeUtilities.MakeNewGeneration(parentGenomes, minOffspring, maxOffspring);

            // clean out old fish
            DeleteOldFish();
        }

        // send out notice that new generation has been created
        GameEvents.onNewGeneration.Invoke(parentGenomes, nextGenerationGenomes);
    }

    /**
     * Spawn fish
     */
    public void Spawn()
    {
        StartCoroutine(SpawnOverTime(nextGenerationGenomes));
    }

    /**
     * Do calculation of spawn area boundaries based on current supplied width and height
     */
    private void CalculateSpawnAreaBoundaries()
    {
        // calculate spawn area corner coordinates
        bottomLeft = new Vector3(transform.position.x - spawnAreaWidth / 2f, transform.position.y - spawnAreaHeight / 2f, 0);
        bottomRight = new Vector3(transform.position.x + spawnAreaWidth / 2f, transform.position.y - spawnAreaHeight / 2f, 0);
        topLeft = new Vector3(transform.position.x - spawnAreaWidth / 2f, transform.position.y + spawnAreaHeight / 2f, 0);
        topRight = new Vector3(transform.position.x + spawnAreaWidth / 2f, transform.position.y + spawnAreaHeight / 2f, 0);
    }

    /**
     * Spawns fish into the school over time
     */
    private IEnumerator SpawnOverTime(List<FishGenome> genomes)
    {
        // loop until we've spawned enough fish
        int spawnedCount = 0;
        while (spawnedCount < genomes.Count && GameManager.Instance.GetStateName() == nameof(RunState))
        {
            // only spawn when we're not paused
            if (!paused)
            {
                // spawn a wave of fish
                // stop if we reach the number of fish that should be in a wave or if we hit the total number of fish mid-wave
                int spawnedThisWave = 0;
                while (spawnedThisWave < fishPerWave && spawnedCount < genomes.Count)
                {
                    // get a random position within the spawn area to instantiate the fish at
                    Vector3 spawnPos = new Vector3(Random.Range(topLeft.x, topRight.x), Random.Range(bottomLeft.y, topLeft.y), spawnZ);

                    // create the fish at the given position and tell it what school it belongs to
                    fishList.Add(Instantiate(fishPrefabConfig.GetFishPrefab(genomes[fishList.Count]), spawnPos, Quaternion.identity).GetComponentInChildren<Fish>());
                    fishList[fishList.Count - 1].SetSchool(this);
                    fishList[fishList.Count - 1].SetGenome(genomes[fishList.Count - 1]);

                    // increment counters
                    spawnedThisWave++;
                    spawnedCount++;
                }

                // after each wave is spawned, inform the UI & other listeners that the population has changed
                PopulationChanged();
            }

            // wait between waves
            yield return new WaitForSeconds(timeBetweenWaves);
            
        }
    }

    #endregion
}
