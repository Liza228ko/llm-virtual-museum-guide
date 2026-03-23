using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public Button submitButton;
    public TMP_Text feedbackLabel;

    [Header("Refs")]
    public GeminiController geminiController;

    [Header("UI Components")]
    // This looks for the component shown in your screenshot
    [SerializeField] private TextMeshProUGUI inputDisplay; 


    void Awake()
    {
        if (submitButton) submitButton.onClick.AddListener(OnSubmit);
        if (inputField) inputField.onSubmit.AddListener(_ => OnSubmit());
    }

    private void OnSubmit()
    {
        Debug.Log("UIManager OnSubmit called!");
        string query = inputField ? inputField.text : null;
        if (!string.IsNullOrEmpty(query) && geminiController)
        {
            geminiController.OnTextInput(query);
        }
        else
        {
            if (geminiController == null)
            {
                Debug.LogError("GeminiController is NOT assigned on the UIManager in the Inspector!");
            }
            else
            {
                Debug.LogWarning("Query is empty, not sending.");
            }
        }
    }


    public void SetUserQuery(string text)
    {
        if (inputDisplay != null)
        {
            inputDisplay.text = text;
        }
        else 
        {
            Debug.LogWarning("Input Display Text is not assigned in UIManager!");
        }
    }

    public void SetFeedback(string msg)
    {
        if (feedbackLabel) feedbackLabel.text = msg;
    }
}
