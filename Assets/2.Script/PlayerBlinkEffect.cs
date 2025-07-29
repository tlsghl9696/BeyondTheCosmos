using UnityEngine;
using System.Collections;

// PlayerBlinkEffect.cs ��ũ��Ʈ�� �÷��̾��� �ð����� ������ ȿ���� �����մϴ�.
// �� ��ũ��Ʈ�� playerBodyGameObject�� �Ҵ�� ���� �÷��̾� GameObject�� SpriteRenderer�� �����Ͽ�
// �� Material�� color �Ӽ��� �����Ͽ� ���� ����� ������ ȿ���� �����մϴ�.
public class PlayerBlinkEffect : MonoBehaviour
{
    // �÷��̾��� ��ü GameObject�� �Ҵ��մϴ�.
    // �� GameObject���� SpriteRenderer ������Ʈ�� �־�� �մϴ�.
    public GameObject playerBodyGameObject;

    private SpriteRenderer playerSpriteRenderer; // playerBodyGameObject���� ã�� SpriteRenderer
    private Color originalColor; // ��������Ʈ�� ���� ���� (���̴� ������ ����)
    private Color _currentBlinkColor; // ���� �����ӿ� ���� ���� (�ܺο��� ���޹���)

    public float blinkInterval = 0.1f; // ������ ���� (��)

    private Coroutine currentBlinkCoroutine; // ���� ���� ���� ������ �ڷ�ƾ ����

