using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Script that controls a drag and drop icon for dragging towers onto the play area.
 * 
 * Author: Jacob Cousineau
 */
[RequireComponent(typeof(RectTransform))]
public class DragAndDropIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    // the object we should attempt to spawn
    public GameObject objectToSpawn;  // if public it will show up in the inspector; this includes varibles that might be have a start or default value
                                      // public color mycolor with show up as a color picker to get a start color
                                      // can put a hide inspector. [SerializeField] will also show up in inspector, but not be public
                                      // Serialize Field will also keep its value between runs. 

    // the rect transform for the icon
    private RectTransform rectTransform;

    // the instance of the object we will (attempt to) spawn
    private GameObject spawnedObject;

    // layer the spawned object started on and will be restored to once placed
    private LayerMask spawnedObjectLayer;

    // the Tower interface attached to the spawned object
    private IDragAndDropObject spawnedObjectDragAndDrop;

    // hold offsets for corners of spawned object renderer's bounding box
    // used for raycasting from all four corners of the spawned object in addition to the center
    private Vector3 spawnedObjectOffsetTopLeft;
    private Vector3 spawnedObjectOffsetTopRight;
    private Vector3 spawnedObjectOffsetBottomLeft;
    private Vector3 spawnedObjectOffsetBottomRight;

    // holds layer mask for raycasting
    private int raycastLayerMask;

    /**
     * Initialization
     */
    void Start ()
    {
        // get rect transform component
        rectTransform = GetComponent<RectTransform>();

        // get layer mask for necessary layers
        raycastLayerMask = LayerMask.GetMask(Layers.FLOOR_LAYER_NAME, Layers.TERRAIN_LAYER_NAME, Layers.PLACED_OBJECTS);

    }


    /**
     * Handle beginning of a drag action on this object
     */
    public void OnBeginDrag(PointerEventData eventData)
    {
        // set the icon to be on top of other sibling objects
        rectTransform.SetAsLastSibling();

        // spawn the object
        // keep it invisible and don't set position yet because we haven't actually figured out where it's going to go yet
        // but, we need to instantiate it so we can figure out whether 
        spawnedObject = Instantiate(objectToSpawn);

        // set the spawned object to be on the unplaced object layer, keeping its old layer for future use
        spawnedObjectLayer = spawnedObject.layer;
        Layers.MoveAllToLayer(spawnedObject, LayerMask.NameToLayer(Layers.IGNORE_RAYCAST_LAYER_NAME));

        // get the IDragAndDrop interface for the spawned object
        spawnedObjectDragAndDrop = spawnedObject.GetComponentInChildren<IDragAndDropObject>();
        if (spawnedObjectDragAndDrop == null)
        {
            Debug.LogError("Trying to drag and drop an object with no IDragAndDrop interface implemented!");
        }
        else
        {
            // show any visualizations that may exist for the object
            PlacementVisualizationManager.Instance.DisplayVisualization(spawnedObjectDragAndDrop.GetType(), true);
        }

        // calculate the offsets needed to get the points at the corners of the spawned object's renderer's bounding box
        MeshRenderer mr = (spawnedObjectDragAndDrop as MonoBehaviour).GetComponent<MeshRenderer>();
        if (!mr)
        {
            Debug.LogError("Attempting to spawn object from drag and drop that has no MeshRenderer!");
        }
        else
        {
            spawnedObjectOffsetTopLeft = new Vector3(-1 * mr.bounds.extents.x, mr.bounds.extents.y);
            spawnedObjectOffsetTopRight = new Vector3(mr.bounds.extents.x, mr.bounds.extents.y);
            spawnedObjectOffsetBottomLeft = new Vector3(-1 * mr.bounds.extents.x, -1 * mr.bounds.extents.y);
            spawnedObjectOffsetBottomRight = new Vector3(mr.bounds.extents.x, -1 * mr.bounds.extents.y);
        }
        // start moving the icon
        UpdateDrag(eventData);

    }

    /**
     * Handle middle of drag action on this object
     */
    public void OnDrag(PointerEventData eventData)
    {
        UpdateDrag(eventData);
    }

    /**
     * Handle end of drag action on this object
     */
    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateDrag(eventData, true);
    }

    /**
     * Make the object follow the pointer as a drag action is occuring
     * 
     * Originally taken from Unity documentation (https://docs.unity3d.com/ScriptReference/EventSystems.IDragHandler.html) but has been heavily modified
     * 
     * @param data PointerEventData The pointer data passed in from OnDragBegin or OnDrag
     * @param finalUpdate bool True if this is the last update at the end of a drag
     */
    private void UpdateDrag(PointerEventData data, bool finalUpdate = false)
    {
        // create a ray from the camera towards the terrain
        Ray primaryRay = Camera.main.ScreenPointToRay(data.position);

        // create a list to hold all of the results from raycasts
        List<RaycastHit> secondaryHitInfo = new List<RaycastHit>();

        // create variable to store initial raycast result data
        RaycastHit primaryHitInfo;

        // attempt a raycast on the terrain and floor (water tilemap) layers
        bool hit = Physics.Raycast(primaryRay, out primaryHitInfo, Mathf.Infinity, raycastLayerMask);
        if (hit)
        {
            // we hit something!

            // if we're in the editor, draw the ray
#if UNITY_EDITOR
            Debug.DrawRay(primaryRay.origin, primaryRay.direction * primaryHitInfo.distance, Color.green);
#endif

            // update the location of the spawned object to be at the point of the raycast hit
            spawnedObject.transform.position = primaryHitInfo.point;

            // do some secondary raycasts to get more information
            // this gets a bit complicated...

            // figure out the origin for a ray that originates on the camera's z plane
            // and goes through the center of the drag and drop object at a 90 degree angle
            float raycastOriginDepth = primaryHitInfo.point.z - Camera.main.transform.position.z;
            Vector3 raycastOrigin = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, raycastOriginDepth));

            // want to raycast from the camera z, so reset the z value
            raycastOrigin.z = Camera.main.transform.position.z;

            // make modifications to this origin so that the new origins are at each corner of the drag and drop object's bounding box
            Vector3 raycastOriginTopLeft = raycastOrigin + spawnedObjectOffsetTopLeft;
            Vector3 raycastOriginTopRight = raycastOrigin + spawnedObjectOffsetTopRight;
            Vector3 raycastOriginBottomLeft = raycastOrigin + spawnedObjectOffsetBottomLeft;
            Vector3 raycastOriginBottomRight = raycastOrigin + spawnedObjectOffsetBottomRight;

            // do raycasts from each of these origins forward into the scene, storing their results
            RaycastHit topLeftHitInfo;
            RaycastHit topRightHitInfo;
            RaycastHit bottomLeftHitInfo;
            RaycastHit bottomRightHitInfo;
            Physics.Raycast(raycastOriginTopLeft, Vector3.forward, out topLeftHitInfo, Mathf.Infinity, raycastLayerMask);
            Physics.Raycast(raycastOriginTopRight, Vector3.forward, out topRightHitInfo, Mathf.Infinity, raycastLayerMask);
            Physics.Raycast(raycastOriginBottomLeft, Vector3.forward, out bottomLeftHitInfo, Mathf.Infinity, raycastLayerMask);
            Physics.Raycast(raycastOriginBottomRight, Vector3.forward, out bottomRightHitInfo, Mathf.Infinity, raycastLayerMask);

            // draw these raycasts for debugging purposes
            Debug.DrawRay(raycastOriginTopLeft, Vector3.forward * (topLeftHitInfo.point.z - Camera.main.transform.position.z), Color.blue);
            Debug.DrawRay(raycastOriginTopRight, Vector3.forward * (topRightHitInfo.point.z - Camera.main.transform.position.z), Color.blue);
            Debug.DrawRay(raycastOriginBottomLeft, Vector3.forward * (bottomLeftHitInfo.point.z - Camera.main.transform.position.z), Color.blue);
            Debug.DrawRay(raycastOriginBottomRight, Vector3.forward * (bottomRightHitInfo.point.z - Camera.main.transform.position.z), Color.blue);

            // put the secondary raycasts in a list to send along to the drag and drop object
            secondaryHitInfo.Add(topLeftHitInfo);
            secondaryHitInfo.Add(topRightHitInfo);
            secondaryHitInfo.Add(bottomLeftHitInfo);
            secondaryHitInfo.Add(bottomRightHitInfo);
        }

        // check if the current location of the spawn is valid
        bool locationValid = spawnedObjectDragAndDrop.PlacementValid(primaryHitInfo, secondaryHitInfo);

        // if this is the final drag update, call a method to handle it
        if (finalUpdate)
        {
            FinalizeDrag(locationValid, primaryHitInfo, secondaryHitInfo);
        }
    }


    /**
     * Finalize spawning of objects at the end of a drag and drop.
     * 
     * @param locationValid bool True if the cursor is over a valid location to place the object
     * @param hitInfo RaycastHit Result of the raycast done to test location validity
     */
    private void FinalizeDrag(bool locationValid, RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // hide any visualizations that may have existed for the object
        PlacementVisualizationManager.Instance.DisplayVisualization(spawnedObjectDragAndDrop.GetType(), false);

        // if we are in a valid place, spawn the object
        if (locationValid)
        {
            // place the spawned object
            spawnedObjectDragAndDrop.Place(primaryHitInfo, secondaryHitInfo);

            // make it visible and turn it on
            spawnedObject.SetActive(true);

            // set the layer back to whatever it was before
            Layers.MoveAllToLayer(spawnedObject, spawnedObjectLayer);
        }
        // if not in a valid place, destroy the spawned object
        else
        {
            // destroy object
            Destroy(spawnedObject);

            // clear old references, reset values to default
            spawnedObject = null;
            spawnedObjectDragAndDrop = null;
            spawnedObjectOffsetTopLeft = Vector3.zero;
            spawnedObjectOffsetTopRight = Vector3.zero;
            spawnedObjectOffsetBottomLeft = Vector3.zero;
            spawnedObjectOffsetBottomRight = Vector3.zero;
        }
    }
}
