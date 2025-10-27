using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Button yesButton;
    public Button noButton;
    public Button continueButton;

    private Queue<string> sentences = new Queue<string>();
    private System.Action onYes;
    private System.Action onNo;
    private System.Action onContinue;

    public bool IsDialogueOpen => dialoguePanel != null && dialoguePanel.activeSelf;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (yesButton != null)
            yesButton.gameObject.SetActive(false);

        if (noButton != null)
            noButton.gameObject.SetActive(false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }

    public void StartDialogue(string speaker, List<string> dialogueLines, System.Action yesAction = null, System.Action noAction = null)
    {
        if (dialoguePanel == null) return;

        dialoguePanel.SetActive(true);
        if (yesButton != null) yesButton.gameObject.SetActive(false);
        if (noButton != null) noButton.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);

        if (speakerNameText != null)
            speakerNameText.text = speaker;

        sentences.Clear();

        foreach (string line in dialogueLines)
            sentences.Enqueue(line);

        onYes = yesAction;
        onNo = noAction;
        onContinue = null;

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
{
    Debug.Log("Dialogue finished — checking which button to show");

    if (onContinue != null && continueButton != null)
    {
        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            Debug.Log("ContinueButton UI click detected");
            onContinue?.Invoke();
            EndDialogue();
        });
        return;
    }
    if (onYes != null || onNo != null)
    {
        if (yesButton != null) yesButton.gameObject.SetActive(true);
        if (noButton != null) noButton.gameObject.SetActive(true);

        yesButton?.onClick.RemoveAllListeners();
        noButton?.onClick.RemoveAllListeners();

        yesButton?.onClick.AddListener(() =>
        {
            Debug.Log("Yes clicked — executing yes action");
            EndDialogue();
            onYes?.Invoke();
        });

        noButton?.onClick.AddListener(() =>
        {
            Debug.Log("No clicked - executing no action");
            EndDialogue();
            onNo?.Invoke();
        });
    }
    else
    {
        EndDialogue();
    }

    return;
}


        string sentence = sentences.Dequeue();
        if (dialogueText != null)
            dialogueText.text = sentence;
    }

    public void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        sentences.Clear();

        if (yesButton != null) yesButton.gameObject.SetActive(false);
        if (noButton != null) noButton.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (speakerNameText != null)
            speakerNameText.text = "";
    }

    public void ShowContinueButton(System.Action continueAction)
    {
        Debug.Log("ShowContinueButton called..");
        onContinue = continueAction;
    }
}
