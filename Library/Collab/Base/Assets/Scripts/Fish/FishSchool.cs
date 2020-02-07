using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class FishSchool : MonoBehaviour, IPausable {

    [Header("References")]
    // fish prefab
    public GameObject fishPrefab;

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
        GameManager.onStartRun.AddListener(Spawn);
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
        if (bottomLeft != null && bottomRight != null && topLeft != null && topRight != null)
        {
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(bottomRight, topRight);
        }
        else
        {
            CalculateSpawnAreaBoundaries();
        }
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
        CheckForEndOfRun();
    }

    /**
     * Called when a fish has succeeded in reaching the end of the level
     */
    public void FishSucceeded(Fish f)
    {
        successfulFishList.Add(f);
        fishList.Remove(f);
        CheckForEndOfRun();
    }

    /**
     * Check if there are any more fish still trying to reach the goal -- if not, end the run
     */
    private void CheckForEndOfRun()
    {
        if (fishList.Count <= 0)
        {
            GameManager.Instance.ToPlace();
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
     * Spawn fish
     */
    public void Spawn()
    {
        // need to get some genomes
        List<FishGenome> genomes;

        // if it's the first turn, generate a full set of genomes from nothing
        if (GameManager.Instance.Turn == 1)
        {
            genomes = FishReproduction.MakeNewGeneration(initialNumFish, true, true);
        }
        // otherwise, need to make new genomes from the succesful fish's genomes
        // also need to clean out the old fish
        else
        {
            // make new genomes
            List<FishGenome> oldGenomes = successfulFishList.Select(fish => fish.GetGenome()).ToList();
            genomes = FishReproduction.MakeNewGeneration(oldGenomes, minOffspring, maxOffspring);

            // clean out old fish
            DeleteOldFish();
        }

        StartCoroutine(SpawnOverTime(genomes));
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
        // keep going until all fish are spawned
        for(int waveIndex = 0; (waveIndex + 1) * fishPerWave <= genomes.Count; waveIndex++)
        {
            // only spawn when not paused
            if (!paused)
            {
                // spawn a wave's worth of fish in a loop
                for (int fishIndex = 0; fishIndex < fishPerWave && waveIndex * fishPerWave + fishIndex <= genomes.Count; fishIndex++)
                {
                    // get a random position within the spawn area to instantiate the fish at
                    Vector3 spawnPos = new Vector3(Random.Range(topLeft.x, topRight.x), Random.Range(bottomLeft.y, topLeft.y));

                    // create the fish at the given position and tell it what school it belongs to
                    fishList.Add(Instantiate(fishPrefab, spawnPos, Quaternion.identity).GetComponentInChildren<Fish>());
                    fishList[fishList.Count - 1].SetSchool(this);
                    fishList[fishList.Count - 1].SetGenome(genomes[fishList.Count - 1]);
                }
            }

            // wait between each wave
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    #endregion
}
