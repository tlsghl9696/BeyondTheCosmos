using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가

public class BossBulletPattern : MonoBehaviour
{
    public GameObject bulletPrefab;     // 보스 탄환 프리팹 (Inspector에서 할당)
    public Transform firePoint;         // 탄환 발사 위치 (Inspector에서 할당)
    public GameObject playerTarget;     // 플레이어 오브젝트 (Inspector에서 할당)

    [Header("패턴 설정: 부채꼴")]
    public int spreadBulletCount = 5;      // 부채꼴: 한 번에 발사할 탄환 수 (기존 7에서 5로 감소)
    public float spreadAngle = 60f;        // 부채꼴: 전체 퍼지는 각도
    public float spreadFireInterval = 2.5f; // 부채꼴: 발사 간격 (기존 2초에서 2.5초로 증가)
    public float spreadPatternRotationSpeed = 30f; // 부채꼴 패턴의 초당 회전 속도

    // [추가된 부분 시작] 부채꼴 패턴 세부 설정
    public float minSpreadRotationAngle = 5f; // 회피 기동 유도를 위한 최소 회전 각도
    public float maxSpreadRotationAngle = 15f; // 회피 기동 유도를 위한 최대 회전 각도
    public float spreadBulletDistanceMultiplier = 1.2f; // 탄환 간 거리 배율 (1.0f 이상)
    // [추가된 부분 끝]

    [Header("패턴 설정: 조준")]
    public int aimBurstBulletCount = 5;     // 조준: 한 번에 쏠 총알 수 (버스트)
    public float aimBulletDelay = 0.7f;    // 조준: 버스트 내 총알 발사 간격 (기존 0.2초에서 0.7초로 변경)
    public float aimFireInterval = 3f;     // 조준: 버스트 발사 간격 (기존 1.5초에서 3초로 증가)
    public float fakeBulletChance = 0.5f;  // 조준: 페이크 총알 발사 확률 (0~1, 기존 0.2f에서 0.5f로 변경)

    public float bulletSpeed = 5f;      // 탄속 (공통)
    public float initialDelay = 1.5f;   // 보스 등장 후 첫 공격까지의 딜레이

    private float timer;
    private bool hasStartedFiring = false; // 첫 공격 딜레이를 위한 플래그
    private float currentSpreadRotation = 0f; // 부채꼴 패턴의 현재 회전 값

    private enum BossPattern { Spread, Aiming } // 보스 패턴 열거형
    private BossPattern currentPattern = BossPattern.Spread; // 현재 보스 패턴

