using UnityEngine;
using System.Collections; // For Coroutines
using UnityEngine.UI; // UI 요소를 위해 필요합니다.
using TMPro; // TextMeshProUGUI를 사용한다면 필요합니다.

public class MeteorHP : MonoBehaviour
{
    public int maxHP = 3; // 운석의 최대 HP (인스펙터에서 설정)
    private int currentHP; // 운석의 현재 HP

    public GameObject meteorExplosionPrefab; // [수정] 운석 파괴 효과를 위한 ExplosionEffect 프리팹 (Inspector에서 할당)
    public int scoreValue = 100; // 운석 파괴 시 얻는 점수

    public AudioClip hitSoundSFX; // 운석 피격 시 재생할 사운드 클립
    private AudioSource audioSource; // 사운드 재생을 위한 AudioSource 컴포넌트

    [Header("UI 설정")]
    public Image hpFillImage;   // 체력바 Fill 이미지 (선택 사항, 없어도 작동)
    public Text hpText; // 퍼센트 텍스트 (선택 사항, 없어도 작동)

    // [추가된 부분] 아이템 드랍 관련 설정
    public GameObject[] itemDropPrefab; // 드랍될 아이템 프리팹
    [Range(0, 100)] // [추가] Inspector에서 0~100 사이의 슬라이더로 조절 가능하게 함
    public float itemDropScale = 0.2f;
    public float itemDropChance = 100f; // [추가] 아이템 드랍 확률 (0~100 백분율 단위)
    public float spawnedItem = 0.2f; // [추가] 드랍된 아이템의 크기


    void Start()
    {
        currentHP = maxHP; // 시작 시 현재 HP를 최대 HP로 설정
        UpdateHPUI(); // UI 업데이트

        // AudioSource 컴포넌트 가져오기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D 사운드로 설정
            Debug.LogWarning("AudioSource component not found on " + gameObject.name + " and was added automatically. Please configure it in the Inspector.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 총알과 충돌했을 때 (태그: Bullet)
        if (other.CompareTag("Bullet"))
        {
            Bullet playerBullet = other.GetComponent<Bullet>();
            if (playerBullet != null)
            {
                // 피격 사운드 재생
                if (audioSource != null && hitSoundSFX != null)
                {
                    audioSource.PlayOneShot(hitSoundSFX, 0.7f); // 볼륨 0.7f 예시
                }

                if (playerBullet.isInstantKillBullet) // 즉사 총알인 경우 (붉은 아이템 효과)
                {
                    currentHP = 0; // 즉사
                }
                else // 일반 총알인 경우
                {
                    currentHP -= playerBullet.damageAmount; // 총알 데미지만큼 HP 감소
                }
            }
            else // Bullet 스크립트를 찾을 수 없는 경우 (안전망)
            {
                currentHP--; // 기본 1 데미지
            }

            currentHP = Mathf.Clamp(currentHP, 0, maxHP); // HP가 0 미만으로 내려가지 않도록 보정
            UpdateHPUI(); // HP 변화 시 UI 업데이트

            if (currentHP <= 0)
            {
                Die();
            }
            // 총알은 Bullet.cs 스크립트에서 풀로 반환되거나 파괴되므로,
            // 여기서는 총알 파괴 로직을 추가하지 않습니다.
        }
    }

    // 체력바 UI를 업데이트하는 메서드
    void UpdateHPUI()
    {
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = (float)currentHP / maxHP;
        }
        if (hpText != null)
        {
            // [수정된 부분] 운석 체력은 자연수로 표시 (이전 요청대로 복구)
            hpText.text = $"{currentHP} / {maxHP}";
        }
    }

    void Die()
    {
        // 점수 추가
        ScoreManager scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(scoreValue); // ScoreManager의 AddScore 메서드 호출
        }
        else
        {
            Debug.LogWarning("ScoreManager not found. Score will not be added.");
        }

        // 폭발 이펙트 생성
        if (meteorExplosionPrefab != null)
        {
            GameObject effectInstance = Instantiate(meteorExplosionPrefab, transform.position, transform.rotation);
            ExplosionEffect explosionEffectScript = effectInstance.GetComponent<ExplosionEffect>();
            if (explosionEffectScript != null)
            {
                explosionEffectScript.PlayEffect(); // 이펙트 재생 시작
            }
            else
            {
                Debug.LogWarning("[MeteorHP] 할당된 운석 파괴 이펙트 프리팹에 ExplosionEffect 스크립트가 없습니다.");
            }
        }

        // [추가된 부분] 운석 파괴 시 아이템 드랍
        if (itemDropPrefab != null && Random.Range(0f, 100f) < itemDropChance)
        {
            GameObject selectedItem = itemDropPrefab[Random.Range(0, itemDropPrefab.Length)];
            GameObject spawnedItem = Instantiate(selectedItem, transform.position, Quaternion.identity); // 수정된 부분
            spawnedItem.transform.localScale = Vector3.one * itemDropScale; // 올바른 참조로 수정
            Debug.Log("[MeteorHP] 운석 파괴 시 아이템 드랍!");
        }


        Destroy(gameObject); // 운석 파괴
    }
}