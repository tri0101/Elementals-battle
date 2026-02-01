using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroChooseItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Button buttonChoose;
    public Image icon;
    public TextMeshProUGUI levelText;
    public Transform starRoot;
    public Transform rankRoot;
    public Image frameRank;
    public HeroViewData Data => data;
    private HeroViewData data;
    private UI_FormationManager formationManager;
    private UI_PanelChooseHero panelChooseHero;

    private HorizontalLayoutGroup starLayout;

    public bool IsInFormation { get; private set; }
    public Transform originalParent;
    public CanvasGroup canvasGroup;

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttonChoose.onClick.RemoveAllListeners();
        buttonChoose.onClick.AddListener(OnClickHero);
    }

    public void Setup(
        HeroViewData heroData,
        UI_FormationManager fm,
        UI_PanelChooseHero panel
    )
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
        if (!IsInFormation)
        {
            bool ok = formationManager.TryAddHero(this);
            if (!ok)
                Debug.Log("Không thể add hero");
        }
        else
        {
            formationManager.RemoveHero(this, panelChooseHero.InventoryContent);
        }
    }

    public void SetInFormation(bool value)
    {
        IsInFormation = value;
    }


    void UpdateStar(int star)
    {
        if (starLayout != null)
        {
            starLayout.spacing = star <= 4 ? -70f : -25f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                starRoot as RectTransform
            );
        }

        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateRankVisual(int rank)
    {
        for (int i = 0; i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(false);

        Color rankColor;
        int plusValue;

        if (rank < greenRank)
        {
            rankColor = blackColor;
            frameRank.color = blackColor;
            plusValue = Mathf.Max(0, rank - blackRank);
        }
        else
        {
            rankColor = greenColor;
            frameRank.color = greenColor;
            plusValue = Mathf.Max(0, rank - greenRank);
        }

        for (int i = 0; i < plusValue && i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(true);
    }


  
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInFormation) return;

        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInFormation) return;

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInFormation) return;

        canvasGroup.blocksRaycasts = true;


        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.name.StartsWith("Slot"))
            {
                Transform targetSlot = result.gameObject.transform;


                if (targetSlot != originalParent)
                {

                    var existingHero = targetSlot.GetComponentInChildren<UI_HeroChooseItem>();
                    if (existingHero != null)
                    {

                        formationManager.SwapHeroes(this, existingHero);
                    }
                    else
                    {

                        formationManager.MoveHeroToSlot(this, targetSlot);
                    }
                    return;
                }
            }
        }



        formationManager.PlaceHero(this, originalParent);

    }
}
