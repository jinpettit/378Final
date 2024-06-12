using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    [SerializeField] Animator transitionAnim;
    public void CastleStage()
    {
        GlobalVariables.player1RoundsWon = 0;
        GlobalVariables.player2RoundsWon = 0;
        StartCoroutine(LoadLevel(3));
    }
    public void FieldStage()
    {
        GlobalVariables.player1RoundsWon = 0;
        GlobalVariables.player2RoundsWon = 0;
        StartCoroutine(LoadLevel(4));
    }

    public void TowerStage()
    {
        GlobalVariables.player1RoundsWon = 0;
        GlobalVariables.player2RoundsWon = 0;
        StartCoroutine(LoadLevel(5));
    }
    public void StageSelect()
    {
        StartCoroutine(LoadLevel(2));
    }


    public void MainMenu()
    {
        StartCoroutine(LoadLevel(0));
    }

    public void Credits()
    {
        SceneManager.LoadScene(1);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadLevel(int index) 
    {
        transitionAnim.SetTrigger("End");
        audioManager.PlaySFX(audioManager.contact);
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(index);
        transitionAnim.SetTrigger("Start");
    }
}
