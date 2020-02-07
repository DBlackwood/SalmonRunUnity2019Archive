using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Base class for towers
 */
[RequireComponent(typeof(TowerRangeEffect))]
public abstract class TowerBase: MonoBehaviour, IDragAndDropObject, IPausable
{
    // effect radius of the tower
    [SerializeField]
    protected int effectRadius;

    // whether the tower is currently activated or not
    public bool TowerActive { get; set; } = false;

    // whether the tower is paused or not
    protected bool paused = true;

    // time between each application of tower effects
    [SerializeField]
    protected int timePerApplyEffect;

    // tower range effect script
    private TowerRangeEffect rangeEffect;

    #region Major Monobehaviour Functions

    /**
     * Handle initialization tasks common to all towers
     */
    protected virtual void Awake()
    {
        // get component references
        rangeEffect = GetComponent<TowerRangeEffect>();
    }

    /**
     * Handle tasks before the first frame update common to all towers
     */
    protected virtual void Start()
    {
        StartCoroutine(StartTowerEffectLoop());
    }

    #endregion

    #region Getters and Setters

    /**
     * Get the radius of the area the tower effects
     */
    public float GetEffectRadius()
    {
        return effectRadius;
    }

    #endregion

    #region Game Loop

    /**
     * Coroutine that will loop until the script is disabled or the object is
     * deactivated
     * 
     * Applies to    
     */
    protected IEnumerator StartTowerEffectLoop()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(timePerApplyEffect);
            if (TowerActive && !paused)
            {
                ApplyTowerEffect();
            }
        }
    }

    #endregion

    #region Pausable

    /**
     * Pause the tower
     */
    public void Pause()
    {
        paused = true;
    }

    /**
     * Resume the tower
     */
    public void Resume()
    {
        paused = false;
    }

    #endregion

    #region Placement

    /**
     * Apply effects of this tower
     */
    protected abstract void ApplyTowerEffect();

    /**
     * IDragAndDropInterface method implementation for PlacementValid
     * 
     * Determines if a location is valid for placement
     * 
     * This is the outwardly-facing function, which will take care of any
     * generic actions common to all towers and then rely on each class's
     * TowerPlacementValid implementation for actually determining if the
     * location is valid.
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        bool placementValid = TowerPlacementValid(primaryHitInfo, secondaryHitInfo);

        if (placementValid)
        {
            rangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Valid);
        }
        else
        {
            rangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Invalid);
        }

        return placementValid;
    }

    /**
     * IDragAndDropInterface method implementation for Place
     * 
     * Places a drag and drop object into the environment
     * 
     * This is the outwardly-facing function, which will take care of any
     * generic actions common to all towers and then rely on each class's
     * PlaceTower implementation for actually placing the tower.
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        TimeManager.Instance.RegisterPausable(this);

        PlaceTower(primaryHitInfo, secondaryHitInfo);

        rangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Off);

        TowerActive = true;
    }

    /**
     * Abstract function that each tower will implement
     * 
     * Determines whether a tower placement is valid
     */
    protected abstract bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo);

    /**
     * Abstract function that each tower will implement
     *
     * Places a tower into the environment   
     */
    protected abstract void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo);

    #endregion
}
