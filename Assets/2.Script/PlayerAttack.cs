using UnityEngine;
using System.Collections; // For Coroutines

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;     // 총알 프리팹 (PlayerBulletSpawner에서 사용)
    public Transform firePoint;         // 총알이 발사될 위치

    public float fireRate = 0.25f;      // 총알 발사 속도 (예: 0.25f = 초당 4발)
    public float bulletSpeed = 10f;     // 총알 속도
    public int baseDamage = 10;         // 기본 데미지

    public float damageMultiplier = 1f; // 데미지 배율 (아이템 획득 등으로 이 값을 변경할 수 있음)

    private float fireTimer = 0f;

    // 즉사 모드 관련 변수
    public bool isInstantKillActive = false; // 즉사 모드 활성화 여부
    public float instantKillDuration = 5f; // 즉사 모드 지속 시간 (예: 5초)
    private Coroutine instantKillCoroutine; // 즉사 모드 코루틴 참조

    private PlayerBulletSpawner bulletSpawner; // PlayerBulletSpawner 참조

    void Start()
    {
        // PlayerBulletSpawner 컴포넌트를 찾아서 참조합니다.
        // FindObjectOfType 대신 FindFirstObjectByType 사용
        bulletSpawner = Object.FindFirstObjectByType<PlayerBulletSpawner>();
        if (bulletSpawner == null)
        {
            Debug.LogError("[PlayerAttack] PlayerBulletSpawner를 찾을 수 없습니다. 총알 발사가 불가능합니다.");
        }
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        // 일정 시간이 지나면 총알 발사 (자동 발사)
        if (fireTimer >= fireRate)
        {
            Fire();
            fireTimer = 0f; // 타이머 초기화
        }
    }

    private void Fire()
    {
        if (bulletSpawner == null)
        {
            Debug.LogWarning("[PlayerAttack] PlayerBulletSpawner가 없어 총알을 발사할 수 없습니다.");
            return;
        }

        // 총알이 '좌에서 우로' 발사되도록 이동 방향을 Vector2.right (오른쪽)으로 고정합니다.
        Vector2 bulletMovementDirection = Vector2.right;

        // PlayerBulletSpawner의 Shoot 메서드를 호출하여 총알을 발사합니다.
        // 이때 firePoint의 위치, 회전, 총알 이동 방향, 속도, 기본 데미지, 데미지 배율, 즉사 여부를 전달합니다.
        bulletSpawner.Shoot(firePoint.position, firePoint.rotation, bulletMovementDirection, bulletSpeed, baseDamage, damageMultiplier, isInstantKillActive);
    }

    // 외부에서 데미지 배율을 변경할 수 있는 공개 메서드 (아이템 획득 시 호출)
    public void SetDamageMultiplier(float newMultiplier)
    {
        damageMultiplier = newMultiplier;
        Debug.Log($"[PlayerAttack] 데미지 배율이 {damageMultiplier}로 변경되었습니다.");
    }

    // 즉사 모드를 활성화하는 공개 메서드 (붉은 아이템 획득 시 호출)
    public void ActivateInstantKillEffect()
    {
        // 기존 코루틴이 있다면 중지하여 중복 실행 방지
        if (instantKillCoroutine != null)
        {
            StopCoroutine(instantKillCoroutine);
        }
        instantKillCoroutine = StartCoroutine(InstantKillRoutine());
    }

    // 즉사 모드 코루틴
    private IEnumerator InstantKillRoutine()
    {
        isInstantKillActive = true;
        Debug.Log($"[PlayerAttack] 즉사 모드 활성화! ({instantKillDuration}초 동안)");

        yield return new WaitForSeconds(instantKillDuration);

        isInstantKillActive = false;
        Debug.Log("[PlayerAttack] 즉사 모드 비활성화.");
    }
}
