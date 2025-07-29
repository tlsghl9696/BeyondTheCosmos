// ItemEffectManager.cs 개편 버전
// - 아이템 효과마다 지속시간 전달 받음
// - ItemEffectUI 연동

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ItemType
{
    데미지_업,   // 붉은색 - 데미지 3배
    무적, // 노란색 - 무적
    수리        // 초록색 - 체력 회복 (즉시 적용)
}

public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Instance;
    public ItemEffectUI itemEffectUI; // [추가] UI 표시용 참조
    public GameObject ItemTextprefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // [변경] 지속시간을 인자로 받는 ApplyItemEffect
    public void ApplyItemEffect(ItemType type, float duration, GameObject player)
    {
        Debug.Log($"[ItemEffectManager] 효과 적용 요청: {type} ({duration:F1}s)");
        GameObject setup = Instantiate(ItemTextprefab);
        setup.GetComponent<Text>().text = $"{type} ({duration:F1}s)";
        setup.transform.SetParent(GameManager.instance.itemPanel.transform);
        Destroy(setup, 2f);
        switch (type)
        {

            case ItemType.데미지_업:
                StartCoroutine(ApplyDamageBoost(player, duration));
                break;
            case ItemType.무적:
                // [수정] PlayerHealth의 TriggerItemInvincibility 호출
                PlayerHealth hp = player.GetComponent<PlayerHealth>();
                if (hp != null)
                {
                    hp.TriggerItemInvincibility(duration); // 새로운 아이템 무적 함수 호출
                    Debug.Log("[ItemEffectManager] 플레이어에게 아이템 무적 효과 요청됨.");
                }
                break;
            case ItemType.수리:
                HealPlayer(player);
                break;
        }

        // [추가] UI 표시 호출
        if (itemEffectUI != null && type != ItemType.수리)
        {
            itemEffectUI.CreateEffectSlot(type.ToString(), duration);
        }
    }

    private IEnumerator ApplyDamageBoost(GameObject player, float duration)
    {
        PlayerAttack attack = player.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.damageMultiplier = 3f;
            yield return new WaitForSeconds(duration);
            attack.damageMultiplier = 1f;
        }
    }

    // [삭제] ApplyInvincibility 코루틴은 이제 ItemEffectManager에서 직접 처리하지 않습니다.
    // 대신 PlayerHealth의 TriggerItemInvincibility를 호출합니다.
    // private IEnumerator ApplyInvincibility(GameObject player, float duration)
    // {
    //     PlayerHealth hp = player.GetComponent<PlayerHealth>();
    //     if (hp != null)
    //     {
    //         hp.isInvincible = true; // 이 부분은 PlayerHealth에서 직접 관리
    //         yield return new WaitForSeconds(duration);
    //         hp.isInvincible = false; // 이 부분도 PlayerHealth에서 직접 관리
    //     }
    // }

    private void HealPlayer(GameObject player)
    {
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.RestoreHP(1);
        }
    }
}