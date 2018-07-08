using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private Text scorePlayer1, scorePlayer2, scorePlayer3, scorePlayer4;
    private GameObject panel;
    public static MenuController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        scorePlayer1 = GameObject.Find("Canvas/Panel/Scores/txtPlayer1").GetComponent<Text>();
        scorePlayer2 = GameObject.Find("Canvas/Panel/Scores/txtPlayer2").GetComponent<Text>();
        scorePlayer3 = GameObject.Find("Canvas/Panel/Scores/txtPlayer3").GetComponent<Text>();
        scorePlayer4 = GameObject.Find("Canvas/Panel/Scores/txtPlayer4").GetComponent<Text>();
        panel = GameObject.Find("Canvas/Panel");
        panel.SetActive(false);
    }

    public void PauseGame()
    {
        panel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        panel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ResetGame()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetScore(int player1, int player2, int player3, int player4)
    {
        scorePlayer1.text = "Player 1: /n " + player1;
        scorePlayer2.text = "Player 2: /n " + player2;
        scorePlayer3.text = "Player 3: /n " + player3;
        scorePlayer4.text = "Player 4: /n " + player4;
    }
}
