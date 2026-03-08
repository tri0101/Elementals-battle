using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_GachaFeatured : MonoBehaviour, IObserver
{
    [Header("Transform")]
    [SerializeField] Transform panelChooseHero;
    [SerializeField] Transform panelTokenExchange;
    [SerializeField] Transform panelLogHistory;
    [SerializeField] Transform panelLimitedOffer;
    [SerializeField] Transform contentChoose;
    [SerializeField] Transform contentPreview;
    [SerializeField] Transform contentTokenExchange;

    [Header("Prefab")]
    [SerializeField] GameObject heroPrefabChoose;
    [SerializeField] GameObject heroPrefabTokenExchange;
    [Header("Button Select")]
    [SerializeField] Button buttonChoose;
    [SerializeField] Button buttonTokenExchange;
    [SerializeField] Button buttonLogHistory;
    [SerializeField] Button buttonLimitedOffer;
    [Header("Button Close")]
    [SerializeField] Button buttonCloseChoose;
    [SerializeField] Button buttonCloseTokenExchange;

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

        // == Close Button ==
        buttonCloseChoose.onClick.RemoveAllListeners();
        buttonCloseChoose.onClick.AddListener(ClosePanelChoose);
        buttonCloseTokenExchange.onClick.RemoveAllListeners();
        buttonCloseTokenExchange.onClick.AddListener(ClosePanelTokenExchange);
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

    //== Observer Pattern ==

    public void OnNotify(object data)
    {

        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 6)
                itemTokenAmountText.text = value.ToString();
            else if(itemId == 7){
                RefreshTicketText();
            }

        }
    }
    void RefreshTicketText()
    {
        int currentTickets = PlayerInventory.Instance.GetItemQuantity(7);
        recruitText1x.text = $"{currentTickets}/1";
        recruitText10x.text = $"{currentTickets}/10";
    }
    
}