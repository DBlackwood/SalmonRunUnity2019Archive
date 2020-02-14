using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangerTower : TowerBase
{
    /**
     * Modes that the ranger can be in
     */
    public enum Mode
    {
        Kill,
        Slowdown
    }

    // current mode that the ranger is in
    public Mode mode;

    // materials for lines that show what angler the ranger is affecting
    public Material hitLineMaterial;
    public Material missLineMaterial;

    // material for making an angler flash to show it is being affected
    public Material flashMaterial;

    // prefab for line renderer
    public GameObject lineRendererPrefab;

    // float representing how much of an effect the ranger should have if it regulates an angler in slowdown mode for small fish
    [Range(-1f, 1f)]
    public float slowdownEffectSmall;

    // float representing how much of an effect the ranger should have if it regulates an angler in slowdown mode for medium fish
    [Range(-1f, 1f)]
    public float slowdownEffectMedium;

    // float representing how much of an effect the ranger should have if it regulates an angler in slowdown mode for large fish
    [Range(-1f, 1f)]
    public float slowdownEffectLarge;

    // float representing how likely the ranger is to successfully regulate an angler
    [Range(0f, 1f)]
    public float regulationSuccessRate;

    // how many times the fish will flash in and out to show it is being caught
    public int numFlashesPerCatch;

    // list of linerenderers used to show effects
    private List<LineRenderer> towerEffectLineRenderers = new List<LineRenderer>();

    // list of linerenderer pos
    private List<Vector3> towerEffectPositions = new List<Vector3>();

    /**
     * Start is called before the first frame update
     */
    protected override void Start()
    {
        base.Start();
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        // update fishermen line positions
        SetLinePositions();
    }

    /**
     * Apply the effects of the ranger tower
     */
    protected override void ApplyTowerEffect()
    {
        Collider[] fishermenColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.PLACED_OBJECTS))
            .Where((collider) => {
                return collider.GetComponentInChildren<FishermanTower>() != null && collider.GetComponentInChildren<FishermanTower>().TowerActive;
            }).ToArray();

        
        foreach (Collider fishermanCollider in fishermenColliders)
        {
            FishermanTower fishermanTower = fishermanCollider.GetComponent<FishermanTower>();

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
        // figure out whether the fisherman will be stopped or not
        bool caught = Random.Range(0f, 1f) <= regulationSuccessRate;

        GameObject g = Instantiate(lineRendererPrefab, transform);
        LineRenderer lr = g.GetComponent<LineRenderer>();

        lr.material = caught ? hitLineMaterial : missLineMaterial;

        lr.enabled = true;

        towerEffectLineRenderers.Add(lr);

        Vector3 towerEffectPosition = fishermanTower.transform.position;
        towerEffectPositions.Add(towerEffectPosition);

        // handle fish being caught
        if (caught)
        {
            // want this variable so we can make the fisherman flash, regardless of what mode we're in
            MeshRenderer fishermanTowerRenderer = fishermanTower.flashRenderer;

            // how we handle this depends on what mode the ranger is in
            switch (mode)
            {
                case Mode.Kill:
                    // make the fisherman inactive
                    fishermanTower.TowerActive = false;

                    // make the fisherman flash for a bit
                    for (int i = 0; i < numFlashesPerCatch; i++)
                    {
                        Material oldMaterial = null;
                        if (fishermanTowerRenderer != null)
                        {
                             oldMaterial = fishermanTowerRenderer.material;
                            fishermanTowerRenderer.material = flashMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);

                        if (fishermanTowerRenderer != null)
                        {
                            Destroy(fishermanTowerRenderer.material);
                            fishermanTowerRenderer.material = oldMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                    }

                    // remove the fisherman tower
                    if (fishermanTower != null)
                    {
                        Destroy(fishermanTower.transform.root.gameObject);
                    }
                    break;
                case Mode.Slowdown:
                    // apply the affect to the angler
                    fishermanTower.AffectCatchRate(slowdownEffectSmall, slowdownEffectMedium, slowdownEffectLarge, timePerApplyEffect);

                    // make the fisherman flash  for a bit
                    for (int i = 0; i < numFlashesPerCatch; i++)
                    {
                        Material oldMaterial = null;
                        if (fishermanTowerRenderer != null)
                        {
                            oldMaterial = fishermanTowerRenderer.material;
                            fishermanTowerRenderer.material = flashMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                        if (fishermanTowerRenderer != null)
                        {
                            Destroy(fishermanTowerRenderer.material);
                            fishermanTowerRenderer.material = oldMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                    }
                    break;
            }
        }
        // fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }

        // end the catch attempt line
        towerEffectPositions.Remove(towerEffectPosition);
        towerEffectLineRenderers.Remove(lr);
        Destroy(g);
        
    }

    /**
     * Set line position for a fisherman we are targeting
     */
    private void SetLinePositions()
    {
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 40);

        for (int i = 0; i < towerEffectLineRenderers.Count; i++)
        {
            Vector3 endPos = towerEffectPositions[i];
            endPos.z = startPos.z;

            towerEffectLineRenderers[i].SetPositions(new Vector3[] { startPos, endPos });
        }
    }
}
