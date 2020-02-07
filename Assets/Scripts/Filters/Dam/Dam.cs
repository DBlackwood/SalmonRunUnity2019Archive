using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A dam restricts fish access to upstream areas
 */
public class Dam : FilterBase, IDragAndDropObject
{
    // default crossing rate for all fish
    [Range(0f, 1f)]
    public float defaultCrossingRate;
    // initialized in Unity
    // Project -> Assets -> Prefabs -> Filters -> Dam -> DamLadder
    // (make sure to double click on Dam cube symbol. 

    // crossing rates for small, medium, and large fish
    private float smallCrossingRate;
    private float mediumCrossingRate;
    private float largeCrossingRate;

    // is there a ladder currently attached to the dam?
    private bool hasLadder = false;

    // box where fish will be dropped off if the successfully pass the dam
    private BoxCollider dropOffBox;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // set all crossing rates to default rate on initialization
        smallCrossingRate = mediumCrossingRate = largeCrossingRate = defaultCrossingRate;
    }

    /** 
     * Update is called once per frame
     */
    void Update()
    {
        
    }

    #region Dam Operation

    /**
     * Activate the dam
     * 
     * @param dropOffBox BoxCollider the box where succesfully passing fish will be dropped off
     */
    public void Activate(BoxCollider dropOffBox)
    {
        this.dropOffBox = dropOffBox;
        active = true;
    }

    /**
     * Add a ladder to this dam
     * 
     * Set a flag so we can apply ladder effects later
     */
    public void AddLadder(DamLadder damLadder)
    {
        // set crossing rates for fish to ones supplied by the ladder
        smallCrossingRate = damLadder.smallCrossingRate;
        mediumCrossingRate = damLadder.mediumCrossingRate;
        largeCrossingRate = damLadder.largeCrossingRate;

        // set flag so we know we have a ladder
        hasLadder = true;
    }

    #endregion

    #region IDragAndDropObject Implementation

    /**
     * Place the dam onto the game map
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // can only place if we are over a dam placement location
        DamPlacementLocation placementLocation = primaryHitInfo.collider.gameObject.GetComponent<DamPlacementLocation>();
        if (placementLocation != null)
        {
            placementLocation.AttachDam(this);
        }
    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // must have hit something
        if (primaryHitInfo.collider)
        {
            DamPlacementLocation placementLocation = primaryHitInfo.collider.gameObject.GetComponent<DamPlacementLocation>();

            // thing we hit must be a dam placement location
            if (placementLocation != null)
            {
                // only return true if the placement location is not already in use
                return !placementLocation.inUse;
            }
        }

        return false;
    }

    #endregion

    #region Base Class (FilterBase) Implementation

    /**
     * Apply the effect of the dam
     * 
     * Fish will attempt to cross the dam and may be able to pass or "get stuck" and die
     */
    protected override void ApplyFilterEffect(Fish fish)
    {
        // only let it through if it hasn't been flagged as stuck
        if (!fish.IsStuck())
        {
            // chance between fish getting past the dam and being caught/getting stuck depends on what size the fish is
            float crossingRate;

            FishGenePair sizeGenePair = fish.GetGenome()[FishGenome.GeneType.Size];
            if (sizeGenePair.momGene == FishGenome.b && sizeGenePair.dadGene == FishGenome.b)
            {
                crossingRate = smallCrossingRate;
            }
            else if (sizeGenePair.momGene == FishGenome.B && sizeGenePair.dadGene == FishGenome.B)
            {
                crossingRate = mediumCrossingRate;
            }
            else
            {
                crossingRate = largeCrossingRate;
            }
            
            // based on the crossing rate we figured out, roll for a crossing
            // if we pass, put the fish past the dam
            if (Random.Range(0f, 1f) <= crossingRate)
            {
                fish.transform.position = GetRandomDropOff(fish.transform.position.z);
            }
            // if it didn't make it, make it permanently stuck (so it can't try repeated times)
            else
            {
                fish.SetStuck(true);
            }
        }
        
    }

    /**
     * Get a random point within the drop off collider
     * 
     * @param z float The z value of the point -- don't want to change the object's z so just pass it in
     */
    private Vector3 GetRandomDropOff(float z)
    {
        return new Vector3(
            Random.Range(dropOffBox.bounds.min.x, dropOffBox.bounds.max.x),
            Random.Range(dropOffBox.bounds.min.y, dropOffBox.bounds.max.y),
            z
        );
    }

    #endregion
}
