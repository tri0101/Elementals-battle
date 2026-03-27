using System.Collections;
using TMPro;
using UnityEngine;

public class UI_CanvasTotalDamage : MonoBehaviour
{
    [SerializeField] private Transform damageAndText;
    [SerializeField] private TextMeshProUGUI totalDamageText;

    [Header("Runtime")]
    [SerializeField] private int totalDamage;
    [SerializeField] private bool isShowing;
    public bool IsShowing => isShowing; // chỉ để debug, tránh set trực tiếp
    public int TotalDamage
    {
        get => totalDamage;
        set => totalDamage = value;
    }

    [Header("Anim")]
    [Min(0f)]
    [SerializeField] private float countUpDuration = 0.1f;

    private Coroutine countRoutine;
    private int currentDisplayed;

    private void Awake()
    {
        // đảm bảo text đúng khi mới vào scene
        currentDisplayed = totalDamage;
        if (totalDamageText != null)
            totalDamageText.text = currentDisplayed.ToString();
    }

    // API cũ: gọi từ HeroReceiveDamagee.SetCanShowTotalDmg()
    public void UpdateTotalDamage()
    {
        AnimateTo(totalDamage);
    }

    // Nếu chỗ khác vẫn muốn set trực tiếp
    public void UpdateTotalDamage(float totalDamageValue)
    {
        totalDamage = Mathf.RoundToInt(totalDamageValue);
        AnimateTo(totalDamage);
    }

    private void AnimateTo(int targetValue)
    {
        if (totalDamageText == null)
            return;

        if (countRoutine != null)
            StopCoroutine(countRoutine);

        countRoutine = StartCoroutine(CoCountUp(currentDisplayed, targetValue, countUpDuration));
    }

    private IEnumerator CoCountUp(int from, int to, float duration)
    {
        if (duration <= 0f || from == to)
        {
            currentDisplayed = to;
            totalDamageText.text = currentDisplayed.ToString();
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // UI thường nên dùng unscaled
            float k = Mathf.Clamp01(t / duration);

            currentDisplayed = Mathf.RoundToInt(Mathf.Lerp(from, to, k));
            totalDamageText.text = currentDisplayed.ToString();

            yield return null;
        }

        currentDisplayed = to;
        totalDamageText.text = currentDisplayed.ToString();
    }

    public void Show()
    {
        if (damageAndText != null)
        {
            isShowing = true;
            damageAndText.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (damageAndText != null)
        {
            isShowing = false;
            damageAndText.gameObject.SetActive(false);
        }
           
    }
}