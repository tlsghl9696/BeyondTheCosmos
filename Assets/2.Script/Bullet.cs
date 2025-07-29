using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요합니다.

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // 총알 속도
    public float destroyDelay = 3f; // 총알 자동 파괴 시간

    // PlayerBulletSpawner에서 설정할 플레이어 총알의 데미지 양
    public int damageAmount = 10;
    // PlayerBulletSpawner에서 설정할 즉사 총알 여부
    public bool isInstantKillBullet = false;

    // 이 총알이 플레이어 총알인지 적 총알인지 구분하는 플래그
    public bool isPlayerBullet = true;

    // 이 총알을 생성한 PlayerBulletSpawner 참조 (오브젝트 풀 반환용)
    public PlayerBulletSpawner spawner;

    void OnEnable() // 오브젝트 풀에서 활성화될 때 호출됩니다.
    {
        // 총알이 다시 활성화될 때마다 초기 상태로 설정합니다.
        // 예를 들어, Rigidbody2D가 있다면 속도를 초기화하거나,
        // ParticleSystem이 있다면 재생을 시작할 수 있습니다.
    }

    void OnDisable() // 오브젝트 풀로 돌아가 비활성화될 때 호출됩니다.
    {
        // 총알이 비활성화될 때 필요한 정리 작업을 수행합니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // 속도 초기화
            rb.angularVelocity = 0f; // 회전 속도 초기화
        }
    }

    // PlayerBulletSpawner에서 이 스크립트 참조를 설정하는 메서드
    public void SetSpawner(PlayerBulletSpawner playerBulletSpawner)
    {
        spawner = playerBulletSpawner;
    }

    // PlayerBulletSpawner에서 총알 활성화 시 호출하는 초기화 메서드
    public void OnSpawned()
    {
        gameObject.SetActive(true); // 총알 활성화
        // 필요한 경우 여기에 추가 초기화 로직을 넣을 수 있습니다.
    }

    // PlayerBulletSpawner에서 총알 비활성화 시 호출하는 정리 메서드
    public void OnDespawned()
    {
        gameObject.SetActive(false); // 총알 비활성화 (풀로 돌아감)
        // 필요한 경우 여기에 추가 정리 로직을 넣을 수 있습니다.
    }

    void Update()
    {
        // Rigidbody2D를 사용한다면 이 부분은 필요 없을 수 있습니다.
        // PlayerBulletSpawner에서 이미 속도를 설정합니다.
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 총알일 경우 (태그: Bullet)
        if (isPlayerBullet)
        {
            // 운석 또는 보스에 충돌했을 때 (이제 'Enemy' 태그는 사용하지 않습니다)
            if (other.CompareTag("Meteor") || other.CompareTag("Boss"))
            {
                // 맞은 오브젝트(MeteorHP 또는 BossHP)가 데미지 처리 및 점수 추가를 담당합니다.
                // 총알은 오브젝트 풀로 반환됩니다.
                if (spawner != null)
                {
                    spawner.ReturnBulletToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject); // 스포너가 없으면 그냥 파괴
                }
            }
        }
        // 적 총알일 경우 (태그: EnemyBullet)
        else // !isPlayerBullet
        {
            if (other.CompareTag("Player"))
            {
                // PlayerHealth.cs에서 데미지 처리 (EnemyBullet 태그로 처리)
                // 총알은 오브젝트 풀로 반환됩니다.
                if (spawner != null)
                {
                    spawner.ReturnBulletToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject); // 스포너가 없으면 그냥 파괴
                }
            }
        }
    }
}
