using UnityEngine;
using UnityEngine.UI; // UI 요소를 다루기 위해 필요
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요 (게임 종료 등)

public class PauseManager : MonoBehaviour
{
    // 일시정지 UI 패널을 Inspector에서 연결할 수 있도록 public으로 선언
    public GameObject pausePanel;

    // 게임이 현재 일시정지 상태인지 추적하는 변수
    private bool isGamePaused = false;

    void Start()
    {
        // 게임 시작 시 일시정지 패널은 비활성화 상태여야 함
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[PauseManager] Pause Panel이 할당되지 않았습니다. Inspector에서 할당해주세요.");
        }
    }

    // ESC 키 입력 감지를 위해 Update 함수 사용
    void Update()
    {
        // ESC 키를 눌렀을 때 일시정지/재개 상태를 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // 일시정지 상태를 토글하는 메인 함수 (버튼 클릭, ESC 키 등에 연결)
    public void TogglePause()
    {
        isGamePaused = !isGamePaused; // 현재 상태 반전

        if (isGamePaused)
        {
            // 게임 일시정지
            Time.timeScale = 0f; // 게임 내 모든 시간 흐름을 멈춤
            if (pausePanel != null)
            {
                pausePanel.SetActive(true); // 일시정지 패널 활성화
            }
            Debug.Log("[PauseManager] 게임 일시정지됨. Time.timeScale = 0");
        }
        else
        {
            // 게임 재개
            Time.timeScale = 1f; // 시간 흐름을 정상으로 돌림
            if (pausePanel != null)
            {
                pausePanel.SetActive(false); // 일시정지 패널 비활성화
            }
            Debug.Log("[PauseManager] 게임 재개됨. Time.timeScale = 1");
        }
    }

    // 일시정지 패널의 "계속하기" 버튼에 연결할 함수
    public void ResumeGame()
    {
        // 이미 일시정지 상태라면 TogglePause를 호출하여 재개
        if (isGamePaused)
        {
            TogglePause();
        }
    }

    // 일시정지 패널의 "설정" 버튼에 연결할 함수 (예시)
    public void OpenSettings()
    {
        Debug.Log("[PauseManager] 설정 창 열기 (구현 필요)");
        // 여기에 설정 UI를 활성화하는 로직 추가
    }

    // 일시정지 패널의 "게임 종료" 버튼에 연결할 함수
    public void QuitGame()
    {
        Debug.Log("[PauseManager] 게임 종료 (실제 빌드에서만 작동)");

        // Unity 에디터에서는 플레이 모드 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 애플리케이션 종료
            Application.Quit();
#endif
    }
}
