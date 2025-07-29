using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �ʿ��մϴ�.
using UnityEngine.UI; // UI ������Ʈ�� ����ϱ� ���� �ʿ��մϴ�.

#if UNITY_EDITOR // Unity �����Ϳ����� EditorApplication.isPlaying�� ����մϴ�.
using UnityEditor;
#endif

public class TitleScreenManager : MonoBehaviour
{
    // ���� ���� ��ư (Inspector���� �Ҵ�)
    public Button startGameButton;
    // ���� ���� ��ư (���� ����, Inspector���� �Ҵ�)
    public Button exitGameButton;

    // ���� ���� ���� �̸� (Build Settings�� ��ϵ� �� �̸��� ��ġ�ؾ� �մϴ�)
    public string mainGameSceneName = "MainGameScene"; // ���� ���� �� �̸����� �����ϼ���.

    void Awake()
    {
        // ���� ���� ��ư �̺�Ʈ ������ �߰�
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
            Debug.Log("[TitleScreenManager] 'Start Game' ��ư ������ �߰���.");
        }
        else
        {
            Debug.LogWarning("[TitleScreenManager] 'Start Game' ��ư�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �Ҵ����ּ���.");
        }

        // ���� ���� ��ư �̺�Ʈ ������ �߰� (���� ����)
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(ExitGame);
            Debug.Log("[TitleScreenManager] 'Exit Game' ��ư ������ �߰���.");
        }
        else
        {
            Debug.LogWarning("[TitleScreenManager] 'Exit Game' ��ư�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �Ҵ����ּ���.");
        }
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��� �Լ�
    public void StartGame()
    {
        Debug.Log("[TitleScreenManager] ���� ����!");
        // ���� �ð��� �ٽ� 1�� ���� (Ȥ�� ���� ������ Time.timeScale�� 0�̾��ٸ�)
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainGameSceneName); // ������ ���� ���� ������ �ε��մϴ�.
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��� �Լ�
    public void ExitGame()
    {
        Debug.Log("[TitleScreenManager] ���� ����!");
        Application.Quit(); // ���� ����� ������ �����մϴ�.

#if UNITY_EDITOR
        // ����Ƽ �����Ϳ��� ���� ���� �� �÷��� ��带 �����մϴ�.
        EditorApplication.isPlaying = false;
#endif
    }
}
