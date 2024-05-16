using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    public GameObject restartMenu;

    [Header("Restart Menu Buttons")]
    public Button restartButton;
    public Button quitButton;

    public void Start()
    {
        print("running");
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        restartMenu.SetActive(false);
        SceneManager.LoadScene("Main VR Scene");
    }
}
