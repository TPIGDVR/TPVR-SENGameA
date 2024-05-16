using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button aboutButton;
    public Button quitButton;

    public Button returnButton;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);
        returnButton.onClick.AddListener(EnableMainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        SceneManager.LoadScene("Main VR Scene");

        //SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main VR Scene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        about.SetActive(false);
    }

    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        about.SetActive(true);
    }
}
