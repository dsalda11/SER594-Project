using UnityEngine;

public class RecruitSpawner : MonoBehaviour
{
    public static RecruitSpawner Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject knightPrefab;
    public GameObject archerPrefab;
    public GameObject mysticPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;         
    public Vector2 slideInOffset = new Vector2(-2f, 0f);
    public float slideInTime = 0.35f;

    private GameObject current;          

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    public void SpawnFor(ClassType ct)
    {
        // pick prefab
        GameObject prefab = null;
        switch (ct)
        {
            case ClassType.Knight: prefab = knightPrefab; break;
            case ClassType.Archer: prefab = archerPrefab; break;
            case ClassType.Mystic: prefab = mysticPrefab; break;
            default: return;
        }
        if (prefab == null || spawnPoint == null) { Debug.Log(" missing"); return; }

       
        if (current != null) Destroy(current);

        
        Vector3 target = spawnPoint.position;
        Vector3 start = (Vector3)(spawnPoint.position + (Vector3)slideInOffset);

        current = Instantiate(prefab, start, Quaternion.identity);

        var sr = current.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) { sr.sortingOrder = Mathf.Max(sr.sortingOrder, 5); }

        // slide in
        current.GetComponent<MonoBehaviour>()?.StartCoroutine(SlideIn(current.transform, start, target, slideInTime));
    }

    private System.Collections.IEnumerator SlideIn(Transform t, Vector3 from, Vector3 to, float time)
    {
        if (time <= 0f) { t.position = to; yield break; }
        float elapsed = 0f;
        while (elapsed < time && t != null)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / time);
            t.position = Vector3.Lerp(from, to, k);
            yield return null;
        }
        if (t != null) t.position = to;
    }
}
