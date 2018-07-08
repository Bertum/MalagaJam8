using UnityEngine;

public class WoodGenerator : MonoBehaviour
{
    private int randomTimeToGenerate;
    private float timer;
    private GameObject woodPrefab;

    private void Awake()
    {
        woodPrefab = Resources.Load<GameObject>("Prefabs/Wood");
        randomTimeToGenerate = Random.Range(2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= randomTimeToGenerate)
        {
            Instantiate(woodPrefab, this.transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
