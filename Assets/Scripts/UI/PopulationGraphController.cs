using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationGraphController : BottomBarContentPage
{
    // graph for successful vs. active vs. dead fish
    public HorizontalBarGraph populationGraph;

    // graph for female vs male fish
    public HorizontalBarGraph sexGraph;

    // graph for small vs medium vs large fish
    public HorizontalBarGraph sizeGraph;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // set up event calls
        GameEvents.onFishPopulationChanged.AddListener(UpdatePopulationData);
    }

    /**
     * Update UI to match updated population data
     */
    private void UpdatePopulationData(List<FishGenome> activeGenomes, List<FishGenome> successfulGenomes, List<FishGenome> deadGenomes)
    {
        populationGraph.UpdateGraph(successfulGenomes.Count, activeGenomes.Count, deadGenomes.Count);

        int numMales = FishGenomeUtilities.FindMaleGenomes(successfulGenomes).Count + FishGenomeUtilities.FindMaleGenomes(activeGenomes).Count;
        int numFemales = FishGenomeUtilities.FindFemaleGenomes(successfulGenomes).Count + FishGenomeUtilities.FindFemaleGenomes(activeGenomes).Count;
        sexGraph.UpdateGraph(numFemales, numMales);

        int numSmall = FishGenomeUtilities.FindSmallGenomes(successfulGenomes).Count + FishGenomeUtilities.FindSmallGenomes(activeGenomes).Count;
        int numMedium = FishGenomeUtilities.FindMediumGenomes(successfulGenomes).Count + FishGenomeUtilities.FindMediumGenomes(activeGenomes).Count;
        int numLarge = FishGenomeUtilities.FindLargeGenomes(successfulGenomes).Count + FishGenomeUtilities.FindLargeGenomes(activeGenomes).Count;
        sizeGraph.UpdateGraph(numSmall, numMedium, numLarge);
    }
}
