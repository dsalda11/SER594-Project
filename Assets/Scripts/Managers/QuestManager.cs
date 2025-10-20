using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    private bool questAccepted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetQuestAccepted(bool accepted)
    {
        questAccepted = accepted;
    }

    public bool IsQuestAccepted()
    {
        return questAccepted;
    }
}
