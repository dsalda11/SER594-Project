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
    private System.Action onContinue;


    private Queue<string> sentences = new Queue<string>();
    private System.Action onYes;
    private System.Action onNo;
    public bool IsDialogueOpen => dialoguePanel != null && dialoguePanel.activeSelf;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    public void StartDialogue(string speaker, List<string> dialogueLines, System.Action yesAction = null, System.Action noAction = null)
    {
        dialoguePanel.SetActive(true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        speakerNameText.text = speaker;
        sentences.Clear();

        foreach (string line in dialogueLines)
            sentences.Enqueue(line);

        onYes = yesAction;
        onNo = noAction;

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            if (onContinue != null)
            {
            continueButton.gameObject.SetActive(true);
            return;
            }
            if (onYes != null || onNo != null)
            {
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
                yesButton.onClick.RemoveAllListeners();
                noButton.onClick.RemoveAllListeners();

                yesButton.onClick.AddListener(() => { EndDialogue(); onYes?.Invoke(); });
                noButton.onClick.AddListener(() => { EndDialogue(); onNo?.Invoke(); });
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }
    public void EndDialogue()
    {
    dialoguePanel.SetActive(false);
    sentences.Clear();
    yesButton.gameObject.SetActive(false);
    noButton.gameObject.SetActive(false);
    }
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        speakerNameText.text = "";
    }

    public void ShowContinueButton(System.Action continueAction)
        {
    continueButton.gameObject.SetActive(true);
    onContinue = continueAction;
    continueButton.onClick.RemoveAllListeners();
    continueButton.onClick.AddListener(() =>
    {
        onContinue?.Invoke();
        EndDialogue();
        continueButton.gameObject.SetActive(false);
    });
    }

}
