using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용한다면 추가

#if UNITY_EDITOR // 에디터에서만 사용되는 기능
using UnityEditor;
#endif

public class GameUIController : MonoBehaviour
{
    public GameObject gameCompletePanel; // 게임 완료 패널 GameObject
    public string titleSceneName = "TitleScene"; // 타이틀 씬 이름 (Build Settings에 추가되어야 함)
    public string gameSceneName = "MainGameScene"; // 현재 게임 씬 이름 (Build Settings에 추가되어야 함)

    void Start()
    {
        // 시작 시 게임 완료 패널은 비활성화
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(false);
        }
    }

    void Update()
    {
        // R 키를 누르면 게임 재시작
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // E 키를 누르면 게임 종료
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitGame();
        }
    }

    // 보스HP 스크립트에서 호출할 함수
    public void ShowGameCompletePanel()
    {
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
            Debug.Log("게임 완료 패널 활성화!");
            // 게임 완료 시 시간 정지 (선택 사항)
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 시간 정지 해제
        // 현재 활성화된 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // 또는 특정 게임 씬으로 로드: SceneManager.LoadScene(gameSceneName);
    }

    public void GoToTitle()
    {
        Time.timeScale = 1f; // 시간 정지 해제
        SceneManager.LoadScene(titleSceneName);
    }

    public void ExitGame()
    {
        Application.Quit(); // 실제 빌드된 게임 종료

#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때 플레이 모드 종료
        EditorApplication.isPlaying = false;
#endif
    }
}