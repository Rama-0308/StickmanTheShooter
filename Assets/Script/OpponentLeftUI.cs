using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public class OpponentLeftUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Start()
    {
        panel.SetActive(false);
        GameManager.Instance.OnOpponentLeft += GameManager_OnOpponentLeft;
    }

    private void GameManager_OnOpponentLeft(object sender, EventArgs e)
    {
        panel.SetActive(true);
        messageText.text = "Opponent left!";
        StartCoroutine(ReturnToTitleAfterDelay());
    }

    private IEnumerator ReturnToTitleAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("TitleScreen");
    }
}
