using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GachaFeatured : MonoBehaviour, IObserver
{
    [Header("Transform")]
    [SerializeField] Transform panelChooseHero;
    [SerializeField] Transform panelTokenExchange;
    [SerializeField] Transform panelLogHistory;
    [SerializeField] Transform panelLimitedOffer;
    [SerializeField] Transform panelSummonRate;


    [SerializeField] Transform contentChoose;
    [SerializeField] Transform contentPreview;
    [SerializeField] Transform contentTokenExchange;
    [SerializeField] Transform contentSummonReward;

    [Header("Prefab")]
    [SerializeField] GameObject heroPrefabChoose;
    [SerializeField] GameObject heroPrefabTokenExchange;
    [SerializeField] GameObject summonRewardPrefab;
    [Header("Button Select")]
    [SerializeField] Button buttonChoose;
    [SerializeField] Button buttonTokenExchange;
    [SerializeField] Button buttonLogHistory;
    [SerializeField] Button buttonLimitedOffer;
    [SerializeField] Button buttonSummonRate; //mở panel summon rate
    [SerializeField] Button buttonSummonReward; //mở panel summon reward(trong panel summon rate)
    [SerializeField] Button buttonDetailRate; //mở panel detail rate (trong panel summon rate)
    [Header("Button Close")]
    [SerializeField] Button buttonCloseChoose;
    [SerializeField] Button buttonCloseTokenExchange;
    [SerializeField] Button buttonCloseLimitedOffer;
    [SerializeField] Button buttonCloseSummonRate;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI nameHeroPreviewText;
    [SerializeField] TextMeshProUGUI itemTokenAmountText;
    [SerializeField] TextMeshProUGUI recruitText1x;
    [SerializeField] TextMeshProUGUI recruitText10x;
    private int selectedHeroId = -1;

    private void Awake()
    {
        PlayerInventory.Instance.AddObserver(this);
        AddListener();
        RefreshTicketText();
        RefreshItemToken(PlayerInventory.Instance.GetItemQuantity(6));


    }
    void OnEnable()
    {
        OnHeroClicked(54);
    }
    void AddListener()
    {
        buttonChoose.onClick.RemoveAllListeners();
        buttonChoose.onClick.AddListener(ChooseHeroFeatured);
        buttonTokenExchange.onClick.RemoveAllListeners();
        buttonTokenExchange.onClick.AddListener(TokenExchange);
        buttonLimitedOffer.onClick.RemoveAllListeners();
        buttonLimitedOffer.onClick.AddListener(LimitedOffer);
        buttonSummonRate.onClick.RemoveAllListeners();
        buttonSummonRate.onClick.AddListener(SummonRate);
        buttonSummonReward.onClick.RemoveAllListeners();
        buttonSummonReward.onClick.AddListener(SummonReward);
        // == Close Button ==
        buttonCloseChoose.onClick.RemoveAllListeners();
        buttonCloseChoose.onClick.AddListener(ClosePanelChoose);
        buttonCloseTokenExchange.onClick.RemoveAllListeners();
        buttonCloseTokenExchange.onClick.AddListener(ClosePanelTokenExchange);
        buttonCloseLimitedOffer.onClick.RemoveAllListeners();
        buttonCloseLimitedOffer.onClick.AddListener(ClosePanelLimitedOffer);
        buttonCloseSummonRate.onClick.RemoveAllListeners();
        buttonCloseSummonRate.onClick.AddListener(ClosePanelSummonRate);
    }
    void TokenExchange()
    {
        panelTokenExchange.gameObject.SetActive(true);
        ClearContentTokenExchange();
        BannerTokenExchange tokenExchangeDataList = GachaManager.Instance.GetTokenExchangeList();
        foreach (BannerTokenExchangeData data in tokenExchangeDataList.exchangeDataList)
        {
            GameObject go = Instantiate(heroPrefabTokenExchange, contentTokenExchange);
            var ui = go.GetComponent<UI_TokenExchange>();
            ui.SetUp(data);
        }
    }
    void LimitedOffer()
    {
        panelLimitedOffer.gameObject.SetActive(true);
    }
    void SummonRate()
    {
        
        ClearContentSummonReward();
        SummonReward();
        panelSummonRate.gameObject.SetActive(true);
    }
    void ClearContentSummonReward()
    {
           foreach (Transform t in contentSummonReward)
            Destroy(t.gameObject);
    }
    void SummonReward()
    {
        //tier SS
        GameObject goSS = Instantiate(summonRewardPrefab, contentSummonReward);
            var uiSS = goSS.GetComponent<UI_SummonReward>();
        uiSS.SetUp(HeroTier.SS);
        //tier S
        GameObject goS = Instantiate(summonRewardPrefab, contentSummonReward);
        var uiS = goS.GetComponent<UI_SummonReward>();
        uiS.SetUp(HeroTier.S);
        //tier A
        GameObject goA = Instantiate(summonRewardPrefab, contentSummonReward);
        var uiA = goA.GetComponent<UI_SummonReward>();
        uiA.SetUp(HeroTier.A);
        //tier B
        GameObject goB = Instantiate(summonRewardPrefab, contentSummonReward);
        var uiB = goB.GetComponent<UI_SummonReward>();
        uiB.SetUp(HeroTier.B);
        //tier C
        GameObject goC = Instantiate(summonRewardPrefab, contentSummonReward);
        var uiC = goC.GetComponent<UI_SummonReward>();
        uiC.SetUp(HeroTier.C);
        //Item
        GameObject goItem = Instantiate(summonRewardPrefab, contentSummonReward);
        var uiItem = goItem.GetComponent<UI_SummonReward>();
        uiItem.SetUpItem();
    }
    void SummonDetailRate()
    {

    }
    void ChooseHeroFeatured()
    {
        
        panelChooseHero.gameObject.SetActive(true);
        ClearContentChoose();

        List<int> featurePool = GachaManager.Instance.GetFeaturedPool();
        for (int i = 0; i < featurePool.Count; i++)
        {
            int heroId = featurePool[i];

            GameObject go = Instantiate(heroPrefabChoose, contentChoose);
            var ui = go.GetComponent<UI_HeroFeatured>();
            ui.SetUp(
                heroId: heroId,
                getSelectedHeroId: GetSelectedHeroId,
                onClicked: OnHeroClicked
            );
        }

        RefreshAll();
    }

    int GetSelectedHeroId() => selectedHeroId;

    void OnHeroClicked(int heroId)
    {
        // chỉ được chọn 1 hero
        selectedHeroId = heroId;

        // báo cho gacha system (theo heroId)
        GachaManager.Instance.SetFeaturedSelectionByHeroId(selectedHeroId);
        nameHeroPreviewText.text = DatabaseManager.Instance.HeroDatabase.GetHero(selectedHeroId).Name;
        SetPreviewHero();
        RefreshAll();
    }

    void RefreshAll()
    {
        foreach (Transform t in contentChoose)
        {
            var ui = t.GetComponent<UI_HeroFeatured>();
            if (ui != null)
                ui.RefreshVisual();
        }
    }

    void ClearContentChoose()
    {
        foreach (Transform t in contentChoose)
            Destroy(t.gameObject);
    }
    void ClearContentTokenExchange()
    {
        foreach (Transform t in contentTokenExchange)
            Destroy(t.gameObject);
    }
    void SetPreviewHero()
    {
        ClearPreviewPrefab();
        GameObject go = DatabaseManager.Instance.HeroDatabase.GetHero(selectedHeroId).HeroPreviewPrefabs;
        if(go != null)
        {
            Instantiate(go, contentPreview);
        }


    }
    void ClearPreviewPrefab()
    {
        foreach(Transform t in contentPreview)
            Destroy(t.gameObject);
    }

   
    void ClosePanelChoose()
    {
        panelChooseHero.gameObject.SetActive(false);
    }
    void ClosePanelTokenExchange()
    {
        panelTokenExchange.gameObject.SetActive(false);
    }
    void ClosePanelLimitedOffer()
    {
        panelLimitedOffer.gameObject.SetActive(false);
    }
    void ClosePanelSummonRate()
    {
        panelSummonRate.gameObject.SetActive(false);
    }
    //== Observer Pattern ==

    public void OnNotify(object data)
    {

        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 6)
                RefreshItemToken(value);
            else if (itemId == 7)
            {
                RefreshTicketText();
            }

        }
    }
    void RefreshItemToken(int value)
    {
        itemTokenAmountText.text = value.ToString();
    }
    void RefreshTicketText()
    {
        int currentTickets = PlayerInventory.Instance.GetItemQuantity(7);
        recruitText1x.text = $"{currentTickets}/1";
        recruitText10x.text = $"{currentTickets}/10";
    }
    
}