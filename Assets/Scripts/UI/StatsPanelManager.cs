using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/**
 * Controls the Stats Panel portion of the UI
 */
public class StatsPanelManager : MonoBehaviour
{
    // singleton instance
    public static StatsPanelManager Instance { get; private set; }

    // states the stats panel can be in
    public enum StatsPanelState
    {
        Population,
        Fish
    }

    // refs to each of the content panels
    public GameObject popStatsContent;
    public GameObject fishStatsContent;

    // refs to the button image for each sub-panel
    public Image popStatsButtonImage;
    public Image fishStatsButtonImage;

    // refs for selected and not-selected button sprites
    public Sprite selectedButtonSprite;
    public Sprite notSelectedButtonSprite;

    // refs for pop stats sub-panel
    public Image sexGraphFemale;
    public Image sexGraphMale;
    public Image sizeGraphSmall;
    public Image sizeGraphMedium;
    public Image sizeGraphLarge;

    // state that the stats panel will start in
    public StatsPanelState initialState;

    // dictionary mapping panel states to the content those states should display
    private Dictionary<StatsPanelState, GameObject> stateContentDict;

    // the fish displayed by the fish section
    private Fish currentFish;

    // current state of the panel
    private StatsPanelState state;

    /**
     * Initialization function
     */
    private void Awake()
    {
        // set up and/or check for issues with singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one stats panel manager in the scene!");
            Destroy(gameObject);
        }
    }

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // initialize the state/content dict
        stateContentDict = new Dictionary<StatsPanelState, GameObject>();
        stateContentDict.Add(StatsPanelState.Population, popStatsContent);
        stateContentDict.Add(StatsPanelState.Fish, fishStatsContent);

        // ensure there aren't any null refs in content
        foreach (GameObject content in stateContentDict.Values)
        {
            if (content == null)
            {
                Debug.LogError("Null content in the stats panel manager!");
            }
        }

        // set initial state
        SetState(initialState);


    }

    /**
     * Button-visible function for calling SetState, using the index in the enum
     * 
     * This is because button functions can't use enum params
     */
    public void SetState(int stateIndex)
    {
        SetState(Enum.GetValues(typeof(StatsPanelState)).Cast<StatsPanelState>().ToArray()[stateIndex]);
    }

    /**
     * Update which fish is currently being displayed by the Fish panel
     */
    public void UpdateFish(Fish fish)
    {
        Debug.Log(fish);
        currentFish = fish;
    }

    /**
     * Set the state of the panel
     */
    private void SetState(StatsPanelState state)
    {
        // set the state
        this.state = state;

        // update the panel content by enabling the correct content and disabling everything else
        foreach (StatsPanelState possibleState in stateContentDict.Keys)
        {
            stateContentDict[possibleState].SetActive(possibleState == state);
        }

        switch (state)
        {
            case StatsPanelState.Population:
                sexGraphFemale.GetComponent<RectTransform>().sizeDelta = new Vector2(54f * 130f / 100f, sexGraphFemale.GetComponent<RectTransform>().sizeDelta.y);
                sexGraphMale.GetComponent<RectTransform>().sizeDelta = new Vector2(46f * 130f / 100f, sexGraphMale.GetComponent<RectTransform>().sizeDelta.y);

                sizeGraphSmall.GetComponent<RectTransform>().sizeDelta = new Vector2(26f * 130f / 100f, sizeGraphSmall.GetComponent<RectTransform>().sizeDelta.y);
                sizeGraphMedium.GetComponent<RectTransform>().sizeDelta = new Vector2(48f * 130f / 100f, sizeGraphMedium.GetComponent<RectTransform>().sizeDelta.y);
                sizeGraphLarge.GetComponent<RectTransform>().sizeDelta = new Vector2(26f * 130f / 100f, sizeGraphLarge.GetComponent<RectTransform>().sizeDelta.y);

                popStatsButtonImage.sprite = selectedButtonSprite;
                fishStatsButtonImage.sprite = notSelectedButtonSprite;
                break;
            case StatsPanelState.Fish:
                popStatsButtonImage.sprite = notSelectedButtonSprite;
                fishStatsButtonImage.sprite = selectedButtonSprite;
                break;
        }
    }
}
