using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum HPNotifyType
{
    HPMinus,
    HPPlus,
    ArmorMinus,
}
public enum DamageType // để phân biệt kiểu damage (normal, crit, block)
{
    critDamage,
    blockDamage,
    normalDamage,
    turnDamage, // damage theo thời gian (dot)
}

public class HeroUI : MonoBehaviour, IObserver
{
    [SerializeField] HeroControl heroControl;

    public HeroControl HeroControl => heroControl;
    [Header("String pattern")]
    private string critRatePattern = "Crit Rate Increased";
    private string armorDecreased = "Armor Decreased";
    private string armorIncreased = "Armor Increased";
    private string healRateIncreased = "Heaing Rate Increased";
    private string healRateDecreased = "Heaing Rate Decreased";
    private string manaRecoveryDecreased = "Mana Recovery Decreased";
    private string manaRecoveryIncreased = "Mana Recovery Increased";
    private string manaRestoration = "Mana Restoration";
    private string manaReduced = "Mana Reduced";
    private string controlFreeIncreased = "Control-free Increased";
    private string controlFreeDecreased = "Control-free Decreased";
    private string rootedNotice = "Root";
    private string burnNotice = "Burn";
    private string stunNotice = "Stun";
    private string paralysisNotice = "Paralysis";

    [Header("Bars")]
    public Image hpBar;
    public Image manaBar;
    public Image shieldBar;
    public TextMeshProUGUI damageTextPrefab;

    public Transform listDamage;
    // ===== Bar Animation =====
    [SerializeField] private float barAnimDuration = 0.25f;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material critMaterial;
    private float currentShield01 = -1f;
    private float currentHp01 = -1f;
    private float currentMana01 = -1f;

    private Coroutine hpRoutine;
    private Coroutine manaRoutine;
    private Coroutine shieldRoutine;
    private void Start()
    {
        heroControl = transform.parent.GetComponent<HeroControl>();
        if (heroControl != null && heroControl.transform != null)
        {
            Vector3 s = transform.localScale;
            s.x = heroControl.transform.CompareTag("Enemy") ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            transform.localScale = s;
        }
        heroControl.AddObserver(this);
        hpBar = transform.Find("HP").GetChild(1).GetComponent<Image>();
        manaBar = transform.Find("Mana").GetChild(1).GetComponent<Image>();
        
        shieldBar = transform.Find("Shield").GetChild(1).GetComponent<Image>();
        if (heroControl.HeroStatRuntime.CurrentShield > 0)
        {
            shieldBar.transform.parent.gameObject.SetActive(true);
            SetShieldBar(1f, true);
        }

        listDamage = transform.Find("ListDamage");
        SetHpBar(1f, true);
       
        if (heroControl.HeroInfo.ultimate == null)
        {
            manaBar.transform.parent.gameObject.SetActive(false);
        }
        SetManaBar(heroControl.HeroStatRuntime.CurrentMana / heroControl.HeroStatRuntime.MaxMana, true);


    }

    // ===== Flip helpers =====
    private bool IsHeroFlippedX()
    {
        if (heroControl == null) return false;

        // IMPORTANT: enemy mặc định scaleX đã âm. Nên phải check theo tag để biết "flip thật sự" hay chỉ là default.
        // - Hero: default +X => flipped khi lossyScale.x < 0
        // - Enemy: default -X => flipped khi lossyScale.x > 0
        bool isEnemy = heroControl.transform != null && heroControl.transform.CompareTag("Enemy");
        float x = heroControl.transform.lossyScale.x;

        return isEnemy ? x > 0f : x < 0f;
    }

    private void ApplyFlipToFloatingText(Transform textTransform)
    {
        if (textTransform == null || heroControl == null)
            return;

        bool flipped = IsHeroFlippedX();

        Vector3 s = textTransform.localScale;
        s.x = Mathf.Abs(s.x) * (flipped ? -1f : 1f);
        textTransform.localScale = s;
    }

