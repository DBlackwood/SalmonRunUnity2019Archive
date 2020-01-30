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
public class WaterGridController : MonoBehaviour {

    // instance of water grid controller
    public static WaterGridController Instance { get; private set; }

    // the scriptable object that contains the actual vector field
    public WaterGridVectorField vectorField;

    // should we clear the field data when we start the game (for developer use)
    public bool clearFieldOnStart;

    // if true, limit the vectors to within the grid square the originate from
    public bool limitVectorsToGridSquare;

    // grid object
    public Grid grid;

    // tilemap object and getters for properties of the tilemap needed elsewhere
    public Tilemap tilemap;
    public int VFWidth
    {
        get
        {
            return tilemap.size.x;
        }
    }
    public int VFHeight
    {
        get
        {
            return tilemap.size.y;
        }
    }

    /**
     * Handle initialization of controller
     */
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one water grid controller in scene!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        StartVectorField();
	}

    /**
     * Handle initial setup for the vector field
     */
    public void StartVectorField()
    {
        vectorField.LoadFromFile(tilemap);

        // if flag set or the vector field doesn't match the tilemap, clear the vector field
        if (clearFieldOnStart || !vectorField.VectorFieldMatches(tilemap))
        {
            vectorField.ResetVectorField(tilemap);
        }
    }

    /**
     * Get the vector from the field that corresponds to a world position
     */
    public Vector2 GetVectorAtWorldPosition(Vector3 position)
    {
        // get grid position from world position
        Vector3Int gridPos = grid.WorldToCell(position);

        // subtract origin from grid pos to get x coordinate
        int x = gridPos.x - tilemap.origin.x;
        // similar process for y coordinate, but we have to flip the result
        // because grid coordinates start from bottom left, but we want to start from top left
        int y = tilemap.size.y - (gridPos.y - tilemap.origin.y);

        // use the coordinates to get the vector
        return GetVector(x, y);
    }

    /**
     * Get the vector at a the given (x,y) position in the vector field
     */
    public Vector2 GetVector(int vectorFieldX, int vectorFieldY)
    {
        // convert 2D coords to 1D, then index into the vector field
        return vectorField.Vectors[GetOneDimensionalIndex(vectorFieldX,vectorFieldY)];
    }

    /**
     * Get the cell position that corresponds to a (x,y) position in the vector field
     */
    public Vector3Int GetCellPosition(int vectorFieldX, int vectorFieldY)
    {
        // add tilemap origin to x coord to get cell x
        int cellX = vectorFieldX + tilemap.origin.x;

        // similar math for y coord, but must undo our flipping of the coordinate system
        int cellY = tilemap.size.y - (vectorFieldY - tilemap.origin.y);

        // z coord is always 0
        return new Vector3Int(cellX, cellY, 0);
    }

    /**
     * Get the world position for the center of the cell corresponding to the given (x,y) coordinate in the vector field
     */
    public Vector3 GetCellCenterWorld(int vectorFieldX, int vectorFieldY)
    {
        return tilemap.GetCellCenterWorld(GetCellPosition(vectorFieldX, vectorFieldY));
    }

    /**
     * Determine if there is a water tile in the tilemap at the given (x,y) coordinate in the vector field
     */
    public bool WaterTileAt(int vectorFieldX, int vectorFieldY)
    {
        return (tilemap.GetTile(GetCellPosition(vectorFieldX, vectorFieldY)) != null);
    }

    /**
     * Given an (x,y) coordinate, convert it to match the 1-D array structure that is actually used to store the vector field
     */
    public int GetOneDimensionalIndex(int vectorFieldX, int vectorFieldY)
    {
        return vectorFieldY * tilemap.size.x + vectorFieldX;
    }

    /**
     * Given an index into the 1-D array that is used to store the vector field, convert to an (x,y) coordinate
     */
    public Vector2Int GetTwoDimensionalIndex(int vectorFieldIndex)
    {
        int vectorFieldY = vectorFieldIndex / tilemap.size.x;
        int vectorFieldX = vectorFieldIndex - (vectorFieldY * tilemap.size.x);

        return new Vector2Int(vectorFieldX, vectorFieldY);
    }

    public void CompressTilemapBounds()
    {
        tilemap.CompressBounds();
    }
}
