using UnityEngine;

public class RandomObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float y;
    [SerializeField] private float radius;
    [SerializeField] private int count;
    [SerializeField] private GameObject prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 p = Random.insideUnitCircle * radius;
            GameObject.Instantiate(prefab, new Vector3(p.x, y, p.y), Quaternion.identity);
        }
    }
}
