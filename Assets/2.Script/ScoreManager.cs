using UnityEngine;
using TMPro; // TextMeshPro ����� ���� �ʿ�

public class ScoreManager : MonoBehaviour
{
    // ScoreManager�� ���� �ν��Ͻ��� �����մϴ�.
    public static ScoreManager instance;
    // ���� �÷��̾��� �����Դϴ�.
    public int currentScore = 0;
    // UI�� ������ ǥ���� TextMeshProUGUI ������Ʈ�Դϴ�.
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        // �̱��� ���� ����: ���� �ϳ��� �ν��Ͻ��� �����ϵ��� �մϴ�.
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // �� ���� �����Ͽ� �� ����� �� �Բ� �ı��ǵ��� �մϴ�.
            // ���� ��ε�� �� ScoreManager�� ���� �����Ǿ� ScoreText�� �ٽ� ã�� �� �ֵ��� �մϴ�.
        }
        else
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ������ �� ������Ʈ�� �ı��մϴ�.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ���� ���� �� ���� �ؽ�Ʈ�� ���� �õ��մϴ�.
        TryFindScoreText();
        // UI�� ���� ������ �ݿ��մϴ�.
        UpdateScoreUI();
    }

    // ���� �ؽ�Ʈ UI�� ã�� �����ϴ� �Լ��Դϴ�.
    void TryFindScoreText()
    {
        // Inspector���� scoreText�� �Ҵ���� �ʾҴٸ� ������ TextMeshProUGUI ������Ʈ�� ã���ϴ�.
        if (scoreText == null)
        {
            scoreText = FindObjectOfType<TextMeshProUGUI>();

            if (scoreText != null)
            {
                Debug.Log("[ScoreManager] scoreText �ڵ� ���� ����: " + scoreText.gameObject.name);
            }
            else
            {
                Debug.LogWarning("[ScoreManager] TextMeshProUGUI�� ã�� �� �����ϴ�. ���� UI�� ǥ�õ��� �ʽ��ϴ�.");
            }
        }
    }

    // ������ �߰��ϰ� UI�� ������Ʈ�ϴ� �Լ��Դϴ�.
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
        Debug.Log("���� ����: " + currentScore);
    }

    // UI�� ������ ������Ʈ�ϴ� �Լ��Դϴ�.
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    // ������ �ʱ�ȭ�ϰ� UI�� ������Ʈ�ϴ� �Լ��Դϴ�.
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        Debug.Log("���� �ʱ�ȭ��.");
    }
}