    void Awake()
    {
        // playerBodyGameObject�� �Ҵ���� �ʾҴٸ� ��� ���ϴ�.
        if (playerBodyGameObject == null)
        {
            Debug.LogWarning("[PlayerBlinkEffect] Player Body GameObject�� �Ҵ���� �ʾҽ��ϴ�. Inspector�� �Ҵ����ּ���.");
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        else
        {
            // �Ҵ�� playerBodyGameObject���� SpriteRenderer�� ã���ϴ�.
            playerSpriteRenderer = playerBodyGameObject.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer == null)
            {
                // playerBodyGameObject ��ü�� ������ �ڽ� ������Ʈ������ ã�ƺ��ϴ�.
                playerSpriteRenderer = playerBodyGameObject.GetComponentInChildren<SpriteRenderer>();
                if (playerSpriteRenderer == null)
                {
                    Debug.LogWarning($"[PlayerBlinkEffect] �Ҵ�� Player Body GameObject '{playerBodyGameObject.name}' �Ǵ� �� �ڽĿ��� SpriteRenderer�� ã�� �� �����ϴ�. ��Ȯ�� GameObject�� �Ҵ��ϰų� SpriteRenderer�� �ִ��� Ȯ�����ּ���.");
                }
            }
        }

        // SpriteRenderer�� ���������� ã�� ���ߴٸ� ������ ����ϰ� �������� �۵����� ������ �˸��ϴ�.
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("[PlayerBlinkEffect] SpriteRenderer�� ã�� �� �����ϴ�. ������ ȿ���� �۵����� �ʽ��ϴ�. �÷��̾� GameObject�� SpriteRenderer�� �ִ��� Ȯ���ϰų�, ��Ȯ�� Player Body GameObject�� �Ҵ����ּ���.");
        }
        else
        {
            // SpriteRenderer�� Material.color�� ���̴��� _Color �Ӽ��� �����մϴ�.
            // ���� ������ �����մϴ�.
            originalColor = playerSpriteRenderer.color;
            Debug.Log($"[PlayerBlinkEffect] Original Sprite Color: {originalColor}");
        }
    }

    // ������ ȿ���� �����ϴ� �Լ�. PlayerHealth ��ũ��Ʈ���� ȣ��˴ϴ�.
    // newBlinkColor ���ڸ� ���� ������ ������ �ܺο��� �����մϴ�.
    public void StartBlinking(float duration, Color newBlinkColor)
    {
        Debug.Log($"[PlayerBlinkEffect] StartBlinking ȣ��� (Material Color ������). ���� ���� �ð�: {duration:F2}��, �� ����: {newBlinkColor}. Time: {Time.time:F2}s");

        // ���� �ڷ�ƾ�� �ִٸ� �����ϰ� ��������Ʈ�� ���� ���� ������� �ǵ����ϴ�.
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            // ���� �������� �����Ǹ� ��������Ʈ�� ���� �������� ����
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.color = originalColor;
            }
            Debug.Log("[PlayerBlinkEffect] ���� ������ �ڷ�ƾ ���� ���� �� ���� �ʱ�ȭ.");
        }

        // SpriteRenderer�� ��ȿ�� ��쿡�� ������ �ڷ�ƾ�� �����մϴ�.
        if (playerSpriteRenderer != null)
        {
            _currentBlinkColor = newBlinkColor; // �����ӿ� ����� ���� ����
            currentBlinkCoroutine = StartCoroutine(BlinkEffect(duration));
            Debug.Log($"[PlayerBlinkEffect] ������ ȿ�� �ڷ�ƾ ���� ��û (Material Color ����). ���� �ð�: {duration}��. Time: {Time.time:F2}s");
        }
        else
        {
            Debug.LogWarning("[PlayerBlinkEffect] SpriteRenderer�� ���� ������ ȿ���� ������ �� �����ϴ�.");
        }
    }

    // ������ ȿ���� �����ϴ� �Լ�. PlayerHealth ��ũ��Ʈ���� ȣ��˴ϴ�.
    public void StopBlinking()
    {
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            currentBlinkCoroutine = null;
            Debug.Log("[PlayerBlinkEffect] StopBlinking ȣ���. �ڷ�ƾ ����.");
        }

        // ������ ���� �� �׻� ��������Ʈ�� ������ ������� (������ ���̰�) �ǵ����ϴ�.
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = originalColor;
            Debug.Log($"[PlayerBlinkEffect] StopBlinking: SpriteRenderer ���� ������� ����.");
        }
    }

    // ������ ������ ȿ���� �����ϴ� �ڷ�ƾ (���� �� ����)
    private IEnumerator BlinkEffect(float duration)
    {
        if (playerSpriteRenderer == null)
        {
            Debug.LogWarning("[PlayerBlinkEffect] SpriteRenderer�� ���� ������ ȿ���� ������ �� �����ϴ�.");
            yield break;
        }

        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;

        while (Time.unscaledTime < endTime)
        {
            // _currentBlinkColor�� originalColor ���̿��� ������ ����մϴ�.
            // Color.Equals�� ����Ͽ� �ε� �Ҽ��� ������ ����� ��Ȯ�� �񱳸� �����մϴ�.
            // ���̴��� _Color �Ӽ��� ���� �����մϴ�.
            if (playerSpriteRenderer.color.Equals(_currentBlinkColor)) // ���� ������ ������ ����� ������
            {
                playerSpriteRenderer.color = originalColor; // ���� ��������
            }
            else // ���� ������ ���� ����� ������ (�Ǵ� �ٸ� �����̸�)
            {
                playerSpriteRenderer.color = _currentBlinkColor; // ������ ��������
            }
            yield return new WaitForSeconds(blinkInterval); // ������ ���ݸ�ŭ ���
        }

        // �ڷ�ƾ ���� ������ �׻� ��������Ʈ�� ������ ������� (������ ���̰�) �����մϴ�.
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = originalColor;
        }
        Debug.Log($"[PlayerBlinkEffect] ���� �ð� ({duration:F2}��) �ʰ�. ���� ����. ���� SpriteRenderer ����: {playerSpriteRenderer.color}. Time: {Time.time:F2}s");
        currentBlinkCoroutine = null; // �ڷ�ƾ�� �Ϸ�Ǿ����� ǥ��
    }
}
