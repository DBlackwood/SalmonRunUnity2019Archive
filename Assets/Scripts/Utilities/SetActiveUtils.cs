using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveUtils
{
    /**
     * Set a gameobject and all objects under it active or inactive
     * 
     * @param go GameObject The GameObject we are targeting
     * @param active bool The value that is passed into GameObject.SetActive on each object
     */
    public static void SetActiveRecursively(GameObject go, bool active)
    {
        go.SetActive(active);
        foreach (Transform child in go.transform)
        {
            SetActiveRecursively(child.gameObject, active);
        }
    }

    /**
     * Set all objects underneath a given GameObject in the hierarchy to be active or inactive (without affecting the original GameObject)
     * 
     * @param go GameObject The GameObject whose children we will be affecting
     * @param active bool The value that is passed into GameObject.SetActive on each child object
     */
    public static void SetChildrenActiveRecursively(GameObject go, bool active)
    {
        foreach (Transform child in go.transform)
        {
            SetActiveRecursively(go, active);
        }
    }
}
