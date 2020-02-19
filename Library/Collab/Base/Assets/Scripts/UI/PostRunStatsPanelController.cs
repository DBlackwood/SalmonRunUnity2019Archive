using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PostRunStatsPanelController : PanelController
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

    // flag for case where there are no offspring left and we have to do something different when leaving the panel
    private bool noOffspring = false;

    /**
     * Called before the first frame update
     */
    private void Start()
    {
        // subscribe to events
        GameEvents.onNewGeneration.AddListener(OnNewGeneration);
        GameEvents.onStartRunStats.AddListener(Activate);

        // set inactive by default
        Deactivate();
    }

    /**
     * Handle pressing of Next Run button
     */
    public void OnNextRunButton()
    {
        // deactivate the panel
        Deactivate();

        // if there is fish in the next generation, just move on to place state
        if (!noOffspring)
        {
            GameManager.Instance.SetState(new PlaceState());
        }

        // otherwise, go to end panel
        else
        {
            GameManager.Instance.SetState(new EndState(EndGame.Reason.NoOffspring));
        }
    }

    /**
     * Handle event of a new generation
     */
    private void OnNewGeneration(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        UpdatePanelData(parentGenomes, offspringGenomes);

        // TODO: this probably should not be handled here in this way
        // something else should have the responsibility of checking if the game should be over -- I'm just trying to work quickly
        if (offspringGenomes.Count == 0)
        {
            OnNoOffspring();
        }
    }

    /**
     * Update the data that will be displayed on the panel
     */
    private void UpdatePanelData(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        // display the previous turn (because that's what
        titleText.text = "Turn " + (GameManager.Instance.Turn - 1).ToString() + " Summary";

        // if a full turn has been done, do the typical update
        if (GameManager.Instance.Turn > 1)
        {
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
                Debug.LogError("Error -- no parent genomes! should not happen!");
            }

            offspringSmallText.text = smallDescriptor + divider + FishGenomeUtilities.FindSmallGenomes(offspringGenomes).Count.ToString();
            offspringMediumText.text = mediumDescriptor + divider + FishGenomeUtilities.FindMediumGenomes(offspringGenomes).Count.ToString();
            offspringLargeText.text = largeDescriptor + divider + FishGenomeUtilities.FindLargeGenomes(offspringGenomes).Count.ToString();
            offspringFemaleText.text = femaleDescriptor + divider + FishGenomeUtilities.FindFemaleGenomes(offspringGenomes).Count.ToString();
            offspringMaleText.text = maleDescriptor + divider + FishGenomeUtilities.FindMaleGenomes(offspringGenomes).Count.ToString();
        }
        // otherwise, do the first-turn specific update
        else if (GameManager.Instance.Turn == 1)
        {
            parentSmallText.text = "N/A";
            parentMediumText.text = "N/A";
            parentLargeText.text = "N/A";

            offspringSmallText.text = smallDescriptor + divider + FishGenomeUtilities.FindSmallGenomes(offspringGenomes).Count.ToString();
            offspringMediumText.text = mediumDescriptor + divider + FishGenomeUtilities.FindMediumGenomes(offspringGenomes).Count.ToString();
            offspringLargeText.text = largeDescriptor + divider + FishGenomeUtilities.FindLargeGenomes(offspringGenomes).Count.ToString();
            offspringFemaleText.text = femaleDescriptor + divider + FishGenomeUtilities.FindFemaleGenomes(offspringGenomes).Count.ToString();
            offspringMaleText.text = maleDescriptor + divider + FishGenomeUtilities.FindMaleGenomes(offspringGenomes).Count.ToString();
        }
    }

    /**
     * Handle instance where there are no offpsring in the new generation
     */
    private void OnNoOffspring()
    {
        noOffspring = true;
    }
}
