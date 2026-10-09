using UnityEngine;
using UnityEngine.UI;

public class LobbyJoinByCode : MonoBehaviour
{
    [SerializeField] private Button joinByCodeButton;

    private void Awake()
    {
        joinByCodeButton.onClick.AddListener(() =>
        {
            UI_InputWindow.Show_Static(
                "Join Private Lobby",
                "",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                6,
                onCancel: () => { },
                onOk: (code) =>
                {
                    LobbyManager.Instance.JoinLobbyByCode(code.ToUpper());
                }
            );
        });
    }
}
