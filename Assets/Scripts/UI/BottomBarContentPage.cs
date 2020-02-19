using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBarContentPage : MonoBehaviour
{
    // title of the page
    public string pageTitle;

    // true if this content page is currently active
    private bool active;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;

            // set all children to the setter value
            SetActiveUtils.SetChildrenActiveRecursively(gameObject, active);
        }
    }
}
