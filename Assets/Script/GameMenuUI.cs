using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button optionsButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button exitMatchButton;
    [SerializeField] private GameObject confirmExitPanel;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmCancelButton;

    private void Awake()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        confirmExitPanel.SetActive(false);

        menuButton.onClick.AddListener(() => menuPanel.SetActive(true));
        optionsButton.onClick.AddListener(() => optionsPanel.SetActive(true));
        exitMatchButton.onClick.AddListener(() => confirmExitPanel.SetActive(true));

        confirmCancelButton.onClick.AddListener(() => confirmExitPanel.SetActive(false));
        confirmYesButton.onClick.AddListener(() => GameManager.Instance.LeaveMatch());
    }
}
