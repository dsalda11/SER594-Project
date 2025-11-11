using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Tooltip("Name of the battle scene. Make sure this scene is added to Build Settings.")]
    public string battleSceneName = "BattleScene";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadBattleScene()
    {
        StartCoroutine(LoadBattleCoroutine());
    }

    IEnumerator LoadBattleCoroutine()
    {

        AsyncOperation op = SceneManager.LoadSceneAsync(battleSceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;


        YieldInstruction wait = new WaitForEndOfFrame();
        yield return wait;

        if (PersistentPlayer.Instance != null)
        {

            GameObject spawn = GameObject.Find("PlayerSpawn");
            if (spawn != null)
            {
                PersistentPlayer.Instance.transform.position = spawn.transform.position;

                var rb = PersistentPlayer.Instance.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
            else
            {
                Debug.Log("PlayerSpawn not found");
            }
        }
        else
        {

            Debug.LogWarning("PersistentPlayer not found.");
        }
        Debug.Log("Loading battle");
    }
    
    
}
