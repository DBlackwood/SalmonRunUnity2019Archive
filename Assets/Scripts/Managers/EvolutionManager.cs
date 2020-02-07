using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles fish evolution over generations
 */
public class EvolutionManager : MonoBehaviour
{
    // singleton instance
    public static EvolutionManager Instance { get; private set; }

    /**
     * Initialization function
     */
    private void Awake()
    {
        // handle singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one EvolutionManager in the scene! Deleting...");
            Destroy(this);
        }
    }

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        
    }
}
