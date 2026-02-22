using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public enum HPNotifyType
{
    HPMinus, 
    HPPlus
}
public enum DamageType // để phân biệt kiểu damage (normal, crit, block)
{
    critDamage,
    blockDamage,
    normalDamage
}

public class HeroUI : MonoBehaviour, IObserver
{
    [SerializeField] HeroControl heroControl;

    public HeroControl HeroControl => heroControl;
    [Header("Bars")]
    public Image hpBar;
    public Image manaBar;
    public TextMeshProUGUI damageTextPrefab;
    public Transform listDamage;
    // ===== Bar Animation =====
    [SerializeField] private float barAnimDuration = 0.25f;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material critMaterial;
    private float currentHp01 = -1f;
    private float currentMana01 = -1f;

    private Coroutine hpRoutine;
    private Coroutine manaRoutine;


    private void Awake()
    {
        heroControl = transform.parent.GetComponent<HeroControl>();

        heroControl.AddObserver(this);
        hpBar = transform.Find("HP").GetChild(1).GetComponent<Image>();
        manaBar = transform.Find("Mana").GetChild(1).GetComponent<Image>();
        listDamage = transform.Find("ListDamage");
        SetHpBar(1f, true);
        SetManaBar(0f, true);
    }
    public void OnNotify(HeroNotifyType type, object value)
    {
        switch (type)
        {
            case HeroNotifyType.HPChanged:
                SetHpBar((float)value);
                break;

            case HeroNotifyType.ManaChanged:
                SetManaBar((float)value);
                break;

            case HeroNotifyType.Dead:
                SetHpBar(0f, true);
               
                break;

            case HeroNotifyType.Revive:
                SetHpBar(1f, true);
                
                break;
        }

      
    }
    public void OnNotify(HPNotifyType type, object value,DamageType damageType )
    {
        switch (type)
        {
            case HPNotifyType.HPMinus:
                SpawnDamageText((int)value, damageType);
                break;

            
        }

      
    }

    // ================= FLOATING TEXT =================
    private void SpawnDamageText(int value, DamageType damageType)
    {
        if (damageTextPrefab == null || value <= 0) return;

        TextMeshProUGUI text =
            Instantiate(damageTextPrefab, listDamage);


       

        switch (damageType)
        {
            case DamageType.critDamage:
                text.text = "CRIT-" + value.ToString();
                text.color = new Color32(253, 255, 0, 255);
                text.fontSize = 15;
                text.fontSharedMaterial = critMaterial;
                break;

            case DamageType.normalDamage:
            default:
                text.text = value.ToString();
                text.color = new Color32(211, 71, 35, 255);
                text.fontSize = 12;
                text.fontSharedMaterial = normalMaterial;
                break;
        }

        StartCoroutine(CoFloatAndFade(text));
    }

    private IEnumerator CoFloatAndFade(TextMeshProUGUI text)
    {
        float startY = 0.35f;
        float endY = 0.55f;
        float duration = 0.5f;

        float t = 0f;
        Color startColor = text.color;

        bool isEnemy =
            transform.parent != null &&
            transform.parent.CompareTag("Enemy");

        float x = isEnemy ? 0.1f : -0.1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            float y = Mathf.Lerp(startY, endY, lerp);

            text.transform.localPosition = new Vector3(x, y, 0f);

            text.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                1f - lerp
            );

            yield return null;
        }

        Destroy(text.gameObject);
    }
    public void SetHpBar(float target01, bool instant = false)
    {
        if (hpBar == null) return;

        target01 = Mathf.Clamp01(target01);

        if (instant || currentHp01 < 0f)
        {
            currentHp01 = target01;
            hpBar.fillAmount = target01;
            return;
        }

        if (hpRoutine != null)
            StopCoroutine(hpRoutine);

        hpRoutine = StartCoroutine(CoAnimateBar(
            currentHp01,
            target01,
            barAnimDuration,
            v =>
            {
                currentHp01 = v;
                hpBar.fillAmount = v;
            }
        ));
    }

    public void SetManaBar(float target01, bool instant = false)
    {
        if (manaBar == null) return;

        target01 = Mathf.Clamp01(target01);

        if (instant || currentMana01 < 0f)
        {
            currentMana01 = target01;
            manaBar.fillAmount = target01;
            return;
        }

        if (manaRoutine != null)
            StopCoroutine(manaRoutine);

        manaRoutine = StartCoroutine(CoAnimateBar(
            currentMana01,
            target01,
            barAnimDuration,
            v =>
            {
                currentMana01 = v;
                manaBar.fillAmount = v;
            }
        ));
    }

    private IEnumerator CoAnimateBar(
        float from,
        float to,
        float duration,
        Action<float> onUpdate
    )
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(from, to, t / duration);
            onUpdate?.Invoke(v);
            yield return null;
        }

        onUpdate?.Invoke(to);
    }
}
