using UnityEngine;
using System.Collections.Generic; // List와 Queue를 사용하기 위해 필요합니다.
using System.Collections; // 코루틴을 사용하기 위해 필요합니다.

// 이 스크립트는 플레이어의 총알 발사와 오브젝트 풀링을 관리합니다.
public class PlayerBulletSpawner : MonoBehaviour
{
    // 총알 프리팹 (Inspector에서 할당)
    public GameObject bulletPrefab;

    // 오브젝트 풀링 변수
    public int poolSize = 20; // 초기 총알 풀의 크기
    private Queue<GameObject> bulletPool; // 총알을 저장할 큐 (선입선출 구조)

    void Awake() // Awake는 Start보다 먼저 호출되어 풀을 초기화하기에 좋습니다.
    {
        bulletPool = new Queue<GameObject>();
        // 풀에 미리 총알을 생성하여 채워 넣습니다.
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // 처음에는 비활성화 상태로 둡니다.

            // 총알 스크립트에 이 스포너(풀 관리자) 참조를 설정합니다.
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetSpawner(this);
            }
            // 모든 플레이어 총알의 태그를 "Bullet"으로 설정
            bullet.tag = "Bullet";
            bulletPool.Enqueue(bullet); // 풀에 추가합니다.
        }
    }

    // PlayerAttack 스크립트에서 호출될 총알 발사 함수
    public void Shoot(Vector3 position, Quaternion rotation, Vector2 movementDirection, float speed, int baseDamage, float damageMultiplier, bool isInstantKillBullet)
    {
        // 풀에서 총알을 가져옵니다.
        GameObject bulletToFire = GetBulletFromPool();

        if (bulletToFire != null)
        {
            // 총알의 위치를 발사 지점으로 설정합니다.
            bulletToFire.transform.position = position;

            // 총알 스프라이트가 기본적으로 위를 보고 있다면, 오른쪽으로 향하게 -90도 Z축 회전 적용
            bulletToFire.transform.rotation = Quaternion.Euler(0, 0, -90);

            // 총알 스크립트의 속도와 데미지/즉사 여부를 설정하고 활성화합니다.
            Bullet bulletScript = bulletToFire.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.speed = speed; // PlayerAttack에서 전달받은 속도 사용
                bulletScript.isInstantKillBullet = isInstantKillBullet; // 즉사 총알 여부 설정

                // 즉사 총알이 아닐 경우에만 데미지 계산 및 설정
                if (!isInstantKillBullet)
                {
                    // baseDamage에 damageMultiplier를 곱하여 최종 데미지를 계산합니다.
                    int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
                    bulletScript.damageAmount = finalDamage; // 계산된 최종 데미지를 총알에 적용합니다.
                }
                // 즉사 총알일 경우 damageAmount는 사용되지 않으므로 따로 설정할 필요 없음

                bulletScript.OnSpawned(); // 총알 활성화 시 필요한 초기화 로직 호출
            }

            // 총알에 Rigidbody2D가 있다면 속도를 설정합니다.
            Rigidbody2D rb = bulletToFire.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 전달받은 이동 방향 벡터에 속도를 곱하여 총알을 이동시킵니다.
                rb.linearVelocity = movementDirection * speed;
            }
            else
            {
                Debug.LogWarning("경고: 생성된 총알 프리팹에 'Rigidbody2D' 컴포넌트가 없습니다. 총알이 움직이지 않을 수 있습니다.");
            }

            // 총알의 수명은 여기서 관리합니다 (예: 3초 후 비활성화)
            StartCoroutine(DeactivateBulletAfterTime(bulletToFire, 3f));
        }
        else
        {
            Debug.LogWarning("경고: 총알 풀이 비어있습니다! 총알을 발사할 수 없습니다. poolSize를 늘리거나 동적 풀링을 고려하세요.");
        }
    }

    // 풀에서 총알 하나를 가져오는 메서드입니다.
    private GameObject GetBulletFromPool()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue(); // 큐에서 총알을 꺼냅니다.
            return bullet;
        }
        return null; // 풀이 비어있으면 null을 반환합니다.
    }

    // 총알을 풀로 돌려보내는 메서드입니다.
    public void ReturnBulletToPool(GameObject bullet)
    {
        // 총알 스크립트의 비활성화 로직을 호출합니다.
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.OnDespawned(); // 총알 비활성화 시 필요한 정리 로직 호출
        }
        bulletPool.Enqueue(bullet); // 풀에 총알을 다시 넣습니다.
    }

    // 일정 시간 후 총알을 비활성화하여 풀로 돌려보내는 코루틴입니다.
    private IEnumerator DeactivateBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        // 총알이 이미 다른 이유(예: 적과 충돌)로 인해 비활성화되지 않았다면 풀로 돌려보냅니다.
        if (bullet != null && bullet.activeSelf) // null 체크 추가
        {
            ReturnBulletToPool(bullet);
        }
    }
}
