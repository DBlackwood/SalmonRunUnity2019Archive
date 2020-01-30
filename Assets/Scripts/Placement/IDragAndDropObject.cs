using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Interface for all towers in the game. Provides shared variables and functions.
 */
public interface IDragAndDropObject
{
    /**
     * Determine whether a tower of this type can be placed at the given location
     * 
     * @param primaryHitInfo RaycastHit The information from a raycast, used to determine whether the placement is valid
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     */
    bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo);

    /**
     * Place the into the level based on the location provided
     * 
     * @param primaryHitInfo RaycastHit The information from a raycast, used to determine whether the placement is valid
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     */
    void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo);
}
