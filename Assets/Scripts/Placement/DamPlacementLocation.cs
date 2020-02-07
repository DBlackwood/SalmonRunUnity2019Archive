using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
public class DamPlacementLocation : MonoBehaviour
{
    // list of all dam placement locations on the map
    private static List<DamPlacementLocation> allLocations = new List<DamPlacementLocation>();

    // drop off box for fish used by the dam once it is actually placed
    // since the dam is being dropped in, we need to hold onto this until the dam is actually placed
    public BoxCollider dropOffBox;

    // is this location currently being used?
    public bool inUse { get; private set; }

    //get and set can be defined here or we can defaults
    // do not have write getter and setter, 
    // bool so get public, and set private in this case. 
    // get; set; would have both as public


    // placement location's mesh renderer
    private MeshRenderer meshRenderer;

    #region Major Monobehaviour functions

    /**
     * Initialization function
     */
    private void Awake()
    {
        // add each placement location to our list as it initializes
        allLocations.Add(this);

        // location will not be in use at the beginning
        inUse = false;

        // get component refs
        meshRenderer = GetComponent<MeshRenderer>();

        // turn the meshRenderer off by default
        meshRenderer.enabled = false;
    }

    #endregion

    #region Dams and Attachment

    /**
     * Attach a dam to this location
     */
    public void AttachDam(Dam dam)
    {
        if (!inUse)
        {

            // position the dam within hierarchy and game space to match the dam location
            dam.transform.SetParent(transform.parent);
            dam.transform.position = transform.position;

            dam.Activate(dropOffBox);

            // the location is now in use and cannot be used by any other dam
            inUse = true;
        }
        else
        {
            Debug.Log("Trying to attach dam to in-use location -- this line should not be reached!");
        }
    }

    #endregion

    #region Visualization (Static)

    /**
     * Activate visualization for all dam placement locations
     * 
     * @param True if we want to activate visualizations, false otherwise
     */
    public static void SetVisualizations(bool activate)
    {
        foreach (DamPlacementLocation placementLocation in allLocations)
        {
            placementLocation.meshRenderer.enabled = activate;
        }
    }

    #endregion
}
