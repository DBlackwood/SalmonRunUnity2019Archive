using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishSchool : MonoBehaviour, IPausable {

    [Header("References")]
    // fish prefab
    public GameObject fishPrefab;

    // controller through which vector field can be accessed
    public WaterGridController controller;

    [Header("School Info")]
    // how big this school is
    public int schoolSize;

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

        // register with time manager
        TimeManager.Instance.RegisterPausable(this);
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
    public void FishRemoved(Fish f)
    {
        fishList.Remove(f);
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
        StartCoroutine(SpawnOverTime());
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
    private IEnumerator SpawnOverTime()
    {
        // keep going until all fish are spawned
        while (fishList.Count < schoolSize)
        {
            // only spawn when not paused
            if (!paused)
            {
                // spawn a wave's worth of fish in a loop
                for (int i = 0; i < fishPerWave; i++)
                {
                    // get a random position within the spawn area to instantiate the fish at
                    Vector3 spawnPos = new Vector3(Random.Range(topLeft.x, topRight.x), Random.Range(bottomLeft.y, topLeft.y));

                    // create the fish at the given position and tell it what school it belongs to
                    fishList.Add(Instantiate(fishPrefab, spawnPos, Quaternion.identity).GetComponentInChildren<Fish>());
                    fishList[fishList.Count - 1].SetSchool(this);
                }
            }

            // wait between each wave
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    #endregion
}
