using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요
using UnityEngine.UI; // UI 요소 관리를 위해 필요

public class GameManager : MonoBehaviour
{
    // GameManager의 단일 인스턴스를 유지합니다.
    public static GameManager instance;

    [Header("게임 오버 UI")]
    // 게임 오버 패널 GameObject입니다.
    public GameObject gameOverPanel;
    // 게임 재시작 버튼입니다.
    public Button restartButton;

    [Header("씬 설정")]
    // 초기 씬의 이름 (현재 사용되지 않지만, 필요에 따라 활용 가능)
    public string initialSceneName;
    // 아이템 패널 GameObject입니다.
    public GameObject itemPanel;

    void Awake()
    {
        // 싱글톤 패턴 구현: 씬에 하나의 인스턴스만 존재하도록 합니다.
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // GameManager도 씬 재시작 시 함께 파괴되도록 합니다.
            // 씬 재시작 시 각 스크립트가 새로 초기화되어 올바른 상태를 유지하도록 합니다.
        }
        else if (instance != this)
        {
            // 이미 인스턴스가 존재하면 새로 생성된 이 오브젝트를 파괴합니다.
            Destroy(gameObject);
        }

        // 재시작 버튼이 할당되어 있으면 클릭 리스너를 추가합니다.
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    // 게임 오버 상태를 처리하는 함수입니다.
    public void GameOver()
    {
        Debug.Log("[GameManager] 게임 오버!");
        // 게임 시간을 정지하여 모든 움직임을 멈춥니다.
        Time.timeScale = 0;

        // 게임 오버 패널이 할당되어 있으면 활성화합니다.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("[GameManager] Game Over Panel이 할당되지 않았습니다!");
        }
    }

    // 게임을 재시작하는 함수입니다.
    public void RestartGame()
    {
        Debug.Log("[GameManager] 게임 재시작");

        // 게임 시간을 다시 정상 속도로 되돌립니다.
        Time.timeScale = 1;

        // 점수 초기화
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore(); // ScoreManager의 점수 값을 0으로 설정하고 UI를 갱신합니다.
        }

        // 보스 스폰 차단 플래그를 비활성화하여 다음 게임에서는 보스가 다시 스폰될 수 있도록 합니다.
        // 이 플래그는 static이므로 직접 false로 재설정해야 합니다.
        BossSpawner.forcePreventSpawn = false;

        // 현재 활성화된 씬을 다시 로드하여 게임을 초기 상태로 되돌립니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 게임을 종료하는 함수입니다.
    public void QuitGame()
    {
        Debug.Log("[GameManager] 게임 종료.");
        // 실제 빌드된 애플리케이션을 종료합니다.
        Application.Quit();
        // UNITY_EDITOR 전처리기 지시문을 사용하여 에디터에서 실행 중일 때만 에디터 플레이 모드를 종료합니다.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void Update()
    {
        // 게임 오버 패널이 활성화되어 있을 때만 키 입력을 감지합니다.
        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            // Space 또는 Enter 키를 누르면 게임 재시작
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                RestartGame();
            }
            // Escape 키를 누르면 게임 종료
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }
    }
}
