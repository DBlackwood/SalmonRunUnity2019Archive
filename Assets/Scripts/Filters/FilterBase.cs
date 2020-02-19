using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A filter is an object in the game that applies an effect to a fish as it passes through.
 */
[RequireComponent(typeof(Collider))]
public abstract class FilterBase : MonoBehaviour
{
    // is this filter currently active
    protected bool active = false;

    // the collider on the filter object
    private Collider myCollider;

    /**
     * Initialization function
     */
    private void Awake()
    {
        // get component refs
        myCollider = GetComponent<Collider>();
    }

    /**
     * Handle objects entering the collider
     */
    private void OnTriggerEnter(Collider other)
    {
        // only care about triggers if the filter is active
        if (active)
        {
            // check what hit us
            // only care if it's a fish
            Fish f = other.gameObject.GetComponentInChildren<Fish>();
            if (f != null)
            {
                ApplyFilterEffect(f);
            }
        }
    }

    /**
     * Effects of the filter will be implemented here by child classes
     */
    protected abstract void ApplyFilterEffect(Fish fish);
}
