using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI player1RoundsText;
    public TextMeshProUGUI player2RoundsText;
    public TextMeshProUGUI player1WinText;
    public TextMeshProUGUI player2WinText;
    public static GameManager instance;

    private int roundsToWin = 2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        player1WinText.gameObject.SetActive(false);
        player2WinText.gameObject.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        player1RoundsText.text = "Player 1: " + GlobalVariables.player1RoundsWon;
        player2RoundsText.text = "Player 2: " + GlobalVariables.player2RoundsWon;
    }

    public void Player1WinsRound()
    {
        GlobalVariables.player1RoundsWon++;
        UpdateUI();

        if (GlobalVariables.player1RoundsWon >= roundsToWin)
        {
            EndMatch(1);
        }
        else
        {
            StartCoroutine(RestartGame());
        }
    }

    public void Player2WinsRound()
    {
        GlobalVariables.player2RoundsWon++;
        UpdateUI();

        if (GlobalVariables.player2RoundsWon >= roundsToWin)
        {
            EndMatch(2);
        }
        else
        {
            StartCoroutine(RestartGame());
        }
    }

    private void EndMatch(int winner)
    {
        Debug.Log("Match ended. Winner: Player " + winner);

        if (winner == 1)
        {
            player1WinText.gameObject.SetActive(true);
        }
        else if (winner == 2)
        {
            player2WinText.gameObject.SetActive(true);
        }

        StartCoroutine(DelayedSceneLoad(5f));
    }

    private IEnumerator DelayedSceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(2);
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        UpdateUI();
    }
}
