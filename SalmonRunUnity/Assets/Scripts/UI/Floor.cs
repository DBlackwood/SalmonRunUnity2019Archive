using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * Script attached to whatever object is considered the "floor" for the game.
 * 
 * Currently, this is used for the drag & drop UI to get the location on the ground that the current mouse position corresponds to.
 * However, this class may also be used for other purposes at some point.
 * 
 * Author: Jacob Cousineau
 */
public class Floor : MonoBehaviour {

    // Singleton instance to be used for access from other scripts
    public static Floor Instance { get; private set; }

    // ground collider
    private new BoxCollider collider;

    /**
     * Handle object initialization
     */
    private void Awake()
    {
        // create the singleton reference
        if (Instance == null)
        {
            Instance = this;
        }
        // if another reference already exists, log an error and delete this version -- we only want one ground in the scene at any time
        else
        {
            Debug.LogError("More than one ground script in the scene!");
            Destroy(this);
        }
    }

    /**
     * Called before initial update
     */
    private void Start()
    {
        // generate a collider for the ground
        collider = gameObject.AddComponent<BoxCollider>();

        // make the collider the same size as the tilemap
        // we need to do this because the tilemap collider is 2d-only and we are using 3d everywhere else
        Tilemap tilemap = WaterGridController.Instance.tilemap;
        collider.center = Vector3.Scale(new Vector3(tilemap.origin.x + tilemap.size.x / 2f, tilemap.origin.y + tilemap.size.y / 2f), tilemap.transform.lossyScale);
        collider.size = Vector3.Scale(tilemap.size, tilemap.transform.lossyScale);
    }
}
