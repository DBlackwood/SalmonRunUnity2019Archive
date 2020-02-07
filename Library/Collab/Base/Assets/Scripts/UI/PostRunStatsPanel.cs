using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostRunStatsPanel : MonoBehaviour
{
    // panel's title, which displays the turn number
    public Text titleText;

    // text for parent data
    public Text parentSmallText;
    public Text parentMediumText;
    public Text parentLargeText;

    // text for offspring data
    public Text offspringSmallText;
    public Text offspringMediumText;
    public Text offspringLargeText;
    public Text offspringFemaleText;
    public Text offspringMaleText;

    /**
     * Called before the first frame update
     */
    private void Start()
    {
        // subscribe to events
        GameEvents.onNewGeneration.AddListener(UpdatePanelData);
        GameEvents.onEndRun.AddListener(Activate);

        // set inactive by default
        Deactivate();
    }

    /**
     * Activate the panel
     */
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    /**
     * Deactivate the panel
     */
    public void Deactivate()
    {
        GameManager.Instance.ToPlace();
        gameObject.SetActive(false);
    }

    /**
     * Update the data that will be displayed on the panel
     */
    private void UpdatePanelData(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        titleText.text = "Turn " + GameManager.Instance.Turn.ToString();

        // parent genome can be null if this is the initial generation and there are no parents
        // have to treat these two cases differently
        if (parentGenomes != null)
        {
            parentSmallText.text = "Small: " + FishGenomeUtilities.FindSmallGenomes(parentGenomes).Count.ToString();
            parentMediumText.text = "Medium: " + FishGenomeUtilities.FindMediumGenomes(parentGenomes).Count.ToString();
            parentLargeText.text = "Large: " + FishGenomeUtilities.FindLargeGenomes(parentGenomes).Count.ToString();
        }
        else
        {
            parentSmallText.text = "Small: N/A";
            parentMediumText.text = "Medium: N/A";
            parentLargeText.text = "Large: N/A";
        }

        offspringSmallText.text = "Small: " + FishGenomeUtilities.FindSmallGenomes(offspringGenomes).Count.ToString();
        offspringMediumText.text = "Medium: " + FishGenomeUtilities.FindMediumGenomes(offspringGenomes).Count.ToString();
        offspringLargeText.text = "Large: " + FishGenomeUtilities.FindLargeGenomes(offspringGenomes).Count.ToString();
        offspringFemaleText.text = "Female: " + FishGenomeUtilities.FindFemaleGenomes(offspringGenomes).Count.ToString();
        offspringMaleText.text = "Male: " + FishGenomeUtilities.FindMaleGenomes(offspringGenomes).Count.ToString();
    }
}
