using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject titleScreen;
    public GameObject UIScreen;
    public GameObject helpScreen;

    private GameObject incognitoText;
    private GameObject exposedText;
    private GameObject runText;
    private bool isGameActive = false;
    private int victims;
    private int status;

    // Start is called before the first frame update
    void Start()
    {
        titleScreen.SetActive(true);
        UIScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        incognitoText = UIScreen.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(1).gameObject;
        exposedText = UIScreen.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(2).gameObject;
        runText = UIScreen.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case 0:
                {
                    incognitoText.SetActive(true);
                    exposedText.SetActive(false);
                    runText.SetActive(false);
                    break;
                }
            case 1:
                {
                    incognitoText.SetActive(false);
                    exposedText.SetActive(true);
                    runText.SetActive(false);
                    break;
                }
            case 2:
                {
                    incognitoText.SetActive(false);
                    exposedText.SetActive(false);
                    runText.SetActive(true);
                    break;
                }
            default:
                {
                    incognitoText.SetActive(true);
                    exposedText.SetActive(false);
                    runText.SetActive(false);
                    break;
                }
        }
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void StartGame()
    {
        isGameActive = true;
        titleScreen.SetActive(false);
        UIScreen.SetActive(true);
    }

    public void ShowHelpScreen()
    {
        Time.timeScale = 0;
        helpScreen.SetActive(true);
        titleScreen.SetActive(false);
        UIScreen.SetActive(false);
    }

    public void ResumeGame()
    {
        helpScreen.SetActive(false);
        if (isGameActive)
        {
            UIScreen.SetActive(true);
        } else
        {
            titleScreen.SetActive(true);
        }
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        UIScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetPlayerStatus(int status)
    {
        this.status = status;
    }
}
