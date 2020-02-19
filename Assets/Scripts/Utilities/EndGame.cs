using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    /**
     * Enum containing reasons why the game is being quit
     */
    public enum Reason
    {
        ManualQuit,
        NoOffspring
    }

    /**
     * Quit the game
     */
    public static void Quit()
    {
        // TODO: for now, we're just gonna reload the scene -- that's probably not *exactly* what we're going to do
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
