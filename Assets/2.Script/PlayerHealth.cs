using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    private int currentHP;
    public AudioClip hitSFX;     // 피격 사운드
    public AudioClip deathSFX;   // 사망 사운드

    public Image[] hpIcons;      // UI에서 하트 이미지 배열
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private AudioSource audioSource; // 피격 사운드 재생을 위한 AudioSource 컴포넌트

    public float hitEffectVolume = 1.0f; // 피격 효과음 볼륨 조절 변수 (Inspector에서 조절)
    public float deathEffectVolume = 1.0f; // 사망 효과음 볼륨 조절 변수 (Inspector에서 조절)

    public GameObject playerExplosionFXPrefab; // 플레이어 폭발 이펙트 프리팹 (Inspector에서 할당)

    public bool isInvincible { get; private set; } // 무적 상태 여부 (외부에서 읽기만 가능)

    // 각 무적 타입별 코루틴 참조
    private Coroutine currentHitInvincibilityCoroutine; // 피격 무적 코루틴
    private Coroutine currentItemInvincibilityCoroutine; // 아이템 무적 코루틴

    private PlayerBlinkEffect playerBlinkEffect;

    // 현재 활성화된 무적 효과 중 가장 긴 지속 시간을 추적
    private float hitInvincibilityEndTime = 0f; // 피격 무적 종료 시간 (Time.unscaledTime 기준)
    private float itemInvincibilityEndTime = 0f; // 아이템 무적 종료 시간 (Time.unscaledTime 기준)

    public void RestoreHP(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // HP가 maxHP를 초과하지 않도록 보장
        UpdateUI(); // UI 업데이트
    }

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource component was not found on " + gameObject.name + " and was added automatically. Please configure it in the Inspector.");
        }

        playerBlinkEffect = GetComponentInParent<PlayerBlinkEffect>();
        if (playerBlinkEffect == null)
        {
            Debug.LogWarning("[PlayerHealth] PlayerBlinkEffect 컴포넌트를 부모에서 찾을 수 없습니다. 깜빡임 효과가 작동하지 않을 수 있습니다. PlayerRoot GameObject에 PlayerBlinkEffect 스크립트를 추가하고 Player Body GameObject를 Player(자식)로 할당해주세요.");
        }
        UpdateInvincibilityState(); // 초기 무적 상태 업데이트
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"[PlayerHealth] TakeDamage 호출됨. 무적 상태: {isInvincible}. 데미지량: {amount}. 현재 HP: {currentHP} (Time: {Time.time:F2}s)");

        if (isInvincible) // 무적 상태일 때는 데미지를 받지 않습니다.
        {
            Debug.Log("[PlayerHealth] 플레이어가 무적 상태이므로 데미지를 받지 않습니다. (TakeDamage 리턴)");
            return;
        }
        if (currentHP <= 0) // 이미 죽었으면 무시
        {
            Debug.Log("[PlayerHealth] 플레이어가 이미 사망 상태이므로 데미지를 받지 않습니다. (TakeDamage 리턴)");
            return;
        }

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();

        Debug.Log($"[PlayerHealth] 데미지 적용 완료! 새 HP: {currentHP}. (Time: {Time.time:F2}s)");

        if (audioSource != null && hitSFX != null)
        {
            audioSource.PlayOneShot(hitSFX, hitEffectVolume);
        }

        if (currentHP <= 0) // 플레이어 사망 시
        {
            Debug.Log("[PlayerHealth] Player Dead - Calling GameManager.GameOver()");

            if (deathSFX != null)
            {
                GameObject tempAudioObject = new GameObject("TempDeathSound");
                tempAudioObject.transform.position = transform.position;
                AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

                tempAudioSource.clip = deathSFX;
                tempAudioSource.volume = deathEffectVolume;
                tempAudioSource.spatialBlend = 0;
                tempAudioSource.Play();

                Destroy(tempAudioObject, deathSFX.length);
            }

            if (playerExplosionFXPrefab != null)
            {
                GameObject explosionInstance = Instantiate(playerExplosionFXPrefab, transform.position, Quaternion.identity);
                ExplosionEffect fxScript = explosionInstance.GetComponent<ExplosionEffect>();
                if (fxScript != null)
                {
                    fxScript.PlayEffect();
                }
                else
                {
                    Debug.LogWarning("Instantiated playerExplosionFXPrefab does not have ExplosionEffect script.");
                }
            }
            else
            {
                Debug.LogWarning("Player Explosion FX Prefab is not assigned in PlayerHealth script.");
            }

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            else
            {
                Debug.LogError("[PlayerHealth] GameManager 인스턴스를 찾을 수 없습니다! 'GameManager.cs' 스크립트가 씬에 있는지, 그리고 싱글톤 'instance'가 올바르게 설정되었는지 확인하세요.");
            }

            Destroy(gameObject); // 플레이어 오브젝트 파괴
        }
        else // 플레이어가 죽지 않고 데미지를 받았을 경우
        {
            // 피격 시 무적 효과 시작
            TriggerHitInvincibility(1.0f); // 피격 무적 시간
        }
    }

    // --- 피격 무적 관련 함수 ---
    public void TriggerHitInvincibility(float duration)
    {
        if (currentHitInvincibilityCoroutine != null)
        {
            StopCoroutine(currentHitInvincibilityCoroutine);
        }
        hitInvincibilityEndTime = Time.unscaledTime + duration; // Time.unscaledTime 사용
        currentHitInvincibilityCoroutine = StartCoroutine(HitInvincibilityRoutine(duration));
        UpdateInvincibilityState(); // 무적 상태 즉시 업데이트
        Debug.Log($"[PlayerHealth] 피격 무적 시작 요청. 지속 시간: {duration:F2}s. 종료 시간: {hitInvincibilityEndTime:F2}");
    }

    private IEnumerator HitInvincibilityRoutine(float duration)
    {
        Debug.Log($"[PlayerHealth] 피격 무적 코루틴 시작. (시작 시간: {Time.unscaledTime:F2}s, 지속 시간: {duration:F2}s)");
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Debug.Log($"[PlayerHealth] 피격 무적 코루틴 종료. (종료 시간: {Time.unscaledTime:F2}s)");
        hitInvincibilityEndTime = 0f; // 종료 시간 리셋
        currentHitInvincibilityCoroutine = null;
        UpdateInvincibilityState(); // 무적 상태 업데이트
    }

    // --- 아이템 무적 관련 함수 (ItemEffectManager에서 호출) ---
    public void TriggerItemInvincibility(float duration)
    {
        if (currentItemInvincibilityCoroutine != null)
        {
            StopCoroutine(currentItemInvincibilityCoroutine);
        }
        itemInvincibilityEndTime = Time.unscaledTime + duration; // Time.unscaledTime 사용
        currentItemInvincibilityCoroutine = StartCoroutine(ItemInvincibilityRoutine(duration));
        UpdateInvincibilityState(); // 무적 상태 즉시 업데이트
        Debug.Log($"[PlayerHealth] 아이템 무적 시작 요청. 지속 시간: {duration:F2}s. 종료 시간: {itemInvincibilityEndTime:F2}");
    }

    private IEnumerator ItemInvincibilityRoutine(float duration)
    {
        Debug.Log($"[PlayerHealth] 아이템 무적 코루틴 시작. (시작 시간: {Time.unscaledTime:F2}s, 지속 시간: {duration:F2}s)");
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Debug.Log($"[PlayerHealth] 아이템 무적 코루틴 종료. (종료 시간: {Time.unscaledTime:F2}s)");
        itemInvincibilityEndTime = 0f; // 종료 시간 리셋
        currentItemInvincibilityCoroutine = null;
        UpdateInvincibilityState(); // 무적 상태 업데이트
    }

    // 최종 무적 상태를 업데이트하고 깜빡임 효과를 제어하는 함수
    void UpdateInvincibilityState()
    {
        // 피격 무적과 아이템 무적 중 현재 유효한지 확인
        bool isHitInvincibleActive = Time.unscaledTime < hitInvincibilityEndTime;
        bool isItemInvincibleActive = Time.unscaledTime < itemInvincibilityEndTime;

        // 최종 무적 상태 결정
        bool newIsInvincible = isHitInvincibleActive || isItemInvincibleActive;

        // 무적 상태가 변경되었거나, 무적 상태 내에서 시각 효과가 변경될 필요가 있을 때
        if (isInvincible != newIsInvincible || (isInvincible && (isHitInvincibleActive || isItemInvincibleActive)))
        {
            isInvincible = newIsInvincible;
            Debug.Log($"[PlayerHealth] 최종 무적 상태 업데이트: {isInvincible}. (Hit End: {hitInvincibilityEndTime:F2}, Item End: {itemInvincibilityEndTime:F2}, Current: {Time.unscaledTime:F2})");

            if (playerBlinkEffect != null)
            {
                if (isInvincible)
                {
                    // 가장 긴 남은 시간을 PlayerBlinkEffect에 전달하여 깜빡임 시작
                    float remainingDuration = Mathf.Max(
                        hitInvincibilityEndTime - Time.unscaledTime,
                        itemInvincibilityEndTime - Time.unscaledTime
                    );

                    // 어떤 무적 효과에 따른 색상을 사용할지 결정
                    Color activeBlinkColor;
                    if (isHitInvincibleActive) // 피격 무적이 활성화되어 있으면 붉은색 우선
                    {
                        activeBlinkColor = Color.red; // 피격 시 붉은색
                    }
                    else if (isItemInvincibleActive) // 아이템 무적만 활성화되어 있으면 노란색
                    {
                        activeBlinkColor = Color.yellow; // 아이템 무적 시 노란색
                    }
                    else // 무적 상태인데 어떤 코루틴도 활성화되어 있지 않다면 (예외 상황)
                    {
                        activeBlinkColor = Color.white; // 기본 깜빡임 색상 (폴백)
                    }

                    if (remainingDuration > 0)
                    {
                        playerBlinkEffect.StartBlinking(remainingDuration, activeBlinkColor); // 색상 전달
                        Debug.Log($"[PlayerHealth] 깜빡임 시작 요청. 남은 시간: {remainingDuration:F2}s. 색상: {activeBlinkColor}");
                    }
                }
                else
                {
                    playerBlinkEffect.StopBlinking();
                    Debug.Log("[PlayerHealth] 깜빡임 효과 강제 중지.");
                }
            }
        }
    }


    void UpdateUI()
    {
        for (int i = 0; i < hpIcons.Length; i++)
        {
            if (i < currentHP)
                hpIcons[i].sprite = fullHeart;
            else
                hpIcons[i].sprite = emptyHeart;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string collidedTag;
        if (other.CompareTag("EnemyBullet"))
        {
            collidedTag = "EnemyBullet";
        }
        else if (other.CompareTag("Meteor"))
        {
            collidedTag = "Meteor";
        }
        else
        {
            collidedTag = other.tag;
        }

        Debug.Log($"[PlayerHealth] OnTriggerEnter2D 발생! 충돌 대상: {other.gameObject.name}, 태그: {collidedTag}. Time: {Time.time:F2}s");

        bool isHandled = false;
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            if (enemyBullet != null && enemyBullet.spawner != null)
            {
                enemyBullet.spawner.ReturnBulletToPool(other.gameObject);
                Debug.Log($"[PlayerHealth] EnemyBullet 풀로 반환됨: {other.gameObject.name}");
            }
            else
            {
                Destroy(other.gameObject);
                Debug.Log($"[PlayerHealth] EnemyBullet 파괴됨: {other.gameObject.name}");
            }
            isHandled = true;
        }
        else if (other.CompareTag("Meteor"))
        {
            Destroy(other.gameObject); // 운석 충돌 시 즉시 파괴
            Debug.Log($"[PlayerHealth] Meteor 충돌 감지 및 파괴됨: {other.gameObject.name}");
            isHandled = true;
        }

        if (isHandled) // 충돌한 오브젝트가 처리될 수 있는 대상이었을 경우에만 데미지 로직 실행
        {
            Debug.Log($"[PlayerHealth] 적 총알 또는 운석과 충돌. 무적 상태: {isInvincible}. Time: {Time.time:F2}s");
            TakeDamage(1);
        }
        else
        {
            Debug.Log($"[PlayerHealth] 처리되지 않은 충돌 발생: {other.gameObject.name}, 태그: {other.tag}");
        }
    }
}
