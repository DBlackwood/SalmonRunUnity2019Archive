using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class that controlls a three-part bar graph
 */
public class HorizontalBarGraph : MonoBehaviour
{
    // rect tranfsorm array for bars/images representing the category being displayed on the left, center, and right of the graph
    public RectTransform[] bars;

    // strings for what each bar means
    public string[] barTitles;

    // Text to display the numeric value of each bar
    public Text[] barTextObjects;

    // how wide the entirety of the graph is
    private float graphWidth;

    /**
     * Initialization function
     */
    private void Awake()
    {
        // make sure we have minimum number of bars
        if (bars.Length < 2 || barTextObjects.Length < 2 || barTitles.Length < 2)
        {
            throw new System.Exception("Less than two bars or numeric displays or bar titles assigned to a horiz bar graph!");
        }

        // make sure number of bars, bar titles, and bar text objects match
        else if (bars.Length != barTitles.Length || bars.Length != barTextObjects.Length || barTitles.Length != barTextObjects.Length)
        {
            throw new System.Exception("Lengths of arrays for horiz bar graph do not match!");
        }

        // the graph width is taken from how wide the rightmost bar/image is in the scene
        // the bars overlap each other, so the width of the right bar is the width of the whole graph
        graphWidth = bars[bars.Length - 1].sizeDelta.x;
    }

    /**
     * Update the graph on-screen
     * 
     * Params for this function are a little tricky because we want ensure that 2 OR MORE values are provided
     * 
     * @param firstValue int The value corresponding to the first bar
     * @param secondValue in The value corresponding to the second bar
     * @param otherValues params int[] Values corresponding to the rest of the bars
     */
    public void UpdateGraph(int firstValue, int secondValue, params int[] otherValues)
    {
        // collect our values into a single array
        int[] allValues = new int[2 + otherValues.Length];
        allValues[0] = firstValue;
        allValues[1] = secondValue;
        for(int i = 0; i < otherValues.Length; i++)
        {
            allValues[2 + i] = otherValues[i];
        }

        // if we have more values than bars, issue a warning
        // we will continue onwards while ingoring the extra values since we can't show them
        // but the warning should inform the programmer that something's up
        if (allValues.Length != bars.Length)
        {
            Debug.LogError("Number of values supplied to graph does not match number of bars");
        }

        // figure out the total sum of all of the values (not counting any extra values we aren't displaying on-screen
        // if there is no corresponding value (because too few values were provided), just assume zero
        float totalSum = 0;
        for(int i = 0; i < bars.Length; i++)
        {
            totalSum += (i < allValues.Length) ? allValues[i] : 0;
        }

        // actually update the graph, looping through each bar
        float previousSum = 0;
        for (int i = 0; i < bars.Length; i++)
        {
            // all bars are (should be) left anchored at the same position, and the left parts of the bar draw over the right parts of the bar
            // so, we figure out how much the combined value of this bar and all previous bars
            float currentSum = previousSum + (i < allValues.Length ? allValues[i] : 0);

            // bar length is set to the a proportion of the full graph length corresponding to the current sum over the total sum
            bars[i].sizeDelta = new Vector2(currentSum / totalSum * graphWidth, bars[i].sizeDelta.y);

            // keep track of how big this bar was for use in the next bar
            previousSum = currentSum;

            // also update the numeric display text
            barTextObjects[i].text = barTitles[i] + "\n(" + allValues[i].ToString() + ")";
        }
    }
}
