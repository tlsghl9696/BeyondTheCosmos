using UnityEngine;
using TMPro; // TextMeshPro 사용을 위해 필요

public class ScoreManager : MonoBehaviour
{
    // ScoreManager의 단일 인스턴스를 유지합니다.
    public static ScoreManager instance;
    // 현재 플레이어의 점수입니다.
    public int currentScore = 0;
    // UI에 점수를 표시할 TextMeshProUGUI 컴포넌트입니다.
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        // 싱글톤 패턴 구현: 씬에 하나의 인스턴스만 존재하도록 합니다.
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // 이 줄을 제거하여 씬 재시작 시 함께 파괴되도록 합니다.
            // 씬이 재로드될 때 ScoreManager도 새로 생성되어 ScoreText를 다시 찾을 수 있도록 합니다.
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생성된 이 오브젝트를 파괴합니다.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 점수 텍스트를 연결 시도합니다.
        TryFindScoreText();
        // UI에 현재 점수를 반영합니다.
        UpdateScoreUI();
    }

    // 점수 텍스트 UI를 찾아 연결하는 함수입니다.
    void TryFindScoreText()
    {
        // Inspector에서 scoreText가 할당되지 않았다면 씬에서 TextMeshProUGUI 컴포넌트를 찾습니다.
        if (scoreText == null)
        {
            scoreText = FindObjectOfType<TextMeshProUGUI>();

            if (scoreText != null)
            {
                Debug.Log("[ScoreManager] scoreText 자동 연결 성공: " + scoreText.gameObject.name);
            }
            else
            {
                Debug.LogWarning("[ScoreManager] TextMeshProUGUI를 찾을 수 없습니다. 점수 UI가 표시되지 않습니다.");
            }
        }
    }

    // 점수를 추가하고 UI를 업데이트하는 함수입니다.
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
        Debug.Log("현재 점수: " + currentScore);
    }

    // UI에 점수를 업데이트하는 함수입니다.
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    // 점수를 초기화하고 UI를 업데이트하는 함수입니다.
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        Debug.Log("점수 초기화됨.");
    }
}
