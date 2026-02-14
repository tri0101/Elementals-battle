using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroChooseItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Button buttonChoose;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform starRoot;
    [SerializeField] private Transform rankRoot;
    [SerializeField] private Image frameRank;

    public HeroViewData Data => data;
    private HeroViewData data;
    private UI_FormationManager formationManager;
    private UI_PanelChooseHero panelChooseHero;

    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;

    public  bool IsInFormation { get; private set; }
    [SerializeField] private Transform originalParent;
    [SerializeField] private CanvasGroup canvasGroup;

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();
        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        buttonChoose.onClick.RemoveAllListeners();
        buttonChoose.onClick.AddListener(OnClickHero);
    }

    public void Setup(HeroViewData heroData, UI_FormationManager fm, UI_PanelChooseHero panel)
    {
        data = heroData;
        formationManager = fm;
        panelChooseHero = panel;

        icon.sprite = data.info.iconFace;
        levelText.text = data.instance.level.ToString();

        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

        IsInFormation = false;
    }

    void OnClickHero()
    {
        if (formationManager == null) return;
        if (formationManager.IsBusy) return;

        if (!IsInFormation)
        {
            bool ok = formationManager.TryAddHero(this);
            if (!ok) Debug.Log("Không thể add hero");
        }
        else
        {
            formationManager.RemoveHero(this, panelChooseHero.InventoryContent);
        }
    }

    public void SetInFormation(bool value) => IsInFormation = value;

    void UpdateStar(int star)
    {
        if (starLayout != null)
        {
            starLayout.spacing = star <= 4 ? -70f : -25f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(starRoot as RectTransform);
        }

        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateRankVisual(int rank)
    {
        if (rankRoot == null)
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

            if (activeCount == 2)
                rankLayout.spacing = -200f;
            else if (activeCount == 3)
                rankLayout.spacing = -70f;
            else
                rankLayout.spacing = 0f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rankRoot as RectTransform);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInFormation) return;
        if (formationManager == null || formationManager.IsBusy) return;

        originalParent = transform.parent;

        transform.SetParent(transform.root, true);
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInFormation) return;
        if (formationManager == null || formationManager.IsBusy) return;

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInFormation) return;
        if (formationManager == null) return;

        canvasGroup.blocksRaycasts = true;

        
        if (formationManager.IsBusy)
        {
            formationManager.PlaceHero(this, originalParent);
            return;
        }

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (!result.gameObject.name.StartsWith("Slot"))
                continue;

            Transform targetSlot = result.gameObject.transform;
            if (targetSlot == null)
                continue;

            if (targetSlot == originalParent)
            {
                formationManager.PlaceHero(this, originalParent);
                return;
            }

            var existingHero = targetSlot.GetComponentInChildren<UI_HeroChooseItem>();
            if (existingHero != null && existingHero != this)
            {
                formationManager.SwapHeroes(this, existingHero);
            }
            else
            {
                formationManager.MoveHeroToSlot(this, targetSlot);
            }

            return;
        }

       
        formationManager.PlaceHero(this, originalParent);
    }
}