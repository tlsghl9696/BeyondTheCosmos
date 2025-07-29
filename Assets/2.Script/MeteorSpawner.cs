using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject[] meteorPrefabs;   // 여러 종류의 운석 프리팹 배열
    public float spawnInterval = 1f;
    public float spawnYMin = -4f;
    public float spawnYMax = 4f;
    public float meteorSpeed = -3f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnMeteor();
            timer = 0f;

        }
    }

    void SpawnMeteor()
    {
        if (meteorPrefabs.Length == 0) return;

        GameObject prefabToSpawn = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];

        // 화면 오른쪽 끝의 월드 좌표 계산
        float rightEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 1f;
        float spawnY = Random.Range(spawnYMin, spawnYMax);
        Vector3 spawnPos = new Vector3(rightEdgeX, spawnY, 0f);

        GameObject meteor = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 운석에 "Meteor" 태그를 명시적으로 설정
        meteor.tag = "Meteor";

        Rigidbody2D rb = meteor.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(meteorSpeed, 0f);
        }
    }
}
