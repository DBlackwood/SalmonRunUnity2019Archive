using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostRunStatsPanel : MonoBehaviour
{
    // descriptor text for each field on the panel
    public string smallDescriptor;
    public string mediumDescriptor;
    public string largeDescriptor;
    public string femaleDescriptor;
    public string maleDescriptor;

    public const string divider = ": ";

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
            parentSmallText.text = smallDescriptor + divider + FishGenomeUtilities.FindSmallGenomes(parentGenomes).Count.ToString();
            parentMediumText.text = mediumDescriptor + divider + FishGenomeUtilities.FindMediumGenomes(parentGenomes).Count.ToString();
            parentLargeText.text = largeDescriptor + divider + FishGenomeUtilities.FindLargeGenomes(parentGenomes).Count.ToString();
        }
        else
        {
            parentSmallText.text = smallDescriptor + divider + "N/A";
            parentMediumText.text = mediumDescriptor + divider + "N/A";
            parentLargeText.text = largeDescriptor + divider + "N/A";
        }

        offspringSmallText.text = smallDescriptor + divider + FishGenomeUtilities.FindSmallGenomes(offspringGenomes).Count.ToString();
        offspringMediumText.text = mediumDescriptor + divider + FishGenomeUtilities.FindMediumGenomes(offspringGenomes).Count.ToString();
        offspringLargeText.text = largeDescriptor + divider + FishGenomeUtilities.FindLargeGenomes(offspringGenomes).Count.ToString();
        offspringFemaleText.text = femaleDescriptor + divider + FishGenomeUtilities.FindFemaleGenomes(offspringGenomes).Count.ToString();
        offspringMaleText.text = maleDescriptor + divider + FishGenomeUtilities.FindMaleGenomes(offspringGenomes).Count.ToString();
    }
}
