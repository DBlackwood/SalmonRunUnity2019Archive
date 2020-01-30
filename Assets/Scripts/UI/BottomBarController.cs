using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BottomBarController : MonoBehaviour
{
    [Header("Expand/Contract")]
    // the distance up and down the bar will move when the user presses the expand/contract button
    public float expandContractDistance;

    [Header("Content Page Buttons")]
    // prefab for buttons that will be used to switch between content pages
    public Button contentPageButtonPrefab;

    // initial position of the first button
    public Vector2 initialButtonPosition;

    // spacing between buttons
    public Vector2 distanceBetweenButtons;

    // bottom bar's rect transform
    private RectTransform rectTransform;

    // is the bottom bar currently expanded?
    private bool expanded = true;

    // list of all content pages that can be shown in the bottom bar
    private BottomBarContentPage[] contentPages;

    // index of currently active content page
    private int activeContentPageIndex;

    /**
     * Initialization function
     */
    private void Awake()
    {
        // get component refs
        rectTransform = GetComponent<RectTransform>();
    }

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // get all content pages
        contentPages = GetComponentsInChildren<BottomBarContentPage>();

        // set up the content pages
        SetupContentPageButtons();

        // activate the first one
        SetActivePage(0);
    }

    /**
     * Handle expand/contract button being pressed
     */
    public void ExpandContractButtonPressed()
    {
        // move the panel up or down, depending on whether we want to expand or "contract"
        float moveDistance = expanded ? -1 * expandContractDistance: expandContractDistance;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + moveDistance);

        // toggle our flag in code for whether we're expaned or not
        expanded = !expanded;
    }

    /**
     * Setup buttons for our content pages
     */
    private void SetupContentPageButtons()
    {
        // make a button for each content page
        for (int i = 0; i < contentPages.Length; i++)
        {
            // instantiate the button
            Button button = Instantiate(contentPageButtonPrefab, transform);

            // place the button in the correct position
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = initialButtonPosition + (i * distanceBetweenButtons);

            // set the button text
            button.GetComponentInChildren<Text>().text = contentPages[i].pageTitle;

            // tell the button what to do when it is clicked on
            int temp = i; // need a temp var because i will always end up evaluating to the last value in the for loop
            button.onClick.AddListener(() => SetActivePage(temp));

        }
    }

    /**
     * Handle one of the buttons being clicked
     */
    private void SetActivePage(int index)
    {
        // set our index so we can keep track of which page is active
        activeContentPageIndex = index;

        // loop through all content page
        // turn on the newly activated page and deactivate all others
        for(int i = 0; i < contentPages.Length; i++)
        {
            contentPages[i].Active = (activeContentPageIndex == i);
        }

        // make sure the bar is expanded
        if (!expanded)
        {
            ExpandContractButtonPressed();
        }
    }
    
}
