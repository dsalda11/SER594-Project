using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Data")]
    public string npcName;
    [TextArea(2, 6)] public List<string> dialogueLines;

    [Header("Quest Settings (Optional)")]
    public bool isQuestGiver;
    public bool requiresQuestAccepted;
    public bool questAccepted;

    private bool playerInRange;
    private bool dialogueActive = false;

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            
            if (dialogueActive)
            {
                DialogueManager.Instance.DisplayNextSentence();
                return;
            }

            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        dialogueActive = true;

        if (npcName == "Wizard")
        {
        DialogueManager.Instance.StartDialogue(npcName, dialogueLines);
        DialogueManager.Instance.ShowContinueButton(() =>
        {
            Destroy(gameObject); 
        });
        return;
        }

        if (isQuestGiver && !QuestManager.Instance.IsQuestAccepted())
        {
            DialogueManager.Instance.StartDialogue(
                npcName,
                dialogueLines,
                yesAction: () =>
                {
                    QuestManager.Instance.SetQuestAccepted(true);
                    dialogueActive = false; 
                },
                noAction: () =>
                {
                    dialogueActive = false;
                }
            );
        }
        else if (!isQuestGiver || (requiresQuestAccepted && QuestManager.Instance.IsQuestAccepted()))
        {
            DialogueManager.Instance.StartDialogue(
                npcName,
                dialogueLines
            );

            if (npcName == "Wizard")
            {
            DialogueManager.Instance.ShowContinueButton(() =>
            {
            Destroy(gameObject); // dest wiz
            });
            }

          
            Invoke(nameof(ResetDialogueState), dialogueLines.Count * 1.5f);
        }
        
    }

    void ResetDialogueState()
    {
        dialogueActive = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            dialogueActive = false; 
            DialogueManager.Instance.EndDialogue();
        }
    }
   
    public void OnYesButtonClicked()
    {
    
    if (QuestManager.Instance != null)
    {
        QuestManager.Instance.SetQuestAccepted(true);
    }
    else
    {
        Debug.LogWarning("QuestManager.Instance is null when accepting quest.");
    }

    
    if (DialogueManager.Instance != null)
    {
        DialogueManager.Instance.EndDialogue();
    }

    
    if (SceneTransitionManager.Instance != null)
    {
        SceneTransitionManager.Instance.LoadBattleScene();
        return;
    }

    #if UNITY_EDITOR || true
    try
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }
    catch (System.Exception e)
    {
        Debug.LogError("Failed to load BattleScene. Make sure it's added to Build Settings. Exception: " + e);
    }
    #endif
    }

}
