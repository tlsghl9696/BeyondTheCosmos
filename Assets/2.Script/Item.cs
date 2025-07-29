using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public ItemType itemType;              // ItemEffectManager���� ������ ������ ����
    public float lifeTime = 5f;            // ȭ�鿡 �����ִ� �ð�
    public float effectDuration = 5f;      // [�߰�] ȿ�� ���� �ð� (UI �� ��ɿ� ���)

    private void Start()
    {
        transform.localScale = Vector3.one * 0.2f;
        Destroy(gameObject, lifeTime);     // ���� �ð� �� ������ ����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[Item.cs] �浹�� �±�: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("[Item.cs] �÷��̾ ������ ����! Ÿ��: " + itemType);

            ApplyEffect(collision.gameObject);
            Destroy(gameObject); // �������� 1ȸ���̹Ƿ� ����
        }
    }

    private void ApplyEffect(GameObject player)
    {
        Debug.Log("[Item.cs] ������ ȿ�� ���� �õ� - Ÿ��: " + itemType);

        if (ItemEffectManager.Instance == null)
        {
            Debug.LogError("[Item.cs] ItemEffectManager �ν��Ͻ��� null�Դϴ�. ���� ��ġ�Ǿ� �ִ��� Ȯ���ϼ���.");
            return;
        }

        // [����] ������ Ÿ�԰� ���ӽð��� �Բ� ����
        ItemEffectManager.Instance.ApplyItemEffect(itemType, effectDuration, player);
    }
}
