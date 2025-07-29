using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다.
using UnityEngine.UI; // UI 컴포넌트를 사용하기 위해 필요합니다.

#if UNITY_EDITOR // Unity 에디터에서만 EditorApplication.isPlaying을 사용합니다.
using UnityEditor;
#endif

public class TitleScreenManager : MonoBehaviour
{
    // 게임 시작 버튼 (Inspector에서 할당)
    public Button startGameButton;
    // 게임 종료 버튼 (선택 사항, Inspector에서 할당)
    public Button exitGameButton;

    // 메인 게임 씬의 이름 (Build Settings에 등록된 씬 이름과 일치해야 합니다)
    public string mainGameSceneName = "MainGameScene"; // 실제 게임 씬 이름으로 변경하세요.

    void Awake()
    {
        // 게임 시작 버튼 이벤트 리스너 추가
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
            Debug.Log("[TitleScreenManager] 'Start Game' 버튼 리스너 추가됨.");
        }
        else
        {
            Debug.LogWarning("[TitleScreenManager] 'Start Game' 버튼이 할당되지 않았습니다. Inspector에서 할당해주세요.");
        }

        // 게임 종료 버튼 이벤트 리스너 추가 (선택 사항)
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(ExitGame);
            Debug.Log("[TitleScreenManager] 'Exit Game' 버튼 리스너 추가됨.");
        }
        else
        {
            Debug.LogWarning("[TitleScreenManager] 'Exit Game' 버튼이 할당되지 않았습니다. Inspector에서 할당해주세요.");
        }
    }

    // 게임 시작 버튼 클릭 시 호출될 함수
    public void StartGame()
    {
        Debug.Log("[TitleScreenManager] 게임 시작!");
        // 게임 시간을 다시 1로 설정 (혹시 이전 씬에서 Time.timeScale이 0이었다면)
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainGameSceneName); // 지정된 메인 게임 씬으로 로드합니다.
    }

    // 게임 종료 버튼 클릭 시 호출될 함수
    public void ExitGame()
    {
        Debug.Log("[TitleScreenManager] 게임 종료!");
        Application.Quit(); // 실제 빌드된 게임을 종료합니다.

#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때 플레이 모드를 종료합니다.
        EditorApplication.isPlaying = false;
#endif
    }
}
