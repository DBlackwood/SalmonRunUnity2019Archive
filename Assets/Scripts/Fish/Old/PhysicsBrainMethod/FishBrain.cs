using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBrain : MonoBehaviour {

    public Grid g;

    public int[] hiddenLayers;

    public Vector2[] sensorLocations; 

    private float[] memory;
    
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        // Debug.Log(WaterGridController.Grid.WorldToCell(transform.position));
    }

    public void Init()
    {
        memory = new float[] { 0 };
        
    }

    private static float Activation(float input)
    {
        return 1f / (1 + Mathf.Exp(-input));
    }
}
