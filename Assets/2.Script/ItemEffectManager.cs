// ItemEffectManager.cs ���� ����
// - ������ ȿ������ ���ӽð� ���� ����
// - ItemEffectUI ����

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ItemType
{
    ������_��,   // ������ - ������ 3��
    ����, // ����� - ����
    ����        // �ʷϻ� - ü�� ȸ�� (��� ����)
}

public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Instance;
    public ItemEffectUI itemEffectUI; // [�߰�] UI ǥ�ÿ� ����
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

    // [����] ���ӽð��� ���ڷ� �޴� ApplyItemEffect
    public void ApplyItemEffect(ItemType type, float duration, GameObject player)
    {
        Debug.Log($"[ItemEffectManager] ȿ�� ���� ��û: {type} ({duration:F1}s)");
        GameObject setup = Instantiate(ItemTextprefab);
        setup.GetComponent<Text>().text = $"{type} ({duration:F1}s)";
        setup.transform.SetParent(GameManager.instance.itemPanel.transform);
        Destroy(setup, 2f);
        switch (type)
        {

            case ItemType.������_��:
                StartCoroutine(ApplyDamageBoost(player, duration));
                break;
            case ItemType.����:
                // [����] PlayerHealth�� TriggerItemInvincibility ȣ��
                PlayerHealth hp = player.GetComponent<PlayerHealth>();
                if (hp != null)
                {
                    hp.TriggerItemInvincibility(duration); // ���ο� ������ ���� �Լ� ȣ��
                    Debug.Log("[ItemEffectManager] �÷��̾�� ������ ���� ȿ�� ��û��.");
                }
                break;
            case ItemType.����:
                HealPlayer(player);
                break;
        }

        // [�߰�] UI ǥ�� ȣ��
        if (itemEffectUI != null && type != ItemType.����)
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

    // [����] ApplyInvincibility �ڷ�ƾ�� ���� ItemEffectManager���� ���� ó������ �ʽ��ϴ�.
    // ��� PlayerHealth�� TriggerItemInvincibility�� ȣ���մϴ�.
    // private IEnumerator ApplyInvincibility(GameObject player, float duration)
    // {
    //     PlayerHealth hp = player.GetComponent<PlayerHealth>();
    //     if (hp != null)
    //     {
    //         hp.isInvincible = true; // �� �κ��� PlayerHealth���� ���� ����
    //         yield return new WaitForSeconds(duration);
    //         hp.isInvincible = false; // �� �κе� PlayerHealth���� ���� ����
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