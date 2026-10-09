using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;


    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LobbyTutorial_Done");
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    
}
