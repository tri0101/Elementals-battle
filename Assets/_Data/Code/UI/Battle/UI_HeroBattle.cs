using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public enum HeroNotifyType
{
    HPChanged,
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

    [Header("Role")]
    public TextMeshProUGUI roleText;

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

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    // ===== Bar Animation =====
    [SerializeField] private float barAnimDuration = 0.25f;

    private float currentHp01 = -1f;
    private float currentMana01 = -1f;

    private Coroutine hpRoutine;
    private Coroutine manaRoutine;

    

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();
    }

    //void OnDestroy()
    //{
    //    if (heroControl != null)
    //        heroControl.RemoveObserver(this);
    //}

   


    public void Setup(
        HeroViewData heroData,
        Action<HeroViewData> onClick = null
    )
    {
        data = heroData;
        onClickCallback = onClick;

        // ===== ICON =====
        if (icon != null)
            icon.sprite = data.info.iconFace;

        // ===== ROLE =====
        if (roleText != null)
            roleText.text = $"{data.info.role}";

        // ===== STAR + RANK =====
        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

        // ===== BAR (init = instant) =====
        SetHpBar(1f, true);
        SetManaBar(0f, true);

        // ===== BUTTON =====
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            button.interactable = true;
        }
    }

    

    public void BindHeroControl(HeroControl hc)
    {
        

        heroControl = hc;

        if (heroControl != null)
            heroControl.AddObserver(this);
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
