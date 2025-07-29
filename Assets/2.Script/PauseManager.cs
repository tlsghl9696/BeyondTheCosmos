using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� �ٷ�� ���� �ʿ�
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �ʿ� (���� ���� ��)

public class PauseManager : MonoBehaviour
{
    // �Ͻ����� UI �г��� Inspector���� ������ �� �ֵ��� public���� ����
    public GameObject pausePanel;

    // ������ ���� �Ͻ����� �������� �����ϴ� ����
    private bool isGamePaused = false;

    void Start()
    {
        // ���� ���� �� �Ͻ����� �г��� ��Ȱ��ȭ ���¿��� ��
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[PauseManager] Pause Panel�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �Ҵ����ּ���.");
        }
    }

    // ESC Ű �Է� ������ ���� Update �Լ� ���
    void Update()
    {
        // ESC Ű�� ������ �� �Ͻ�����/�簳 ���¸� ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // �Ͻ����� ���¸� ����ϴ� ���� �Լ� (��ư Ŭ��, ESC Ű � ����)
    public void TogglePause()
    {
        isGamePaused = !isGamePaused; // ���� ���� ����

        if (isGamePaused)
        {
            // ���� �Ͻ�����
            Time.timeScale = 0f; // ���� �� ��� �ð� �帧�� ����
            if (pausePanel != null)
            {
                pausePanel.SetActive(true); // �Ͻ����� �г� Ȱ��ȭ
            }
            Debug.Log("[PauseManager] ���� �Ͻ�������. Time.timeScale = 0");
        }
        else
        {
            // ���� �簳
            Time.timeScale = 1f; // �ð� �帧�� �������� ����
            if (pausePanel != null)
            {
                pausePanel.SetActive(false); // �Ͻ����� �г� ��Ȱ��ȭ
            }
            Debug.Log("[PauseManager] ���� �簳��. Time.timeScale = 1");
        }
    }

    // �Ͻ����� �г��� "����ϱ�" ��ư�� ������ �Լ�
    public void ResumeGame()
    {
        // �̹� �Ͻ����� ���¶�� TogglePause�� ȣ���Ͽ� �簳
        if (isGamePaused)
        {
            TogglePause();
        }
    }

    // �Ͻ����� �г��� "����" ��ư�� ������ �Լ� (����)
    public void OpenSettings()
    {
        Debug.Log("[PauseManager] ���� â ���� (���� �ʿ�)");
        // ���⿡ ���� UI�� Ȱ��ȭ�ϴ� ���� �߰�
    }

    // �Ͻ����� �г��� "���� ����" ��ư�� ������ �Լ�
    public void QuitGame()
    {
        Debug.Log("[PauseManager] ���� ���� (���� ���忡���� �۵�)");

        // Unity �����Ϳ����� �÷��� ��� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ����� ���ӿ����� ���ø����̼� ����
            Application.Quit();
#endif
    }
}
