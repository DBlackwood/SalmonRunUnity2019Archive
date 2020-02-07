using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

/**
 * Custom inspector for water grid & accompanying vector field
 * 
 * Author/s: Jacob Cousineau
 */
[CustomEditor(typeof(WaterGridController)), CanEditMultipleObjects]
public class WaterGridInspector : Editor
{
    private WaterGridController controller;
    private WaterGridVectorField vectorField;

    // index of the currently selected vector in the field
    private int selectedVectorIndex = -1;

    /**
     * Draw the inspector
     */
    public override void OnInspectorGUI()
    {
        // draw the normal inspector
        base.OnInspectorGUI();

        // if a selection is made and it corresponds to an actual water tile, show a vector2 field where you can edit the vector value directly
        if (selectedVectorIndex >= 0)
        {
            // draw line separating this from the rest of the inspector
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            // draw the vector2 field
            vectorField.Vectors[selectedVectorIndex] = EditorGUILayout.Vector2Field("Currently Selected Vector: ", vectorField.Vectors[selectedVectorIndex]);


        }
    }

    /**
     * Draw handles when the scene GUI is being drawn
     */
    protected virtual void OnSceneGUI()
    {
        // draw handles for all of the vectors
        for (int index = 0; index < vectorField.Vectors.Length; index++)
        {
            // only draw a handle if there's actually a water tile there
            Vector2Int index2D = controller.GetTwoDimensionalIndex(index);
            if (controller.WaterTileAt(index2D.x, index2D.y))
            {
                Vector2 viewportPoint = Camera.current.WorldToViewportPoint(controller.GetCellCenterWorld(index2D.x, index2D.y));
                if (Mathf.Abs(viewportPoint.x) < 1 && Mathf.Abs(viewportPoint.y) < 1)
                {
                    // begin checking for changes in handle movement
                    // this is standard Unity code for using custom handles
                    EditorGUI.BeginChangeCheck();

                    if (index == selectedVectorIndex)
                    {
                        Handles.color = Color.yellow;
                    }

                    // draw a custom, rectangle handle at the tip of the vector (origin of vector is at center of the cell)
                    Vector3 newTargetPosition = Handles.Slider2D(controller.GetCellCenterWorld(index2D.x, index2D.y) + (Vector3)vectorField.Vectors[index], Vector3.forward, Vector3.up, Vector3.right, 0.1f * controller.grid.transform.localScale.x, Handles.RectangleHandleCap, new Vector2(0.1f, 0.1f));

                    if (index == selectedVectorIndex)
                    {
                        Handles.color = Color.white;
                    }

                    // if a change was made, update the vector field
                    // again, this if statement is standard Unity code for using custom handles
                    if (EditorGUI.EndChangeCheck())
                    {
                        // standard Unity code...
                        Undo.RecordObject(controller, "Change Look At Target Position");

                        // create the vector that will go in the vector field, remembering to make the vector absolute rather than relative to the center of the cell
                        Vector2 newVector = newTargetPosition - controller.GetCellCenterWorld(index2D.x, index2D.y);

                        // if flag is set to limit vectors to within their grid square, enforce the limit
                        if (controller.limitVectorsToGridSquare)
                        {
                            float gridSquareSize = controller.grid.cellSize.x * controller.grid.transform.localScale.x;

                            if (newVector.magnitude > gridSquareSize)
                            {
                                newVector = newVector.normalized * gridSquareSize;
                            }
                        }

                        // actually add the vector to the field
                        vectorField.Vectors[index] = newVector;

                        Repaint();

                        // since this is the last vector we touched, keep track of that so we can show its values in the inspector
                        selectedVectorIndex = index;
                    }
                }
            }
        }
    }

    /**
     * Draw gizmos for the vector field.
     */
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawWaterGridControllerGizmos(WaterGridController controller, GizmoType gizmoType)
    {
        // make sure the controller exists
        if (controller)
        {
            // loop through all vectors in the field
            for (int x = 0; x < controller.VFWidth; x++)
            {
                for (int y = 0; y < controller.VFHeight; y++)
                {
                    // check to see if there's actually a water tile at this cell
                    if (controller.WaterTileAt(x,y))
                    {
                        // draw an arrow gizmo originating from the center of the cell
                        Vector3 center = controller.GetCellCenterWorld(x, y);
                        if (controller.GetVector(x,y) != Vector2.zero)
                        {
                            DrawArrow.ForGizmo(center, controller.GetVector(x, y), 0.25f * controller.grid.transform.localScale.x);
                        }
                    }
                }
            }
        }
    }

    /**
     * Handler for whenever the custom inspector is enabled
     */
    protected void OnEnable()
    {
        controller = (WaterGridController)target;
        vectorField = controller.vectorField;

        vectorField.LoadFromFile(controller.tilemap);
    }

    /**
     * Handler for whenever the custom inspector is disabled
     */
    protected void OnDisable()
    {
        vectorField.SaveToFileFromEditor();
    }
}
