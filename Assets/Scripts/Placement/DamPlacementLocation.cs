using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamPlacementLocation : MonoBehaviour
{
    // list of all dam placement locations on the map
    private static List<DamPlacementLocation> allLocations = new List<DamPlacementLocation>();

    // mesh renderer for the dam placement visual
    public MeshRenderer mainMeshRenderer;

    // meshRenderer for the ladder placement visual
    public MeshRenderer ladderMeshRenderer;

    // drop off box for fish used by the dam once it is actually placed
    // since the dam is being dropped in, we need to hold onto this until the dam is actually placed
    public BoxCollider dropOffBox;

    // is this location currently being used?
    public bool inUse { get; private set; } = false;

    // does this location currently have a ladder attached?
    public bool HasLadder { get; private set; } = false;

    //get and set can be defined here or we can defaults
    // do not have write getter and setter, 
    // bool so get public, and set private in this case. 
    // get; set; would have both as public

    // Dam component currently being used at this location (if any)
    private Dam currentDam;

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

        // ensure that we have visualizers for the dam and the ladder
        if (mainMeshRenderer == null || ladderMeshRenderer == null)
        {
            Debug.LogError("A MeshRenderer on a DamPlacementLocation is missing!");
        }

        // turn the meshRenderers off by default
        mainMeshRenderer.enabled = false;
        ladderMeshRenderer.enabled = false;
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
            dam.transform.rotation = transform.rotation;
            dam.transform.localScale = transform.localScale;

            dam.Activate(dropOffBox);

            // the location is now in use and cannot be used by any other dam
            inUse = true;

            // keep track of the dam for use later
            currentDam = dam;
        }
        else
        {
            Debug.Log("Trying to attach dam to in-use location -- this line should not be reached!");
        }
    }

    /**
     * Attach a ladder to the dam at this location (if there is one)
     */
    public void AttachLadder(DamLadder damLadder)
    {
        // can only attach if there is a dam here with no ladder
        if (inUse && currentDam != null && !HasLadder)
        {
            // put the ladder in the correct location and parent it to the current dam
            damLadder.transform.SetParent(currentDam.transform);
            damLadder.transform.localPosition = ladderMeshRenderer.transform.localPosition;
            damLadder.transform.localRotation = ladderMeshRenderer.transform.localRotation;
            damLadder.transform.localScale = ladderMeshRenderer.transform.localScale;

            // tell the dam that a ladder is being attached
            currentDam.AddLadder(damLadder);

            // this dam location now has a ladder attached to it
            HasLadder = true;
        }
    }

    #endregion

    #region Visualization (Static)

    /**
     * Activate visualization for all dam placement locations
     * 
     * @param True if we want to activate visualizations, false otherwise
     */
    public static void SetDamVisualizations(bool activate)
    {
        foreach (DamPlacementLocation placementLocation in allLocations)
        {
            // if the placement location is in use and we're trying to activate it, don't do so
            // because you shouldn't be able to place anything there
            if (!activate || !placementLocation.inUse)
            {
                placementLocation.mainMeshRenderer.enabled = activate;
            }
            
        }
    }

    /**
     * Activate visualization for all ladder placement locations
     * 
     * @param True if we want to activate visualizations, false otherwise
     */
    public static void SetLadderVisualizations(bool activate)
    {
        foreach (DamPlacementLocation placementLocation in allLocations)
        {
            // only show visualizations where the placement location is in use but there is no ladder
            if (!activate || (placementLocation.inUse && !placementLocation.HasLadder))
            {
                placementLocation.ladderMeshRenderer.enabled = activate;
            }

        }
    }

    #endregion
}
