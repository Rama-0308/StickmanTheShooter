using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthenticateUI : MonoBehaviour {


    [SerializeField] private Button authenticateButton;
    [SerializeField] private Button backToTitleScreen;


    private void Awake() {
        authenticateButton.onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            Hide();
        });

        backToTitleScreen.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("TitleScreen");
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}