using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangerTower : TowerBase
{
    public Material hitLineMaterial;
    public Material missLineMaterial;
    public Material flashMaterial;

    // float representing how likely the ranger is to successfully regulate an angler
    [Range(0f, 1f)]
    public float regulationSuccessRate;

    // how many times the fish will flash in and out to show it is being caught
    public int numFlashesPerCatch;

    // fisherman tower that the catch attempt line is pointing at
    private FishermanTower catchAttemptFish;

    // LineRenderer component used to display a catch or catch attempt
    private LineRenderer regulateAttemptLine;

    /**
     * Start is called before the first frame update
     */
    protected override void Start()
    {
        base.Start();

        regulateAttemptLine = GetComponent<LineRenderer>();
        regulateAttemptLine.enabled = false;
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        // update fish line position
        if (regulateAttemptLine.enabled)
        {
            SetLinePos();
        }
    }

    /**
     * Apply the effects of the ranger tower
     */
    protected override void ApplyTowerEffect()
    {
        Collider[] fishermenColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.PLACED_OBJECTS))
            .Where((collider) => {
                return collider.GetComponentInChildren<FishermanTower>() != null;
            }).ToArray();

        if (fishermenColliders.Length > 0)
        {
            FishermanTower fishermanTower = fishermenColliders[Random.Range(0, fishermenColliders.Length)].GetComponent<FishermanTower>();

            transform.parent.LookAt(fishermanTower.transform, Vector3.back);

            RegulateFisherman(fishermanTower);
        }
    }

    /**
     * Position the ranger at the correct location using the information from a raycast
     * 
     * @param primaryHitInfo RaycastHit The results of the primary raycast that was done
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     */
    protected override void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        transform.position = primaryHitInfo.point;
    }

    /**
     * Determine whether a ranger could be placed at the location specified by a raycast
     * 
     * @param primaryHitInfo RaycastHit The results of the raycast that was done
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     */
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        int correctLayer = LayerMask.NameToLayer(Layers.TERRAIN_LAYER_NAME);

        // for placement to be valid, primary raycast must have hit a gameobject on the Terrain layer
        if (primaryHitInfo.collider && primaryHitInfo.collider.gameObject.layer == correctLayer)
        {
            // secondary raycasts must also hit gameobjects on the Terrain layer at approximately the same z-pos as the primary raycast
            return secondaryHitInfo.TrueForAll((hitInfo) =>
            {
                return hitInfo.collider &&
                       hitInfo.collider.gameObject.layer == correctLayer &&
                       Mathf.Abs(hitInfo.point.z - primaryHitInfo.point.z) < 0.1f;
            });
        }

        // if one of these conditions was not met, return false
        return false;
    }

    /**
     * Attempt to regulate an angler
     */
    private void RegulateFisherman(FishermanTower fishermanTower)
    {
        StartCoroutine(RegulateFishermanCoroutine(fishermanTower));
    }

    /**
     * Display attempt to catch fish
     */
    private IEnumerator RegulateFishermanCoroutine(FishermanTower fishermanTower)
    {
        // figure out whether the fish will be caught or not
        bool caught = Random.Range(0f, 1f) <= regulationSuccessRate;

        // do setup for regulation attempt line visualizer
        catchAttemptFish = fishermanTower;

        Destroy(regulateAttemptLine.material);
        regulateAttemptLine.material = caught ? hitLineMaterial : missLineMaterial;

        regulateAttemptLine.enabled = true;

        // handle fish being caught
        if (caught)
        {
            // make the fisherman flash  for a bit
            SkinnedMeshRenderer fishermanTowerRenderer = fishermanTower.transform.root.GetComponentInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < numFlashesPerCatch; i++)
            {
                Material oldMaterial = fishermanTowerRenderer.material;
                fishermanTowerRenderer.material = flashMaterial;
                yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                Destroy(fishermanTowerRenderer.material);
                fishermanTowerRenderer.material = oldMaterial;
                yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
            }

            // remove the fisherman tower
            Destroy(fishermanTower.transform.root.gameObject);
        }
        // fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }

        // end the catch attempt line
        regulateAttemptLine.enabled = false;
    }

    /**
     * Set line position for a fisherman we are targeting
     */
    private void SetLinePos()
    {
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 fishPos = catchAttemptFish.transform.position;
        fishPos.z = startPos.z;

        regulateAttemptLine.SetPositions(new Vector3[] { startPos, fishPos });
    }
}
