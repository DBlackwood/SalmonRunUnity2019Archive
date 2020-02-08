using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * RTS-style camera controller script.
 * 
 * Based on tutorial by Brackeys: https://www.youtube.com/watch?v=cfjLQrMGEb4
 */
public class CameraController : MonoBehaviour {

    // can the mouse be used to pan as well as the arrow keys?
    public bool panWithMouse;

    // how fast the camera pans
    public float panSpeed;

    // lowest speed the camera can pan at, expressed in terms of a fraction of the default pan speed
    [Range(0f, 1f)]
    public float minPanSpeedRatio;

    // size of borders around edge of screen which will start panning when the mouse enters the area
    public float panBorderThickness;

    // min and max values for x and y
    public float xMin;
    public float yMin;
    public float xMax;
    public float yMax;

    // farthest the camera can zoom out
    public float zoomOutMaxPos;

    // closest the camera can zoom in
    public float zoomInMaxPos;

    // how speed at which you can zoom in or out
    public float zoomSpeed;
    
	
	/**
     * LateUpdate is called once per frame after Update has been called
     */
	private void LateUpdate ()
    {
        UpdateCamera();
    }

    /**
     * Update camera position and zoom
     */
    private void UpdateCamera()
    {
        // get camera's current pos
        Vector3 pos = transform.position;

        // figure out new zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // look for keyboard inputs as well
        if (Input.GetKey(KeyCode.Q))
        {
            scroll = -0.1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            scroll = 0.1f;
        }

        pos.z += scroll * zoomSpeed * Time.deltaTime * 100f / Time.timeScale;

        // clamp zoom within boundaries
        pos.z = Mathf.Clamp(pos.z, zoomOutMaxPos, zoomInMaxPos);

        // modulate the pan speed based on the current zoom level (smaller pan when more zoomed in)
        float zoomMultiplier = 1.1f - (pos.z - zoomOutMaxPos) / (zoomInMaxPos - zoomOutMaxPos);

        // figure out how much distance the pan should cover
        // dividing by timeScale so we always appear to pan at the same speed regardless of gameplay speed
        float panDistance = Mathf.Min(panSpeed * zoomMultiplier * Time.deltaTime / Time.timeScale, panSpeed * Time.deltaTime / Time.timeScale);
        // pan depending on which keys have been pressed (or which borders the mouse is currently in)
        if (Input.GetKey(KeyCode.W) || (panWithMouse && Input.mousePosition.y >= Screen.height - panBorderThickness))
        {
            pos.y += panDistance;
        }
        if (Input.GetKey(KeyCode.A) || (panWithMouse && Input.mousePosition.x <= panBorderThickness))
        {
            pos.x -= panDistance;
        }
        if (Input.GetKey(KeyCode.S) || (panWithMouse && Input.mousePosition.y <= panBorderThickness))
        {
            pos.y -= panDistance;
        }
        if (Input.GetKey(KeyCode.D) || (panWithMouse && Input.mousePosition.x >= Screen.width - panBorderThickness))
        {
            pos.x += panDistance;
        }

        // clamp pan within boundaries
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = Mathf.Clamp(pos.y, yMin, yMax);

        // actually update camera pos
        transform.position = pos;
    }
}
