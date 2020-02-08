using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftTopBarController : MonoBehaviour
{
    // turn counter text
    public Text turnCounterText;

    // coin text
    public Text coinText;

    /**
     * Initialization function
     */
    private void Awake()
    {
        GameEvents.onTurnUpdated.AddListener(UpdateTopBarUI);
    }

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        
    }

    /**
     * Update the UI for the top bar
     * 
     * Implementation of function from IUpdatableUI interface
     */
    private void UpdateTopBarUI()
    {
        turnCounterText.text = "Turn: " + GameManager.Instance.Turn.ToString();
    }
}