    private IEnumerator CoKeepFlipSynced(TextMeshProUGUI text)
    {
        if (text == null) yield break;

        bool lastFlip = IsHeroFlippedX();
        ApplyFlipToFloatingText(text.transform);

        while (text != null)
        {
            bool flip = IsHeroFlippedX();
            if (flip != lastFlip)
            {
                lastFlip = flip;
                ApplyFlipToFloatingText(text.transform);
            }

            yield return null;
        }
    }

    public void OnNotify(ModifyStatType type, int value)
    {
        switch (type)
        {
            case ModifyStatType.CritRate:
                SpawnFloatingEffectText(type, value);
                break;
            case ModifyStatType.Armor:
                SpawnFloatingEffectText(type, value);
                break;
            case ModifyStatType.HealingRate:
                SpawnFloatingEffectText(type, value);
                break;
            case ModifyStatType.ManaRecovery:
                SpawnFloatingEffectText(type, value);
                break;
            case ModifyStatType.Mana:
                SpawnFloatingEffectText(type, value);
                break;
            case ModifyStatType.ControlFree:
                SpawnFloatingEffectText(type, value);
                break;
        }
    }
    public void OnNotify(AbilityEffectType type)
    {
        switch (type)
        {
            case AbilityEffectType.Burn:
                SpawnFloatingEffectText(type);
                break;
            case AbilityEffectType.Rooted:
                SpawnFloatingEffectText(type);
                break;
            case AbilityEffectType.Stun:
                SpawnFloatingEffectText(type);
                break;
            case AbilityEffectType.Paralysis:
                SpawnFloatingEffectText(type);
                break;
        }
    }
    public void OnNotify(HeroNotifyType type, object value)
    {
        switch (type)
        {
            case HeroNotifyType.HPChanged:
                SetHpBar((float)value);
                break;
            case HeroNotifyType.ShieldChanged:
                SetShieldBar((float)value);
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
    public void OnNotify(HPNotifyType type, object value, DamageType damageType)
    {
        switch (type)
        {
            case HPNotifyType.HPMinus:
                SpawnDamageText(type , (int)value, damageType);
                break;
            case HPNotifyType.HPPlus:
                SpawnDamageText(type, (int)value, damageType);
                break;
        }
    }

    // ================= FLOATING TEXT =================
    public void SpawnFloatingEffectText(ModifyStatType type, int value)
    {
        if (damageTextPrefab == null || listDamage == null) return;

        TextMeshProUGUI text = Instantiate(damageTextPrefab, listDamage);
        StartCoroutine(CoKeepFlipSynced(text));
        if(value > 0)
        {
            text.color = new Color32(253, 255, 0, 255);
            text.fontSize = 12;
            text.fontSharedMaterial = critMaterial;
        }
        else
        {
            text.color = new Color32(211, 71, 35, 255);
            text.fontSize = 12;
            text.fontSharedMaterial = critMaterial;
        }
        switch (type)
        {

            case ModifyStatType.CritRate:
                if (value > 0) text.text = critRatePattern;
                StartCoroutine(CoShowAndFade(text));
                break;
            case ModifyStatType.Armor:
                if (value < 0) text.text = armorDecreased;
                else text.text = armorIncreased;
                StartCoroutine(CoShowAndFade(text));
                break;
            case ModifyStatType.HealingRate:
                if (value > 0) text.text = healRateIncreased;
                else text.text = healRateDecreased;
                StartCoroutine(CoShowAndFade(text));
                break;
            case ModifyStatType.ManaRecovery:
                if (value > 0) text.text = manaRecoveryIncreased;
                else text.text = manaRecoveryDecreased;
                StartCoroutine(CoShowAndFade(text));
                break;
            case ModifyStatType.Mana:
                if (value > 0) text.text = manaRestoration;
                else text.text = manaReduced;
                StartCoroutine(CoShowAndFade(text));
                break;
            case ModifyStatType.ControlFree:
                if (value > 0) text.text = controlFreeIncreased;
                else text.text = controlFreeDecreased;
                StartCoroutine(CoShowAndFade(text));
                break;
        }
    }
    public void SpawnFloatingEffectText(AbilityEffectType type)
    {
        if (damageTextPrefab == null || listDamage == null) return;

        TextMeshProUGUI text = Instantiate(damageTextPrefab, listDamage);
        StartCoroutine(CoKeepFlipSynced(text));

        switch (type)
        {
            case AbilityEffectType.Burn:
                text.text = burnNotice;
                text.color = new Color32(253, 255, 0, 255);
                text.fontSize = 15;
                text.fontSharedMaterial = critMaterial;
                StartCoroutine(CoShowAndFade(text));
                break;
            case AbilityEffectType.Rooted:
                text.text = rootedNotice;
                text.color = new Color32(253, 255, 0, 255);
                text.fontSize = 15;
                text.fontSharedMaterial = critMaterial;
                StartCoroutine(CoShowAndFade(text));
                break;
            case AbilityEffectType.Stun:
                text.text = stunNotice;
                text.color = new Color32(253, 255, 0, 255);
                text.fontSize = 15;
                text.fontSharedMaterial = critMaterial;
                StartCoroutine(CoShowAndFade(text));
                break;
            case AbilityEffectType.Paralysis:
                text.text = paralysisNotice;
                text.color = new Color32(253, 255, 0, 255);
                text.fontSize = 15;
                text.fontSharedMaterial = critMaterial;
                StartCoroutine(CoShowAndFade(text));
                break;
        }
    }
    private void SpawnDamageText(HPNotifyType hpType, int value, DamageType damageType)
    {
        if (damageTextPrefab == null || value <= 0) return;

        TextMeshProUGUI text =
            Instantiate(damageTextPrefab, listDamage);
        StartCoroutine(CoKeepFlipSynced(text));

        if (hpType == HPNotifyType.HPPlus)
        {
            
            text.text = value.ToString();
            text.color = Color.green;
            text.fontSize = 12;
            text.fontSharedMaterial = normalMaterial;
        }
        else {
            switch (damageType)
            {
                case DamageType.critDamage:
                    text.text = "CRIT-" + value.ToString();
                    text.color = new Color32(253, 255, 0, 255);
                    text.fontSize = 15;
                    text.fontSharedMaterial = critMaterial;
                    break;

                case DamageType.normalDamage or DamageType.turnDamage:
                default:
                    text.text = value.ToString();
                    text.color = new Color32(211, 71, 35, 255);
                    text.fontSize = 12;
                    text.fontSharedMaterial = normalMaterial;
                    break;
            }

        }
        StartCoroutine(CoFloatAndFade(text));
    }

    private IEnumerator CoShowAndFade(TextMeshProUGUI text)
    {
        if (text == null) yield break;

        float yOffset = 0.35f;
        float showDuration = 0.35f;
        float fadeDuration = 1f;

        bool isEnemy =
            transform.parent != null &&
            transform.parent.CompareTag("Enemy");

        float x = isEnemy ? 0.15f : -0.15f;

        Vector3 p = text.transform.localPosition;
        text.transform.localPosition = new Vector3(x, p.y + yOffset, p.z);

        Color startColor = text.color;
        text.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        if (showDuration > 0f)
            yield return new WaitForSeconds(showDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / fadeDuration);

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

        float x = isEnemy ? 0.15f : -0.15f;

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
    public void SetShieldBar(float target01, bool instant = false)
    {
        if (shieldBar == null) return;
        if (shieldBar.transform.parent.gameObject.activeSelf == false)
            shieldBar.transform.parent.gameObject.SetActive(true);
        target01 = Mathf.Clamp01(target01);

        if (instant || currentShield01 < 0f)
        {
            currentShield01 = target01;
            shieldBar.fillAmount = target01;
            return;
        }

        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);

        shieldRoutine = StartCoroutine(CoAnimateBar(
            currentShield01,
            target01,
            barAnimDuration,
            v =>
            {
                currentShield01 = v;
                shieldBar.fillAmount = v;
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