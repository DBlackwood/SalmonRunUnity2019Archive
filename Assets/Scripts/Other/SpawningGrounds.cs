using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningGrounds : MonoBehaviour
{
    // how many fish this spawning grounds has the capacity for
    public int numNestingSights;

    // how many males and females have been taken in
    private int males;
    private int females;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // subscribe to onEndRun event
        GameEvents.onEndRun.AddListener(Clear);
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        
    }

    /**
     * Handle trigger collisions with other objects
     */
    private void OnTriggerEnter(Collider other)
    {
        // figure out if the thing that hit us is actually a fish
        Fish fish = other.GetComponentInChildren<Fish>();
        if (fish != null)
        {
            // need to check if this is a male or female
            bool isMale = fish.GetGenome().IsMale();

            // check if there is a nesting sight available for this fish
            // if so, tell the fish it has reached the spawning grounds
            if (isMale && males < numNestingSights)
            {
                fish.ReachSpawningGrounds();
                males++;
            }
            else if (!isMale && females < numNestingSights)
            {
                // if so, it has officially reached the spawning grounds
                fish.ReachSpawningGrounds();
                females++;
            }
        }
    }

    /**
     * Clear the spawning ground of all fish that are currently here
     */
    private void Clear()
    {
        males = 0;
        females = 0;
    }
}
