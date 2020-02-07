using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerBase))]
public class TowerRangeEffect : MonoBehaviour
{
    // currently selected tower effect
    public static TowerRangeEffect currentlySelectedRangeEffect { get; private set; }

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
    public EffectState State { get; private set; } = EffectState.Off;

    // the actual tower component this effect is for
    private TowerBase tower;

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // get component reference
        tower = GetComponent<TowerBase>();

        UpdateRadius();
        UpdateEffect(EffectState.Off);
    }

    /**
     * Handle left click on the tower range effect
     */
    private void OnMouseDown()
    {
        // make sure the tower is on
        if (tower.isActiveAndEnabled)
        {
            // if a tower effect is already on and it's not this one, turn it off
            if (currentlySelectedRangeEffect != null && currentlySelectedRangeEffect != this)
            {
                currentlySelectedRangeEffect.UpdateEffect(EffectState.Off);
            }

            // update currently selected range effect
            currentlySelectedRangeEffect = this;

            // toggle the selected range effect
            UpdateEffect(State == EffectState.Off ? EffectState.Neutral : EffectState.Off);
        }
    }

    /**
     * Update the tower range effect's state
     */
    public void UpdateEffect(EffectState effectState)
    {
        // update the state
        State = effectState;

        // special case for turning it off
        if (State == EffectState.Off)
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
            switch (State)
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
