using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 총알 발사 관련 변수들은 PlayerBulletSpawner로 이동했습니다.
    // public GameObject projectilePrefab;
    // public Transform firePoint;
    // public float fireRate = 0.2f;
    // private float nextFireTime = 0f;

    public float speed = 10.0f;         // 이동 속도
    public float yRange = 4f;           // 상하 이동 제한 범위

    void Update()
    {
        // 자동 발사 로직은 PlayerBulletSpawner에서 처리됩니다.
        // if (Time.time >= nextFireTime)
        // {
        //     Fire(); // 총알 생성
        //     nextFireTime = Time.time + fireRate; // 다음 발사 시각 설정
        // }

        // 상하 이동
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * vertical * Time.deltaTime * speed); // 위/아래로 이동

        // 이동 제한
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -yRange, yRange); // Y축 이동 제한
        transform.position = pos;
    }

    // 총알 발사 함수는 PlayerBulletSpawner로 이동했습니다.
    // void Fire()
    // {
    //     Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    // }
}