    void Start()
    {
        // 플레이어 타겟을 자동으로 찾습니다. (태그: Player)
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player;
            }
            else
            {
                Debug.LogWarning("[BossBulletPattern] 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다. 조준 패턴이 제대로 작동하지 않을 수 있습니다. Inspector에서 Player Target을 수동으로 할당하는 것이 좋습니다.");
            }
        }

        // 보스 등장 후 첫 공격까지 딜레이 코루틴 시작
        StartCoroutine(StartFiringAfterDelay());
    }

    IEnumerator StartFiringAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        hasStartedFiring = true;
        Debug.Log("[BossBulletPattern] 보스 공격 시작!");
    }

    void Update()
    {
        if (!hasStartedFiring) return; // 아직 공격 시작 딜레이 중이면 리턴

        timer += Time.deltaTime;
        currentSpreadRotation += spreadPatternRotationSpeed * Time.deltaTime; // 부채꼴 패턴 계속 회전

        // 패턴 전환 로직
        if (currentPattern == BossPattern.Spread)
        {
            if (timer >= spreadFireInterval)
            {
                // [수정된 부분] 랜덤하게 두 가지 부채꼴 패턴 중 하나 선택
                int patternType = Random.Range(0, 2); // 0 또는 1
                if (patternType == 0)
                {
                    FireSpreadDefault(); // 기본 부채꼴
                }
                else
                {
                    FireSpreadRotating(); // 회전 부채꼴 (회피 유도)
                }

                timer = 0f;
                currentPattern = BossPattern.Aiming; // 다음 패턴으로 전환
                Debug.Log("[BossBulletPattern] 패턴 전환: 조준 패턴");
            }
        }
        else if (currentPattern == BossPattern.Aiming)
        {
            if (timer >= aimFireInterval)
            {
                StartCoroutine(FireAimingBurst()); // 조준 버스트 패턴 시작
                timer = 0f;
                currentPattern = BossPattern.Spread; // 다음 패턴으로 전환
                Debug.Log("[BossBulletPattern] 패턴 전환: 부채꼴 패턴");
            }
        }
    }

    // [기존 FireSpread 함수를 FireSpreadDefault로 이름 변경]
    // 부채꼴 패턴 발사 (기본)
    void FireSpreadDefault()
    {
        // ← 방향을 중심으로 퍼지게 하기 (보스가 오른쪽에 있기 때문)
        float baseStartAngle = 180f - spreadAngle / 2f;
        float angleStep = spreadAngle / (spreadBulletCount - 1) * spreadBulletDistanceMultiplier; // 탄환 간 거리 넓힘

        for (int i = 0; i < spreadBulletCount; i++)
        {
            float angle = baseStartAngle + angleStep * i; // 여기서는 고정된 형태로 발사
            float rad = angle * Mathf.Deg2Rad;

            // 발사 방향 계산
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            SpawnBullet(direction); // 총알 생성 및 발사
        }
        Debug.Log("[BossBulletPattern] 부채꼴 패턴 (기본) 발사!");
    }

    // [새로 추가된 함수] 부채꼴 패턴 발사 (회전하며 회피 유도)
    void FireSpreadRotating()
    {
        float actualRotation = Random.Range(minSpreadRotationAngle, maxSpreadRotationAngle);
        float baseStartAngle = 180f - spreadAngle / 2f;
        float angleStep = spreadAngle / (spreadBulletCount - 1) * spreadBulletDistanceMultiplier; // 탄환 간 거리 넓힘

        for (int i = 0; i < spreadBulletCount; i++)
        {
            float angle = baseStartAngle + angleStep * i + actualRotation; // 랜덤 회전값 적용
            float rad = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            SpawnBullet(direction);
        }
        Debug.Log("[BossBulletPattern] 부채꼴 패턴 (회전형) 발사! 회전 각도: " + actualRotation);
    }

    // 플레이어를 조준하여 한 발씩 끊어서 발사하는 버스트 패턴
    IEnumerator FireAimingBurst()
    {
        if (playerTarget == null)
        {
            Debug.LogWarning("[BossBulletPattern] 플레이어 타겟이 없어 조준 패턴 발사 불가.");
            yield break; // 코루틴 종료
        }

        for (int i = 0; i < aimBurstBulletCount; i++)
        {
            Vector2 playerPosition = playerTarget.transform.position; // 플레이어 현재 좌표
            Vector2 firePointPosition = firePoint.position;
            Vector2 playerDirection = (playerPosition - firePointPosition).normalized;

            // 페이크 총알 발사 여부 결정 (확률에 따라)
            // [수정된 부분] 50% 확률로 변경 (fakeBulletChance = 0.5f)
            bool isFake = (Random.value < fakeBulletChance);

            Vector2 finalDirection = playerDirection;
            if (isFake)
            {
                // 플레이어 좌표에 인접하나 가만히 있으면 안 맞는 페이크 탄환 발사
                // 플레이어의 현재 위치에서 좌우로 살짝 오프셋을 주어 목표 지점 설정
                // 플레이어는 상하로만 움직이므로, 좌우 오프셋을 주면 가만히 있을 때 안 맞도록 유도 가능
                float sideOffsetAmount = 0.5f; // 플레이어 기준 좌우로 0.5 유닛 오프셋 (조절 가능)
                float offsetX = Random.Range(0, 2) == 0 ? -sideOffsetAmount : sideOffsetAmount; // 좌 또는 우

                // 플레이어의 현재 X 좌표에 오프셋을 더하여 페이크 목표 지점 설정
                Vector2 fakeTargetPosition = new Vector2(playerPosition.x + offsetX, playerPosition.y);
                finalDirection = (fakeTargetPosition - firePointPosition).normalized;

                Debug.Log("[BossBulletPattern] 페이크 조준 탄환 발사! 목표 X 오프셋: " + offsetX);
            }
            else
            {
                Debug.Log("[BossBulletPattern] 실제 조준 탄환 발사!");
            }
            SpawnBullet(finalDirection); // 총알 생성 및 발사

            if (i < aimBurstBulletCount - 1) // 마지막 총알이 아니면 딜레이 적용
            {
                // [수정된 부분] 0.7초의 텀을 두고 발사
                yield return new WaitForSeconds(aimBulletDelay);
            }
        }
    }

    // 실제 총알을 생성하고 속성(태그, 방향, 속도)을 설정하는 공통 메서드
    void SpawnBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 보스 총알의 태그를 "EnemyBullet"으로 설정 (태그 통일)
        bullet.tag = "EnemyBullet";

        // Bullet 스크립트가 있다면 isPlayerBullet을 false로 설정 (적 총알임을 명시)
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.isPlayerBullet = false;
            // 보스 총알의 데미지는 PlayerHealth.cs에서 고정 데미지로 처리하거나,
            // 여기에 `bulletScript.damageAmount = 특정_데미지;`를 추가할 수 있습니다.
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        // 탄환이 날아가는 방향으로 회전 설정 (시각적 일관성)
        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
    }
}