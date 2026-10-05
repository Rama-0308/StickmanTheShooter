using TMPro;
using UnityEngine;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;

    private void Awake()
    {
        leftText.gameObject.SetActive(false);
        rightText.gameObject.SetActive(false);
    }

    void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;

    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);

        if (GameManager.Instance.GetLocalPlayer() == GameManager.Player.Player1)
        {
            leftText.text = "You";
            rightText.text = "Opponent";
            Debug.Log(GameManager.Instance.GetLocalPlayer());
        }
        else
        {
            rightText.text = "You";
            leftText.text = "Opponent";
        }
    }
}
