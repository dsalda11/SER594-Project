using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("NPC Dialogue Settings")]
    public string speakerName = "Villager";
    [TextArea(3, 10)]
    public List<string> dialogueLines;

    [Header("Optional Quest NPC")]
    public bool isQuestGiver = false;

    private bool playerInRange = false;
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found!");

            return; 
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isQuestGiver)
            {
                dialogueManager.StartDialogue(
                    speakerName,
                    dialogueLines,
                    yesAction: () => Debug.Log("Player accepted quest."),
                    noAction: () => Debug.Log("Player declined quest.")
                );
            }
            else
            {
                dialogueManager.StartDialogue(speakerName, dialogueLines);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
             Debug.Log("Player entered range of " + gameObject.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           playerInRange = false;
        if (dialogueManager != null)
        {
            dialogueManager.HideDialogue();
        }
        }
    }
}
