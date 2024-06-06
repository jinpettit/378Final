using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public void PlayGame()
    {
        GlobalVariables.player1RoundsWon = 0;
        GlobalVariables.player2RoundsWon = 0;
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Credits()
    {
        SceneManager.LoadScene(2);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
