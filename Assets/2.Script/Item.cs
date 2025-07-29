using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public ItemType itemType;              // ItemEffectManager에서 정의한 아이템 종류
    public float lifeTime = 5f;            // 화면에 남아있는 시간
    public float effectDuration = 5f;      // [추가] 효과 지속 시간 (UI 및 기능에 사용)

    private void Start()
    {
        transform.localScale = Vector3.one * 0.2f;
        Destroy(gameObject, lifeTime);     // 일정 시간 후 아이템 삭제
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[Item.cs] 충돌한 태그: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("[Item.cs] 플레이어가 아이템 습득! 타입: " + itemType);

            ApplyEffect(collision.gameObject);
            Destroy(gameObject); // 아이템은 1회용이므로 제거
        }
    }

    private void ApplyEffect(GameObject player)
    {
        Debug.Log("[Item.cs] 아이템 효과 적용 시도 - 타입: " + itemType);

        if (ItemEffectManager.Instance == null)
        {
            Debug.LogError("[Item.cs] ItemEffectManager 인스턴스가 null입니다. 씬에 배치되어 있는지 확인하세요.");
            return;
        }

        // [변경] 아이템 타입과 지속시간을 함께 전달
        ItemEffectManager.Instance.ApplyItemEffect(itemType, effectDuration, player);
    }
}
