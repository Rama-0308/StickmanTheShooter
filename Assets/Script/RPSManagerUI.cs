using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RPSManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject choiceButtonsGroup;
    [SerializeField] private Button rockButton;
    [SerializeField] private Button paperButton;
    [SerializeField] private Button scissorsButton;
    [SerializeField] private TextMeshProUGUI chooseText;
    [SerializeField] private GameObject revealGroup;
    [SerializeField] private Image hostRevealImage, clientRevealImage;
    [SerializeField] private Sprite rockSprite, paperSprite, scissorsSprite;
    [SerializeField] private TextMeshProUGUI turnWinnerText;


    private void Awake()
    {
        revealGroup.SetActive(false);
        choiceButtonsGroup.SetActive(false);
    }
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;

        rockButton.onClick.AddListener(() => GameManager.Instance.PlayerChoices(GameManager.PlayerChoice.Rock));

        paperButton.onClick.AddListener(() => GameManager.Instance.PlayerChoices(GameManager.PlayerChoice.Paper));

        scissorsButton.onClick.AddListener(() => GameManager.Instance.PlayerChoices(GameManager.PlayerChoice.Scissors));

        GameManager.Instance.OnRpsResolved += GameManager_OnRpsResolved;

        GameManager.Instance.OnTurnEnd += GameManager_OnTurnEnd;

        GameManager.Instance.OnRematch += GameManager_OnRematch;

        
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        choiceButtonsGroup.SetActive(true);
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        choiceButtonsGroup.SetActive(true);
        revealGroup.SetActive(false);
        turnWinnerText.gameObject.SetActive(false);
    }

    private void GameManager_OnTurnEnd(object sender, System.EventArgs e)
    {
        choiceButtonsGroup.SetActive(true);
        revealGroup.SetActive(false);
        turnWinnerText.gameObject.SetActive(false);
    }

    private void GameManager_OnRpsResolved(object sender, GameManager.OnRpsResolvedEventArgs e)
    {
        choiceButtonsGroup.SetActive(false);
        revealGroup.SetActive(true);
        turnWinnerText.gameObject.SetActive(true);
        hostRevealImage.sprite = GetSprite(e.hostChoice);
        clientRevealImage.sprite = GetSprite(e.clientChoice);
        

        if (e.winner == GameManager.Player.Player1)
        {
            turnWinnerText.text = "Player1 Turn!";
        }
        else if (e.winner == GameManager.Player.Player2)
        {
            turnWinnerText.text = "Player2 Turn!";
        }
        else
        {
            turnWinnerText.text = "Draw!";
        }

        if (e.winner == GameManager.Player.None)
        {
            StartCoroutine(ShowChoicesAfterDelay());

        }
    }

    private IEnumerator ShowChoicesAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        choiceButtonsGroup.SetActive(true);
        revealGroup.SetActive(false);
        turnWinnerText.gameObject.SetActive(false);

    }

  

    private Sprite GetSprite(GameManager.PlayerChoice c) => c switch
    {
        GameManager.PlayerChoice.Rock => rockSprite,
        GameManager.PlayerChoice.Paper => paperSprite,
        GameManager.PlayerChoice.Scissors => scissorsSprite,
        _ => null
    };
};
