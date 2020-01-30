using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Script that gives access to the water grid and its tilemap.
 * 
 * There should only be one water grid at any time.
 */
[RequireComponent(typeof(Grid))]
public class WaterGrid : MonoBehaviour {

    private static Grid gridInstance;
    private static Tilemap tilemapInstance;

    // grid component accessor
    public static Grid GI
    {
        get
        {
            return gridInstance;
        }
    }

    // tilemap component accessor
    public static Tilemap TI
    {
        get
        {
            return tilemapInstance;
        }
    }

	// Use this for initialization
	void Start () {

        // check if grid and tilemap instances are set
		if (gridInstance == null && tilemapInstance == null)
        {
            // set grid and tilemap instances so they are visible to the outside world
            gridInstance = GetComponent<Grid>();
            tilemapInstance = GetComponentInChildren<Tilemap>();
        }
        else
        {
            // grid and tilemap instances already set -- there is more than one grid, so throw an error.
            Debug.LogError("More than one water grid in the scene!");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
