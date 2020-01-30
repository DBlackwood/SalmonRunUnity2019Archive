using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * RTS-style camera controller script.
 * 
 * Based on tutorial by Brackeys: https://www.youtube.com/watch?v=cfjLQrMGEb4
 */
public class CameraController : MonoBehaviour {

    public float panSpeed;
    public float panBorderThickness;

    public float xMin;
    public float yMin;
    public float xMax;
    public float yMax;

    public float minZoomPos;
    public float maxZoomPos;

    public float scrollSpeed;
    
	
	// Update is called once per frame
	void Update () {

        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.z += scroll * scrollSpeed * Time.deltaTime * 100f;

        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = Mathf.Clamp(pos.y, yMin, yMax);
        pos.z = Mathf.Clamp(pos.z, minZoomPos, maxZoomPos);

        transform.position = pos;
    }
}
