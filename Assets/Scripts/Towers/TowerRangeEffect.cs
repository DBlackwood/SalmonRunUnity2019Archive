using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerBase))]
public class TowerRangeEffect : MonoBehaviour
{
    /**
     * Enum representing states the range effect can be in
     */
    public enum EffectState
    {
        Off,
        Neutral,
        Invalid,
        Valid
    }

    // materials to be applied for each non-off state
    public Material neutralMaterial;
    public Material invalidMaterial;
    public Material validMaterial;

    // object that actually holds the effect object/sprite
    [SerializeField]
    private GameObject rangeVisualizationObj;

    // the current state of this range effect
    private EffectState state;

    // the actual tower component this effect is for
    private TowerBase tower;

    // Start is called before the first frame update
    private void Start()
    {
        // get component reference
        tower = GetComponent<TowerBase>();

        UpdateRadius();
        UpdateEffect(EffectState.Off);
    }

    /**
     * Update the tower range effect's state
     */
    public void UpdateEffect(EffectState effectState)
    {
        // update the state
        state = effectState;

        // special case for turning it off
        if (state == EffectState.Off)
        {
            // turn off the range visualizer object
            rangeVisualizationObj.SetActive(false);
        }
        else
        {
            // make sure the range visualizer object is on
            rangeVisualizationObj.SetActive(true);

            // get the correct material to apply using switch statement
            Material m = neutralMaterial;
            switch (state)
            {

                case EffectState.Neutral:
                    m = neutralMaterial;
                    break;
                case EffectState.Invalid:
                    m = invalidMaterial;
                    break;
                case EffectState.Valid:
                    m = validMaterial;
                    break;
            }

            // get the visualizer's mesh renderer
            MeshRenderer mr = rangeVisualizationObj.GetComponent<MeshRenderer>();

            // destroy the old material instance to prevent memory leak
            Destroy(mr.material);

            // set the material to the correct one
            mr.material = m;       
         }

    }

    /**
     * Update the radius of the effect
     */
    public void UpdateRadius()
    {
        float radius = tower.GetEffectRadius();
        rangeVisualizationObj.transform.parent = null;
        rangeVisualizationObj.transform.localScale = new Vector3(radius * 2, rangeVisualizationObj.transform.localScale.y, radius * 2);
        rangeVisualizationObj.transform.parent = transform;
    }
}
