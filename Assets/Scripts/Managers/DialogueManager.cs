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

    [Header("Class Choice UI")]
    public GameObject choicePanel;   // ChoicePanel
    public Button knightButton;
    public Button archerButton;
    public Button mysticButton;
    private System.Action onFinished;

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
        Debug.Log($"[DM] Instance set on: {gameObject.name} (scene: {gameObject.scene.name})");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (yesButton != null)
            yesButton.gameObject.SetActive(false);

        if (noButton != null)
            noButton.gameObject.SetActive(false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);

    }

    public void StartDialogue(
        string speaker,
        List<string> dialogueLines,
        System.Action yesAction = null,
        System.Action noAction = null,
        System.Action finishedAction = null)   
    {
        if (dialoguePanel == null) return;

        dialoguePanel.SetActive(true);
        if (yesButton != null) yesButton.gameObject.SetActive(false);
        if (noButton != null) noButton.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false); // make sure

        if (speakerNameText != null) speakerNameText.text = speaker;

        sentences.Clear();
        foreach (string line in dialogueLines) sentences.Enqueue(line);

        onYes = null;
        onNo = null;
        onContinue = null;
        onFinished = null;

        onYes = yesAction;
        onNo = noAction;
        onFinished = finishedAction; 
        Debug.Log($"[DM] StartDialogue called for {speaker}, finishedAction={(finishedAction != null)}");

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
                Debug.Log($"[DM] onYes={onYes != null}, onNo={onNo != null}, onFinished={onFinished != null}");
                if (onFinished != null)
                {
                    var f = onFinished;
                    onFinished = null;
                    f.Invoke();
                }
                else
                {
                    EndDialogue();
                }
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

    public void ShowClassChoice(string recruitName)
    {
        Debug.Log("CALLED for " + recruitName);
        if (choicePanel == null || knightButton == null || archerButton == null || mysticButton == null)
        {
            Debug.Log("Class choice UI not set up on DialogueManager.");
            Debug.LogError("[DM] Class choice UI not set: " +
                       $"choicePanel={(choicePanel != null)}, " +
                       $"knight={(knightButton != null)}, archer={(archerButton != null)}, mystic={(mysticButton != null)}");
            return;
        }

        choicePanel.SetActive(true);
        choicePanel.transform.SetAsLastSibling(); // ensures it renders on top


        knightButton.onClick.RemoveAllListeners();
        archerButton.onClick.RemoveAllListeners();
        mysticButton.onClick.RemoveAllListeners();

        knightButton.onClick.AddListener(() =>
        {
            PartyState.Instance?.SetSquireClass(ClassType.Knight);
            ConfirmRecruit(recruitName, ClassType.Knight);
        });

        archerButton.onClick.AddListener(() =>
        {
            PartyState.Instance?.SetSquireClass(ClassType.Archer);
            ConfirmRecruit(recruitName, ClassType.Archer);
        });

        mysticButton.onClick.AddListener(() =>
        {
            PartyState.Instance?.SetSquireClass(ClassType.Mystic);
            ConfirmRecruit(recruitName, ClassType.Mystic);
        });
        Debug.Log("[DM] ChoicePanel active: " + choicePanel.activeSelf);

    }

    private void ConfirmRecruit(string recruitName, ClassType ct)
    {
        if (choicePanel != null) choicePanel.SetActive(false);

        string className = ct.ToString();
        int hp = PartyState.Instance ? PartyState.Instance.bonusHP : 0;
        int atk = PartyState.Instance ? PartyState.Instance.bonusATK : 0;
        int spd = PartyState.Instance ? PartyState.Instance.bonusSPD : 0;
        if (RecruitSpawner.Instance != null)
            RecruitSpawner.Instance.SpawnFor(ct);
        if (speakerNameText != null) speakerNameText.text = recruitName;
        // if (dialogueText != null)
        //     dialogueText.text = $"Recruited {recruitName} as {className}!\n" +
        //                         $"+{hp} HP, +{atk} ATK, +{spd} SPD";

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                continueButton.gameObject.SetActive(false);
                EndDialogue();

                // ⬇️ Move to the tutorial/battle scene only AFTER recruit is complete
                if (SceneTransitionManager.Instance != null)
                    SceneTransitionManager.Instance.LoadBattleScene();
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
            });
        }
    }

}
