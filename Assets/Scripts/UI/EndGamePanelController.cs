using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Controls panel that pops up when ending the game
 */
public class EndGamePanelController : PanelController
{
    // panel title text
    public Text titleText;

    // panel turn text
    public Text turnText;

    // panel message text
    public Text messageText;

    // affirmative button
    public GameObject confirmButton;

    // cancel button
    public GameObject cancelButton;

    // affirmative button text
    private Text confirmButtonText;

    // cancel button text
    private Text cancelButtonText;

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // get component refs
        confirmButtonText = confirmButton.GetComponentInChildren<Text>();
        cancelButtonText = cancelButton.GetComponentInChildren<Text>();

        // subscribe to events
        GameEvents.onEndGame.AddListener(OnEndOfGameEvent);

        // deactivated on start
        Deactivate();
    }

    /**
     * Handle end of game event
     */
    private void OnEndOfGameEvent(EndGame.Reason reason)
    {
        UpdatePanel(reason);
        Activate();
    }

    /**
     * Update panel text/data based on what the reason for ending the game is
     */
    private void UpdatePanel(EndGame.Reason reason)
    {
        // do all common updates
        UpdatePanelCommon();

        // do anything specific to the reason for ending the game
        switch(reason)
        {
            case EndGame.Reason.ManualQuit:
                titleText.text = "Quit?";
                messageText.text = "Are you sure?";
                confirmButton.SetActive(true);
                confirmButtonText.text = "Yes";
                cancelButton.SetActive(true);
                cancelButtonText.text = "Go Back";
                break;
            case EndGame.Reason.NoOffspring:
                titleText.text = "Game Over";
                messageText.text = "All fish have died! Press button to restart. Thanks for playing!";
                confirmButton.SetActive(true);
                confirmButtonText.text = "Restart";
                cancelButton.SetActive(false);
                break;
        }
    }

    /**
     * Update any panel text/data that works commonly across all reasons for ending the game
     */
    private void UpdatePanelCommon()
    {
        turnText.text = "Turns Lasted: " + (GameManager.Instance.Turn - 1).ToString();
    }

    /**
     * Handle pressing of end game button
     */
    public void OnConfirmButton()
    {
        EndGame.Quit();
    }

    /**
     * Handle pressing of cancel button
     */
    public void OnCancelButton()
    {
        Deactivate();
        GameManager.Instance.RevertState();
    }
}
