using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro�� ����Ѵٸ� �߰�

#if UNITY_EDITOR // �����Ϳ����� ���Ǵ� ���
using UnityEditor;
#endif

public class GameUIController : MonoBehaviour
{
    public GameObject gameCompletePanel; // ���� �Ϸ� �г� GameObject
    public string titleSceneName = "TitleScene"; // Ÿ��Ʋ �� �̸� (Build Settings�� �߰��Ǿ�� ��)
    public string gameSceneName = "MainGameScene"; // ���� ���� �� �̸� (Build Settings�� �߰��Ǿ�� ��)

    void Start()
    {
        // ���� �� ���� �Ϸ� �г��� ��Ȱ��ȭ
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(false);
        }
    }

    void Update()
    {
        // R Ű�� ������ ���� �����
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // E Ű�� ������ ���� ����
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitGame();
        }
    }

    // ����HP ��ũ��Ʈ���� ȣ���� �Լ�
    public void ShowGameCompletePanel()
    {
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
            Debug.Log("���� �Ϸ� �г� Ȱ��ȭ!");
            // ���� �Ϸ� �� �ð� ���� (���� ����)
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // �ð� ���� ����
        // ���� Ȱ��ȭ�� ���� �ٽ� �ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // �Ǵ� Ư�� ���� ������ �ε�: SceneManager.LoadScene(gameSceneName);
    }

    public void GoToTitle()
    {
        Time.timeScale = 1f; // �ð� ���� ����
        SceneManager.LoadScene(titleSceneName);
    }

    public void ExitGame()
    {
        Application.Quit(); // ���� ����� ���� ����

#if UNITY_EDITOR
        // ����Ƽ �����Ϳ��� ���� ���� �� �÷��� ��� ����
        EditorApplication.isPlaying = false;
#endif
    }
}