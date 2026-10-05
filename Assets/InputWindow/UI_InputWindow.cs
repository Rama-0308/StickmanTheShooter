using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_InputWindow : MonoBehaviour
{
    private static UI_InputWindow instance;

    private Button okBtn;
    private Button cancelBtn;
    private TextMeshProUGUI titleText;
    private TMP_InputField inputField;

    private Action onCancelCallback;
    private Action<string> onOkCallback;

    private void Awake()
    {
        instance = this;

        okBtn = transform.Find("okBtn").GetComponent<Button>();
        cancelBtn = transform.Find("cancelBtn").GetComponent<Button>();
        titleText = transform.Find("titleText").GetComponent<TextMeshProUGUI>();
        inputField = transform.Find("inputField").GetComponent<TMP_InputField>();

        okBtn.onClick.AddListener(() =>
        {
            Hide();
            onOkCallback?.Invoke(inputField.text);
        });

        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
            onCancelCallback?.Invoke();
        });

        Hide();
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            okBtn.onClick.Invoke();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            cancelBtn.onClick.Invoke();
        }
    }

    private void Show(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        titleText.text = titleString;

        inputField.characterLimit = characterLimit;
        inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar(validCharacters, addedChar);
        };

        inputField.text = inputString;
        inputField.Select();

        onCancelCallback = onCancel;
        onOkCallback = onOk;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private char ValidateChar(string validCharacters, char addedChar)
    {
        return validCharacters.IndexOf(addedChar) != -1 ? addedChar : '\0';
    }

    public static void Show_Static(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk)
    {
        instance.Show(titleString, inputString, validCharacters, characterLimit, onCancel, onOk);
    }

    public static void Show_Static(string titleString, int defaultInt, Action onCancel, Action<int> onOk)
    {
        instance.Show(titleString, defaultInt.ToString(), "0123456789-", 20, onCancel,
            (string inputText) =>
            {
                if (int.TryParse(inputText, out int result))
                    onOk(result);
                else
                    onOk(defaultInt);
            }
        );
    }
}

