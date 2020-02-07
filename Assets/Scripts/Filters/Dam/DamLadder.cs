using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamLadder : MonoBehaviour, IDragAndDropObject
{

    #region IDragAndDropObject Implementation

    /**
     * Place the dam onto the game map
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
        DamPlacementLocation placementLocation = primaryHitInfo.collider.transform.root.GetComponentInChildren<DamPlacementLocation>();
        if (placementLocation != null)
        {
            placementLocation.AttachLadder(this);
        }
    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // must have hit something
        if (primaryHitInfo.collider)
        {
            // can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
            DamPlacementLocation placementLocation = primaryHitInfo.collider.transform.root.GetComponentInChildren<DamPlacementLocation>();

            // thing we hit must be a dam placement location
            if (placementLocation != null)
            {
                // only return true if the placement location is not already in use
                return (placementLocation.inUse && !placementLocation.HasLadder);
            }
        }

        return false;
    }

    #endregion
}
