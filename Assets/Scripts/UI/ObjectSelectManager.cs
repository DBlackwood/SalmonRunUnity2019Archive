using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelectManager : MonoBehaviour
{
    // tower that is currently selected
    private TowerBase selectedTower;
    private TowerRangeEffect selectedRangeEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        // only check when mouse is clicked
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // raycast into scene from mouse pos to determine what we clicked on
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // check if we actually hit an object that we care about
            if (Physics.Raycast(ray, out hit) && hit.collider)
            {
                CheckForTower(hit.collider.gameObject, Input.GetMouseButtonDown(0));
                CheckForFish(hit.collider.gameObject);
            }
        }
    }

    /**
     * Check for a tower and apply the effects that selecting a tower should trigger
     * 
     * @param hitObject GameObject The object that the raycast hit
     */
    private void CheckForTower(GameObject hitObject, bool isLeftMouse)
    {
        // flag to tell us later if we actually hit a tower
        bool hitTower = false;

        // attempt to get the TowerBase component from the object if it has one
        TowerBase towerTemp = hitObject.GetComponent<TowerBase>();

        // make sure that we found a TowerBase component that is active
        if (towerTemp != null && towerTemp.isActiveAndEnabled)
        {
            // set flag so we know later that we hit a tower
            hitTower = true;

            // turn off previous effect if there was one
            if (selectedRangeEffect != null && selectedRangeEffect != hitObject.GetComponent<TowerRangeEffect>())
            {
                // turn the neutral range effect off
                selectedRangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Off);
            }

            // set as selected tower
            selectedTower = towerTemp;

            // get the range effect component and update it to show the neutral tower range effect
            selectedRangeEffect = hitObject.GetComponent<TowerRangeEffect>();
            selectedRangeEffect.UpdateEffect(selectedRangeEffect.State == TowerRangeEffect.EffectState.Off ? TowerRangeEffect.EffectState.Neutral : TowerRangeEffect.EffectState.Off);
        }

        // if we did not hit a tower, remove selected tower
        if (!hitTower)
        {
            // remove reference
            if (selectedTower != null)
            {
                selectedTower = null;
            }

            // remove references to other components
            if (selectedRangeEffect != null)
            {
                // turn the neutral range effect off
                selectedRangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Off);
                selectedRangeEffect = null;
            }
        }
    }

    /**
     * Check for a fish and apply the effects that selecting a fish should trigger
     */
    private void CheckForFish(GameObject hitObject)
    {
        bool hitFish = false;

        Fish fish = hitObject.GetComponentInChildren<Fish>();

        if (fish != null && fish.isActiveAndEnabled)
        {
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Sex].momGene);
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Sex].dadGene);
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Size].momGene);
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Size].dadGene);
        }
    }
}
