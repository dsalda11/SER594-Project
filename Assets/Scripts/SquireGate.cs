using UnityEngine;

public class SquireGate : MonoBehaviour
{
    [Tooltip("Set the Squire")]
    public GameObject squire;

    private bool enabledOnce = false;

    void Start()
    {
        if (squire != null) squire.SetActive(false); 
    }

    void Update()
    {
        if (enabledOnce) return;
        if (QuestManager.Instance != null && QuestManager.Instance.IsQuestAccepted())
        {
            if (squire != null) squire.SetActive(true);
            enabledOnce = true;
            enabled = false; // stop updating
        }
    }
}
