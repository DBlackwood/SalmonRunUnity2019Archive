using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;

/**
 * Asset containing vector field data for the water grid.
 */
[CreateAssetMenu(menuName = "Water Tile Vector Field", fileName = "VectorField.asset")]
public class WaterGridVectorField : ScriptableObject {

    // file path (relative to the project data directory) where the vector field data will be stored
    private static string RELATIVE_FILE_PATH = "/DataFiles/VectorFields/";

    // accessor property for the vector field data
    public Vector2[] Vectors
    {
        get
        {
            return vectors;
        }
    }

    // the actual vector data for the vector field
    [SerializeField]
    private Vector2[] vectors;

    /**
     * Determine whether this vector field matches a given tilemap
     */
    public bool VectorFieldMatches(Tilemap t)
    {
        return (vectors != null && vectors.Length == t.size.x * t.size.y);
    }

    /**
     * Resets the vector field to match a given tilemap
     */
    public void ResetVectorField(Tilemap t)
    {
        // ensure that the tilemap's bounds have been updated
        t.CompressBounds();

        // reallocate space for the vectors
        vectors = new Vector2[t.size.x * t.size.y];

        // loop through vectors to give initial value
        for(int i = 0; i < t.size.x; i++)
        {
            for(int j = 0; j < t.size.y; j++)
            {
                vectors[t.size.y * i + j] = new Vector2(0, 1);
            }
        }
    }

#if UNITY_EDITOR
    /**
     * Save the vector field object to a file
     */
    public void SaveToFileFromEditor()
    {
        string dataAsJson = JsonUtility.ToJson(this);

        string editorDirectoryPath = GetEditorDataDirectoryPath();
        if (!Directory.Exists(editorDirectoryPath))
        {
            Directory.CreateDirectory(editorDirectoryPath);
        }

        string resourcesDirectoryPath = GetResourcesDataDirectoryPath();
        if (!Directory.Exists(resourcesDirectoryPath))
        {
            Directory.CreateDirectory(resourcesDirectoryPath);
        }

        File.WriteAllText(GetEditorDataFilePath(), dataAsJson);
        File.WriteAllText(GetResourcesDataFilePath(), dataAsJson);
    }
#endif

    /**
     * Load vector field data from a file
     */
    public void LoadFromFile(Tilemap tilemap)
    {
#if UNITY_EDITOR
        // make sure that the file to load from actually exists
        if (File.Exists(GetEditorDataFilePath()))
        {
            // read in the data from the file
            string dataAsJson = File.ReadAllText(GetEditorDataFilePath());

            // make a temporary object to take in the data from the file
            WaterGridVectorField tempVF = ScriptableObject.CreateInstance<WaterGridVectorField>();
            JsonUtility.FromJsonOverwrite(dataAsJson, tempVF);

            // make sure that the tilemap size matches the saved data
            if (tilemap.size.x * tilemap.size.y == tempVF.vectors.Length)
            {
                // if the data matches, replace current vector field data with data from file
                this.vectors = tempVF.vectors;
            }
            else
            {
                // if the data does not match, reset the vector field based on the tilemap
                ResetVectorField(tilemap);
            }
        }
        else
        {
            // if the file does not exist, reset the vector field based on the tilemap
            ResetVectorField(tilemap);
        }
#elif UNITY_STANDALONE
        TextAsset textAsset = (TextAsset)Resources.Load("VectorField/" + this.name, typeof(TextAsset));

        // make a temporary object to take in the data from the file
        WaterGridVectorField tempVF = ScriptableObject.CreateInstance<WaterGridVectorField>();
        JsonUtility.FromJsonOverwrite(textAsset.text, tempVF);

        // make sure that the tilemap size matches the saved data
        if (tilemap.size.x * tilemap.size.y == tempVF.vectors.Length)
        {
            // if the data matches, replace current vector field data with data from file
            this.vectors = tempVF.vectors;
        }
        else
        {
            // if the data does not match, reset the vector field based on the tilemap
            ResetVectorField(tilemap);
        }
#endif
    }

#if UNITY_EDITOR
    /**
     * Get the full path (including filename) where vector field data will be saved to
     */
    private string GetEditorDataFilePath()
    {
        string dataFilePath = GetEditorDataDirectoryPath() + GetDataFileName();
        return dataFilePath;
    }

    /**
     * Get the path to the directory where vector field data will be saved
     */
    private string GetEditorDataDirectoryPath()
    {
        string dataDirectoryPath = Application.dataPath + RELATIVE_FILE_PATH;
        return dataDirectoryPath;
    }
#endif

    /**
     * Get the name of the file in which vector field data will be saved
     */
    private string GetDataFileName()
    {
        string dataFileName = this.name + ".json";
        return dataFileName;
    }

    private string GetResourcesDataFilePath()
    {
        string dataFilePath = GetResourcesDataDirectoryPath() + GetDataFileName();
        return dataFilePath;
    }

    private string GetResourcesDataDirectoryPath()
    {
        string dataDirectoryPath = Application.dataPath + "/Resources/VectorField/";
        return dataDirectoryPath;
    }
}
