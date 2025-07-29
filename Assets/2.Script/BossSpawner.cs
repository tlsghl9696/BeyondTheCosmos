using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    // 보스 프리팹입니다.
    public GameObject bossPrefab;
    // 보스가 스폰될 위치입니다.
    public Transform bossSpawnPoint;
    // 보스가 스폰되기 위한 필요 점수입니다.
    public int requiredScore = 1500;

    // 배경 음악 AudioSource입니다.
    public AudioSource bgmSource;
    // 보스전 배경 음악입니다.
    public AudioClip bossBGM;

    // 보스가 스폰되었는지 여부를 추적하는 플래그입니다.
    private bool bossSpawned = false;
    // ScoreManager 참조입니다.
    private ScoreManager scoreManager;

    // 보스 스폰을 강제로 막을지 여부를 결정하는 static 플래그입니다.
    // 게임 재시작 시 이 플래그를 false로 설정해야 보스가 다시 스폰될 수 있습니다.
    public static bool forcePreventSpawn = false;

    void Start()
    {
        // 씬에서 ScoreManager 인스턴스를 찾아 연결합니다.
        // ScoreManager가 DontDestroyOnLoad를 사용하지 않으므로, 매 씬 로드 시 새로 찾아야 합니다.
        scoreManager = FindObjectOfType<ScoreManager>();

        // forcePreventSpawn이 true이면 보스 스폰을 강제로 차단하고 함수를 종료합니다.
        if (forcePreventSpawn)
        {
            Debug.Log("[BossSpawner] 보스 스폰 강제 차단됨");
            return;
        }

        // 보스 스폰 상태를 초기화합니다.
        bossSpawned = false;
    }

    void Update()
    {
        // 보스가 이미 스폰되었거나 ScoreManager가 없으면 업데이트를 건너_ㅂ니다.
        if (bossSpawned || scoreManager == null) return;

        // 현재 점수가 필요 점수 이상이면 보스를 스폰합니다.
        if (scoreManager.currentScore >= requiredScore)
        {
            SpawnBoss();
        }
    }

    // 보스를 스폰하는 함수입니다.
    void SpawnBoss()
    {
        // 보스가 스폰되었음을 표시합니다.
        bossSpawned = true;

        // 보스 프리팹을 지정된 위치에 인스턴스화합니다.
        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        // MeteorSpawner를 찾아 비활성화하여 메테오 스폰을 중지합니다.
        MeteorSpawner meteorSpawner = FindObjectOfType<MeteorSpawner>();
        if (meteorSpawner != null)
        {
            meteorSpawner.enabled = false;
        }

        // 씬에 남아있는 모든 메테오를 찾아서 파괴합니다.
        var meteors = Object.FindObjectsByType<Meteor>(FindObjectsSortMode.None);
        foreach (Meteor meteor in meteors)
        {
            Destroy(meteor.gameObject);
        }

        // 배경 음악 AudioSource가 할당되어 있고 보스 BGM이 할당되어 있으면 BGM을 변경합니다.
        if (bgmSource != null && bossBGM != null)
        {
            bgmSource.Stop(); // 기존 BGM 정지
            bgmSource.clip = bossBGM; // 보스 BGM으로 변경
            bgmSource.volume = 0.75f; // 보스 BGM 볼륨 설정 (조절 가능)
            bgmSource.Play(); // 보스 BGM 재생
        }

        Debug.Log("보스 등장!");
    }
}
