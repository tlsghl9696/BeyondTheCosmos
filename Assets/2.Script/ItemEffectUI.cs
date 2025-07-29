// ItemEffectUI.cs 개편 버전
// - 중첩 가능한 아이템 효과 UI 슬롯을 동적으로 생성 및 관리
// - 텍스트에 실시간 남은 시간 표시
// - Heal 효과는 제외

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemEffectUI : MonoBehaviour
{
    [Header("UI 슬롯 프리팹과 부모 패널")]
    public GameObject effectSlotPrefab;         // 효과 슬롯 프리팹 (슬라이더 + 텍스트)
    public Transform effectSlotParent;          // 슬롯들이 붙을 부모 (ex. Vertical Layout)

    // 내부 슬롯 관리용
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
    /// 새로운 효과 슬롯 생성 및 표시 시작
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
