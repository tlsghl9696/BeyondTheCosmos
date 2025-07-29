using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossHP : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    public Image hpFillImage;   // 체력바 Fill 이미지
    public Text hpText;         // 퍼센트 텍스트
    public SpriteRenderer bossSprite; // 보스 스프라이트 (깜빡임용)

    public AudioClip hitSoundSFX;         // 피격 사운드 클립
    public float hitSoundVolume = 0.5f;   // 피격 사운드 볼륨 (0~1, 인스펙터 조절 가능)

    public AudioClip deathSoundSFX;       // 사망 사운드 클립
    public float deathSoundVolume = 0.4f;   // 사망 사운드 볼륨 (0~1, 인스펙터 조절 가능)

    private AudioSource audioSource;

    public GameObject bossDeathExplosionPrefab; // 보스 사망 시 사용할 ExplosionEffect 프리팹 (Inspector에서 할당)
    // 보스 사망 폭발 이펙트 재생 횟수
    [Range(1, 5)] // Inspector에서 1에서 5 사이의 정수로 조절 가능하게 함
    public int numberOfExplosions = 2; // 폭발 이펙트 재생 횟수 (기본 2회)
    // 각 폭발 이펙트 사이의 지연 시간
    public float timeBetweenExplosions = 0.2f; // 각 폭발 사이의 시간

    public GameObject[] itemDropPrefabs; // 보스 피격 시 드랍될 아이템 프리팹 배열
    [Range(0, 100)]
    public float itemDropChance = 5f;

    // 아이템 드랍 시 강제 스케일 값
    public float itemDropScale = 0.2f; // <-- 여기에 원하는 스케일 값을 설정 (Inspector에서 조절 가능)

    public GameUIController gameUIController; // GameUIController 참조

    // 보스가 사망 중인지 확인하는 플래그 (중복 Die() 호출 방지)
    private bool isDying = false;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource가 없어 자동으로 추가됨. 인스펙터에서 설정해주세요.");
        }

        // GameUIController를 찾아 할당합니다.
        if (gameUIController == null)
        {
            gameUIController = FindObjectOfType<GameUIController>();
            if (gameUIController == null)
            {
                Debug.LogError("GameUIController가 씬에 없습니다. Canvas에 추가하고 BossHP에서 할당하세요.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return; // 이미 사망 중이면 추가 데미지 처리 방지

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();
        StartCoroutine(HitFlash());

        // 피격 사운드 재생 (볼륨 조절 가능)
        if (audioSource != null && hitSoundSFX != null)
        {
            audioSource.PlayOneShot(hitSoundSFX, hitSoundVolume);
        }

        // 아이템 드랍
        if (itemDropPrefabs.Length > 0)
        {
            if (Random.Range(0f, 100f) < itemDropChance)
            {
                GameObject randomItem = itemDropPrefabs[Random.Range(0, itemDropPrefabs.Length)];

                // --- 아이템 스폰 위치 무작위화 ---
                float offsetX = Random.Range(-2f, 2f); // 원하는 X 오프셋 범위
                float offsetY = Random.Range(-1f, 1f); // 원하는 Y 오프셋 범위
                Vector3 dropPosition = transform.position + new Vector3(offsetX, offsetY, 0);

                // --- 아이템 생성 및 스케일 강제 설정 ---
                GameObject spawnedItem = Instantiate(randomItem, dropPosition, Quaternion.identity);
                spawnedItem.transform.localScale = Vector3.one * itemDropScale; // 여기에서 스케일 강제 적용!
                                                                                // 추가: 실제로 적용된 스케일 값 로그 출력
                Debug.Log($"[BossHP] 생성된 아이템 '{spawnedItem.name}'의 최종 스케일: {spawnedItem.transform.localScale}");

                Debug.Log($"아이템 드랍 성공 ({itemDropChance}%) at {dropPosition} with scale {itemDropScale}");
            }
            else
            {
                Debug.Log($"아이템 드랍 실패 ({itemDropChance}%)");
            }
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    IEnumerator HitFlash()
    {
        if (bossSprite != null)
        {
            bossSprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            bossSprite.color = Color.white;
        }
    }

    void UpdateHPUI()
    {
        if (hpFillImage != null)
            hpFillImage.fillAmount = (float)currentHP / maxHP;

        if (hpText != null)
            hpText.text = $"{(int)((float)currentHP / maxHP * 100)}%";
    }

    // 보스 사망 시 호출되는 함수입니다.
    void Die()
    {
        if (isDying) return; // 이미 사망 처리 중이면 중복 실행 방지
        isDying = true; // 사망 처리 시작 플래그 설정

        Debug.Log("[BossHP] 보스 사망! 사망 처리 시작."); // New log

        // 모든 적 총알 제거
        DestroyAllEnemyBullets();

        // [수정] 보스 시각 효과 및 물리 효과를 즉시 비활성화하여 즉사 처리합니다.
        // 모든 Renderer 컴포넌트를 비활성화하여 보스를 즉시 숨깁니다.
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in allRenderers)
        {
            renderer.enabled = false;
            Debug.Log($"[BossHP] Renderer '{renderer.GetType().Name}' 비활성화됨.");
        }

        Collider2D bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null) bossCollider.enabled = false;

        // Rigidbody2D가 있다면 물리적 움직임을 즉시 멈춥니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // 현재 속도 0으로 설정
            rb.angularVelocity = 0f; // 회전 속도 0으로 설정
            rb.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정 (선택 사항, 즉시 멈추는 데 도움)
            Debug.Log("[BossHP] Rigidbody2D 물리 정지."); // New log
        }

        // [수정된 부분] 보스 오브젝트와 모든 자식 오브젝트에 붙은 MonoBehaviour 스크립트를 비활성화하고, 실행 중인 코루틴을 모두 중지합니다.
        MonoBehaviour[] allScripts = GetComponentsInChildren<MonoBehaviour>(); // GetComponentsInChildren 사용
        foreach (MonoBehaviour script in allScripts)
        {
            // 이 BossHP 스크립트는 Die()를 호출 중이므로 비활성화하지 않습니다.
            // ExplosionEffect 스크립트 (프리팹에 붙어있으므로 여기서 직접 비활성화할 필요 없음)
            // GameUIController (씬 전체에 영향을 미치므로 비활성화하지 않습니다.)
            if (script != this && !(script is ExplosionEffect) && !(script is GameUIController))
            {
                script.enabled = false; // 스크립트 비활성화
                script.StopAllCoroutines(); // 해당 스크립트에서 실행 중인 모든 코루틴 중지
                Debug.Log($"[BossHP] 스크립트 '{script.GetType().Name}' 비활성화 및 모든 코루틴 중지."); // 진단 로그
            }
        }

        // [추가] Animator 컴포넌트가 있다면 비활성화하거나 상태를 설정합니다.
        Animator bossAnimator = GetComponent<Animator>();
        if (bossAnimator != null)
        {
            bossAnimator.enabled = false; // Animator 비활성화
            // 필요하다면 bossAnimator.SetTrigger("Death"); 와 같이 죽음 애니메이션 트리거를 사용할 수도 있습니다.
            Debug.Log("[BossHP] 보스 Animator 비활성화."); // 진단 로그
        }

        // 사망 사운드 재생 (볼륨 조절 가능)
        float deathSoundLength = 0f;
        if (audioSource != null && deathSoundSFX != null)
        {
            audioSource.PlayOneShot(deathSoundSFX, deathSoundVolume);
            deathSoundLength = deathSoundSFX.length;
            Debug.Log($"[BossHP] 사망 사운드 재생 시작. 길이: {deathSoundLength}초."); // 진단 로그
        }

        Debug.Log("[BossHP] HandleBossDeathSequence 코루틴 시작 직전."); // 진단 로그
        // 폭발 이펙트 재생 및 오브젝트 파괴를 위한 코루틴 시작
        // 패널 활성화는 이제 이 코루틴의 마지막 단계에서 이루어집니다.
        StartCoroutine(HandleBossDeathSequence(deathSoundLength));
    }

    // 모든 적 총알을 제거하는 함수입니다.
    void DestroyAllEnemyBullets()
    {
        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in enemyBullets)
        {
            Destroy(bullet);
        }
        Debug.Log("보스 사망으로 모든 적 총알 제거 완료");
    }

    // 보스 사망 시퀀스를 처리하는 코루틴입니다.
    IEnumerator HandleBossDeathSequence(float deathSoundLength)
    {
        Debug.Log("[BossHP][HandleBossDeathSequence] 코루틴 시작."); // 진단 로그

        // 2. 폭발 이펙트 재생 (사운드와 거의 동시에 시작)
        float maxExplosionEffectDuration = 0f; // 가장 긴 이펙트 지속 시간을 저장
        if (bossDeathExplosionPrefab != null)
        {
            ExplosionEffect explosionEffectScriptInPrefab = bossDeathExplosionPrefab.GetComponent<ExplosionEffect>();
            if (explosionEffectScriptInPrefab != null)
            {
                maxExplosionEffectDuration = explosionEffectScriptInPrefab.effectDuration;
                Debug.Log($"[BossHP] 프리팹 '{bossDeathExplosionPrefab.name}'에서 ExplosionEffect 발견! Effect Duration: {maxExplosionEffectDuration}."); // 진단 로그
            }
            else
            {
                Debug.LogWarning("[BossHP] bossDeathExplosionPrefab에 ExplosionEffect 스크립트가 없습니다. effectDuration을 가져올 수 없습니다. 기본값 1.0f 사용.");
                maxExplosionEffectDuration = 1.0f; // 기본값
            }

            Debug.Log($"[BossHP][HandleBossDeathSequence] 폭발 이펙트 {numberOfExplosions}회 재생 시작."); // 진단 로그

            for (int i = 0; i < numberOfExplosions; i++)
            {
                GameObject effectInstance = Instantiate(bossDeathExplosionPrefab, transform.position, Quaternion.identity);

                ExplosionEffect explosionEffectScript = effectInstance.GetComponent<ExplosionEffect>();
                if (explosionEffectScript != null)
                {
                    Debug.Log($"[BossHP] 폭발 이펙트 인스턴스 '{effectInstance.name}'에 ExplosionEffect 스크립트 성공적으로 부착됨."); // 진단 로그
                    explosionEffectScript.PlayEffect(); // 이펙트 재생 시작 (ExplosionEffect가 스스로 파괴됨)
                }
                else
                {
                    Debug.LogWarning($"[BossHP] 할당된 보스 사망 이펙트 프리팹 '{bossDeathExplosionPrefab.name}'의 인스턴스에 ExplosionEffect 스크립트가 없습니다. 이펙트가 자동으로 파괴되지 않을 수 있습니다."); // 진단 로그
                }
                // 마지막 폭발을 제외하고 각 폭발 사이에 지연 시간 추가
                if (i < numberOfExplosions - 1)
                {
                    yield return new WaitForSeconds(timeBetweenExplosions);
                    Debug.Log($"[BossHP][HandleBossDeathSequence] 폭발 {i + 1}회 후 {timeBetweenExplosions}초 대기."); // 진단 로그
                }
            }
            Debug.Log("[BossHP][HandleBossDeathSequence] 모든 폭발 이펙트 인스턴스화 완료."); // 진단 로그
        }
        else
        {
            Debug.LogWarning("[BossHP] bossDeathExplosionPrefab이 할당되지 않았습니다.");
        }

        // 3. 모든 폭발 이펙트가 완전히 끝날 때까지 대기
        // 폭발 이펙트 시퀀스의 총 지속 시간 계산: (폭발 횟수-1) * 폭발 사이 간격 + 단일 폭발 지속 시간
        float totalExplosionSequenceTime = (numberOfExplosions > 0) ? ((numberOfExplosions - 1) * timeBetweenExplosions + maxExplosionEffectDuration) : 0f;

        // 사망 사운드 길이와 폭발 이펙트 시퀀스 중 더 긴 시간 동안 대기합니다.
        // 폭발 이펙트와 사운드가 동시에 시작되었으므로 둘 중 더 오래 걸리는 시간을 기다립니다.
        float totalDelayForEffects = Mathf.Max(deathSoundLength, totalExplosionSequenceTime);

        Debug.Log($"[BossHP][HandleBossDeathSequence] 이펙트 대기 시간 계산: {totalDelayForEffects}초 (사운드 길이: {deathSoundLength}, 폭발 시퀀스: {totalExplosionSequenceTime})."); // 진단 로그
        yield return new WaitForSeconds(totalDelayForEffects);
        Debug.Log("[BossHP][HandleBossDeathSequence] 이펙트 대기 시간 완료. 보스 오브젝트 파괴 진행."); // 진단 로그

        // 4. 보스 오브젝트 파괴
        Destroy(gameObject);
        Debug.Log("[BossHP] 보스 오브젝트 파괴 완료."); // 진단 로그

        // 5. [수정] 게임 클리어 패널 활성화
        if (gameUIController != null)
        {
            gameUIController.ShowGameCompletePanel(); // 게임 완료 패널 활성화
            Debug.Log("[BossHP] 게임 완료 패널 활성화 요청."); // 진단 로그
        }
        else
        {
            Debug.LogWarning("GameUIController가 할당되지 않아 게임 완료 패널을 표시할 수 없습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying) return; // 사망 중에는 충돌 처리 방지

        if (collision.CompareTag("Bullet"))
        {
            Bullet playerBullet = collision.GetComponent<Bullet>();
            if (playerBullet != null)
            {
                if (playerBullet.isInstantKillBullet)
                {
                    currentHP = 0;
                }
                else
                {
                    TakeDamage(playerBullet.damageAmount);
                }
            }
            else
            {
                TakeDamage(10);
            }
        }
    }
}
