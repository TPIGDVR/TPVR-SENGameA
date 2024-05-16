using HelpfulScript;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Pausemenu : UIBase
    {
        [Header("Restart Menu Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;


        [SerializeField] private ToggleRay[] toggleRays;

        public void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
            quitButton.onClick.AddListener(QuitGame);
            gameObject.SetActive(false);
        }

        public override void ToggleUI()
        {
            base.ToggleUI();
            if (gameObject.active)
            {
                //if active
                foreach (var t in toggleRays)
                {
                    t.ActivateRay();
                }
            }
            else
            {
                foreach (var t in toggleRays)
                {
                    t.DeactivateRay();
                }
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void RestartGame()
        {
            DeactivateUI();
            SceneManager.LoadScene("Main VR Scene");
        }
    }
}