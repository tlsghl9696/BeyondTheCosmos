// ItemEffectUI.cs ���� ����
// - ��ø ������ ������ ȿ�� UI ������ �������� ���� �� ����
// - �ؽ�Ʈ�� �ǽð� ���� �ð� ǥ��
// - Heal ȿ���� ����

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemEffectUI : MonoBehaviour
{
    [Header("UI ���� �����հ� �θ� �г�")]
    public GameObject effectSlotPrefab;         // ȿ�� ���� ������ (�����̴� + �ؽ�Ʈ)
    public Transform effectSlotParent;          // ���Ե��� ���� �θ� (ex. Vertical Layout)

    // ���� ���� ������
    private class EffectSlot
    {
        public GameObject slotObject;
        public Slider slider;
        public Text label;
        public Coroutine timerCoroutine;
        public string effectName;
    }

    private List<EffectSlot> activeSlots = new List<EffectSlot>();

    /// <summary>
    /// ���ο� ȿ�� ���� ���� �� ǥ�� ����
    /// </summary>
    public void CreateEffectSlot(string effectName, float duration)
    {
        GameObject newSlot = Instantiate(effectSlotPrefab, effectSlotParent);

        Slider slider = newSlot.GetComponentInChildren<Slider>();
        Text label = newSlot.GetComponentInChildren<Text>();

        EffectSlot slot = new EffectSlot
        {
            slotObject = newSlot,
            slider = slider,
            label = label,
            timerCoroutine = null,
            effectName = effectName
        };

        if (slider != null)
        {
            slider.maxValue = duration;
            slider.value = duration;
        }

        if (label != null)
        {
            label.text = $"{effectName} ({duration:F1}s)";
        }

        slot.timerCoroutine = StartCoroutine(Countdown(slot, duration));

        activeSlots.Add(slot);
    }

    private IEnumerator Countdown(EffectSlot slot, float duration)
    {
        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (slot.slider != null)
                slot.slider.value = timer;

            if (slot.label != null)
                slot.label.text = $"{slot.effectName} ({timer:F1}s)";

            yield return null;
        }

        if (slot != null)
        {
            activeSlots.Remove(slot);
            Destroy(slot.slotObject);
        }
    }
}
