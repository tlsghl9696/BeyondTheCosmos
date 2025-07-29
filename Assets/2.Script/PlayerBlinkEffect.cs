using UnityEngine;
using System.Collections;

// PlayerBlinkEffect.cs 스크립트는 플레이어의 시각적인 깜빡임 효과를 제어합니다.
// 이 스크립트는 playerBodyGameObject에 할당된 실제 플레이어 GameObject의 SpriteRenderer에 접근하여
// 그 Material의 color 속성을 조작하여 색상 기반의 깜빡임 효과를 구현합니다.
public class PlayerBlinkEffect : MonoBehaviour
{
    // 플레이어의 본체 GameObject를 할당합니다.
    // 이 GameObject에는 SpriteRenderer 컴포넌트가 있어야 합니다.
    public GameObject playerBodyGameObject;

    private SpriteRenderer playerSpriteRenderer; // playerBodyGameObject에서 찾을 SpriteRenderer
    private Color originalColor; // 스프라이트의 원래 색상 (쉐이더 재질의 색상)
    private Color _currentBlinkColor; // 현재 깜빡임에 사용될 색상 (외부에서 전달받음)

    public float blinkInterval = 0.1f; // 깜빡임 간격 (초)

    private Coroutine currentBlinkCoroutine; // 현재 실행 중인 깜빡임 코루틴 참조

    void Awake()
    {
        // playerBodyGameObject가 할당되지 않았다면 경고를 띄웁니다.
        if (playerBodyGameObject == null)
        {
            Debug.LogWarning("[PlayerBlinkEffect] Player Body GameObject가 할당되지 않았습니다. Inspector에 할당해주세요.");
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        else
        {
            // 할당된 playerBodyGameObject에서 SpriteRenderer를 찾습니다.
            playerSpriteRenderer = playerBodyGameObject.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer == null)
            {
                // playerBodyGameObject 자체에 없으면 자식 오브젝트에서도 찾아봅니다.
                playerSpriteRenderer = playerBodyGameObject.GetComponentInChildren<SpriteRenderer>();
                if (playerSpriteRenderer == null)
                {
                    Debug.LogWarning($"[PlayerBlinkEffect] 할당된 Player Body GameObject '{playerBodyGameObject.name}' 또는 그 자식에서 SpriteRenderer를 찾을 수 없습니다. 정확한 GameObject를 할당하거나 SpriteRenderer가 있는지 확인해주세요.");
                }
            }
        }

        // SpriteRenderer를 최종적으로 찾지 못했다면 오류를 기록하고 깜빡임이 작동하지 않음을 알립니다.
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("[PlayerBlinkEffect] SpriteRenderer를 찾을 수 없습니다. 깜빡임 효과가 작동하지 않습니다. 플레이어 GameObject에 SpriteRenderer가 있는지 확인하거나, 정확한 Player Body GameObject를 할당해주세요.");
        }
        else
        {
            // SpriteRenderer의 Material.color는 쉐이더의 _Color 속성을 제어합니다.
            // 원래 색상을 저장합니다.
            originalColor = playerSpriteRenderer.color;
            Debug.Log($"[PlayerBlinkEffect] Original Sprite Color: {originalColor}");
        }
    }

    // 깜빡임 효과를 시작하는 함수. PlayerHealth 스크립트에서 호출됩니다.
    // newBlinkColor 인자를 통해 깜빡일 색상을 외부에서 지정합니다.
    public void StartBlinking(float duration, Color newBlinkColor)
    {
        Debug.Log($"[PlayerBlinkEffect] StartBlinking 호출됨 (Material Color 깜빡임). 받은 지속 시간: {duration:F2}초, 새 색상: {newBlinkColor}. Time: {Time.time:F2}s");

        // 기존 코루틴이 있다면 중지하고 스프라이트의 색상 값을 원래대로 되돌립니다.
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            // 기존 깜빡임이 중지되면 스프라이트를 원래 색상으로 복구
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.color = originalColor;
            }
            Debug.Log("[PlayerBlinkEffect] 기존 깜빡임 코루틴 강제 중지 및 색상 초기화.");
        }

        // SpriteRenderer가 유효한 경우에만 깜빡임 코루틴을 시작합니다.
        if (playerSpriteRenderer != null)
        {
            _currentBlinkColor = newBlinkColor; // 깜빡임에 사용할 색상 설정
            currentBlinkCoroutine = StartCoroutine(BlinkEffect(duration));
            Debug.Log($"[PlayerBlinkEffect] 깜빡임 효과 코루틴 시작 요청 (Material Color 제어). 지속 시간: {duration}초. Time: {Time.time:F2}s");
        }
        else
        {
            Debug.LogWarning("[PlayerBlinkEffect] SpriteRenderer가 없어 깜빡임 효과를 시작할 수 없습니다.");
        }
    }

    // 깜빡임 효과를 중지하는 함수. PlayerHealth 스크립트에서 호출됩니다.
    public void StopBlinking()
    {
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            currentBlinkCoroutine = null;
            Debug.Log("[PlayerBlinkEffect] StopBlinking 호출됨. 코루틴 중지.");
        }

        // 깜빡임 중지 시 항상 스프라이트의 색상을 원래대로 (완전히 보이게) 되돌립니다.
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = originalColor;
            Debug.Log($"[PlayerBlinkEffect] StopBlinking: SpriteRenderer 색상 원래대로 복구.");
        }
    }

    // 실제로 깜빡임 효과를 구현하는 코루틴 (색상 값 조절)
    private IEnumerator BlinkEffect(float duration)
    {
        if (playerSpriteRenderer == null)
        {
            Debug.LogWarning("[PlayerBlinkEffect] SpriteRenderer가 없어 깜빡임 효과를 시작할 수 없습니다.");
            yield break;
        }

        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;

        while (Time.unscaledTime < endTime)
        {
            // _currentBlinkColor와 originalColor 사이에서 색상을 토글합니다.
            // Color.Equals를 사용하여 부동 소수점 오차를 고려한 정확한 비교를 수행합니다.
            // 쉐이더의 _Color 속성을 직접 변경합니다.
            if (playerSpriteRenderer.color.Equals(_currentBlinkColor)) // 현재 색상이 깜빡임 색상과 같으면
            {
                playerSpriteRenderer.color = originalColor; // 원래 색상으로
            }
            else // 현재 색상이 원래 색상과 같으면 (또는 다른 색상이면)
            {
                playerSpriteRenderer.color = _currentBlinkColor; // 깜빡임 색상으로
            }
            yield return new WaitForSeconds(blinkInterval); // 설정된 간격만큼 대기
        }

        // 코루틴 종료 시점에 항상 스프라이트의 색상을 원래대로 (완전히 보이게) 보장합니다.
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = originalColor;
        }
        Debug.Log($"[PlayerBlinkEffect] 지속 시간 ({duration:F2}초) 초과. 루프 종료. 최종 SpriteRenderer 색상: {playerSpriteRenderer.color}. Time: {Time.time:F2}s");
        currentBlinkCoroutine = null; // 코루틴이 완료되었음을 표시
    }
}
