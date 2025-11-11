using UnityEngine;

public enum ClassType { None, Knight, Archer, Mystic }

public class PartyState : MonoBehaviour
{
    public static PartyState Instance { get; private set; }

    [Header("Current Recruit")]
    public ClassType squireClass = ClassType.None;

    [Header("Tiny Demo Bonuses")]
    public int bonusHP;
    public int bonusATK;
    public int bonusSPD;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void SetSquireClass(ClassType c)
    {
        squireClass = c;
        switch (c)
        {
            case ClassType.Knight: bonusHP = 10; bonusATK = 2; bonusSPD = 0; break;
            case ClassType.Archer: bonusHP = 5;  bonusATK = 1; bonusSPD = 2; break;
            case ClassType.Mystic: bonusHP = 4;  bonusATK = 0; bonusSPD = 1; break;
            default: bonusHP = bonusATK = bonusSPD = 0; break;
        }
    }
}
