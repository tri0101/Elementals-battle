using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public enum HeroNotifyType
{
    HPChanged,
    ShieldChanged,
    ManaChanged,
    Dead,
    Revive
}

public class UI_HeroBattle : MonoBehaviour, IObserver
{
    [Header("Icon")]
    public Image icon;

    [Header("Hero Control")]
    public HeroControl heroControl;

    [Header("Text")]
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI skillText;

    [Header("Star")]
    public Transform starRoot;

    [Header("Rank")]
    public Transform rankRoot;
    public Image frameRank;

    [Header("Bars")]
    public Image hpBar;
    public Image manaBar;

    [Header("Button")]
    public Button button;

    // ===== Layout =====
    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;

    // ===== Data =====
    private HeroViewData data;
    private Action<HeroViewData> onClickCallback;

    // ===== Rank config =====
    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = new Color(157/255f, 143/255f, 143/255f);
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    // ===== Bar Animation =====
    [SerializeField] private float barAnimDuration = 0.25f;

    private float currentHp01 = -1f;
    private float currentMana01 = -1f;

    private Coroutine hpRoutine;
    private Coroutine manaRoutine;

    // ===== Skill Text Blink =====
    [Header("Skill Ready UI")]
    [SerializeField] private Color skillColorA = Color.white;
    [SerializeField] private Color skillColorB = new Color(1f, 0.85f, 0.1f); // yellow-ish
    [SerializeField] private float skillBlinkSpeed = 2.5f;

    private Coroutine skillBlinkRoutine;
    private bool lastCanSkill;
    public Image backDead;

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();

        SetSkillReadyVisual(false, true);
    }

    private void OnDisable()
    {
        StopSkillBlink();
    }

    public void Setup(
        HeroViewData heroData,
        Action<HeroViewData> onClick = null
    )
    {
        data = heroData;
        onClickCallback = onClick;

        if (icon != null)
            icon.sprite = data.info.iconFace;

        if (roleText != null)
            roleText.text = $"{data.info.role}";

        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

        

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            button.enabled = false;
        }

        lastCanSkill = false;
        SetSkillReadyVisual(false, true);
    }

    public void BindHeroControl(HeroControl hc)
    {
        heroControl = hc;

        if (heroControl != null)
        {
            heroControl.AddObserver(this);

            lastCanSkill = heroControl.CanSkill;
            SetSkillReadyVisual(lastCanSkill, true);
        }
        SetHpBar(1f, true);
        SetManaBar(heroControl.HeroStatRuntime.CurrentMana / heroControl.HeroStatRuntime.MaxMana, true);
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
                if (button != null)
                    button.interactable = false;
                break;

            case HeroNotifyType.Revive:
                SetHpBar(1f, true);
                if (button != null)
                    button.interactable = true;
                break;
        }

        // Also update skill ready indicator (since Notify happens frequently on HP/Mana changes)
        UpdateSkillReadyState();
    }

    private void Update()
    {
        
        if (heroControl.HeroStatRuntime.CurrentHealth <= 0)
        {
            skillText.gameObject.SetActive(false);
            backDead.gameObject.SetActive(true);
        }
        // Fallback polling in case HeroControl doesn't push notifications when CanSkill changes.
        UpdateSkillReadyState();
    }

    private void UpdateSkillReadyState()
    {
        if (heroControl == null)
            return;

        bool canSkill = heroControl.CanSkill;
        if (canSkill == lastCanSkill)
            return;

        lastCanSkill = canSkill;
        SetSkillReadyVisual(canSkill, false);
    }

    private void SetSkillReadyVisual(bool enabled, bool instant)
    {
        if (skillText == null)
            return;

        skillText.gameObject.SetActive(enabled);

        if (!enabled)
        {
            StopSkillBlink();
            skillText.color = skillColorA;
            return;
        }

        if (instant)
            skillText.color = skillColorB;

        StartSkillBlink();
    }

    private void StartSkillBlink()
    {
        if (skillText == null)
            return;

        if (skillBlinkRoutine != null)
            return;

        skillBlinkRoutine = StartCoroutine(CoBlinkSkillText());
    }

    private void StopSkillBlink()
    {
        if (skillBlinkRoutine == null)
            return;

        StopCoroutine(skillBlinkRoutine);
        skillBlinkRoutine = null;
    }

    private IEnumerator CoBlinkSkillText()
    {
        // Ping-pong between A and B
        while (skillText != null && skillText.gameObject.activeInHierarchy)
        {
            float t = Mathf.PingPong(Time.time * skillBlinkSpeed, 1f);
            skillText.color = Color.Lerp(skillColorA, skillColorB, t);
            yield return null;
        }

        skillBlinkRoutine = null;
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

    void UpdateStar(int star)
    {
        if (starRoot == null) return;

        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateRankVisual(int rank)
    {
        if (rankRoot == null || frameRank == null)
            return;

        for (int i = 0; i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(false);

        if (rank < greenRank)
        {
            frameRank.color = blackColor;
            int plus = rank - blackRank;

            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }
        else
        {
            frameRank.color = greenColor;
            int plus = rank - greenRank;

            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }

        if (rankLayout != null)
        {
            int activeCount = 0;
            for (int i = 0; i < rankRoot.childCount; i++)
                if (rankRoot.GetChild(i).gameObject.activeSelf)
                    activeCount++;

            rankLayout.spacing =
                activeCount == 2 ? -200f :
                activeCount == 3 ? -70f : 0f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rankRoot as RectTransform);
        }
    }

    void OnClick()
    {
        onClickCallback?.Invoke(data);
    }
}