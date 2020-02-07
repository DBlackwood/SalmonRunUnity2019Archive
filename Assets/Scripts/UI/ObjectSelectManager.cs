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
        // only check when left mouse is clicked
        if (Input.GetMouseButtonDown(0))
        {
            

            // raycast into scene from mouse pos to determine what we clicked on
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // check if we actually hit an object that we care about
            if (Physics.Raycast(ray, out hit) && hit.collider)
            {
                CheckForTower(hit.collider.gameObject);
                CheckForFish(hit.collider.gameObject);
            } 

            
        }
    }

    /**
     * Check for a tower and apply the effects that selecting a tower should trigger
     */
    private void CheckForTower(GameObject hitObject)
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

            // set as selected tower
            selectedTower = towerTemp;

            // get the range effect component and update it to show the neutral tower range effect
            selectedRangeEffect = hitObject.GetComponent<TowerRangeEffect>();
            selectedRangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Neutral);
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
            // if we hit a fish, set the stats panel to fish mode and update it
            StatsPanelManager.Instance.UpdateFish(fish);
            StatsPanelManager.Instance.SetState(1);
        }
    }
}
