using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UI_ShowGacha : MonoBehaviour
{
    [SerializeField] private Transform prefabHeroSample;
    [SerializeField] private Transform prefabHeroShardSample;

    [SerializeField] private Transform panelGacha;
    [SerializeField] private bool panelIsActive;
    [SerializeField] UI_CostGacha costGacha;
    private GridLayoutGroup grid;

    private bool isShowing = false; 
    public bool IsShowing => isShowing;

    /* ================= BUTTON ================= */
    void Awake()
    {
        grid = panelGacha.GetComponent<GridLayoutGroup>();
    }



    public void OnClickRoll()
    {
        if (isShowing || panelIsActive) return;

        panelIsActive = true;
        panelGacha.parent.gameObject.SetActive(true);
        ClearOldCards();

        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.padding = new RectOffset(0, 0, 0, 0);
        if (costGacha.CurrentTypeOne == GachaCostType.Ticket)
        {
            PlayerInventory.Instance.ConsumeItem(4, costGacha.TicketCostOne);
        }
        else
        {
            PlayerInventory.Instance.ConsumeItem(2, costGacha.DiamondCostOne);
        }
        StartCoroutine(RollOne());
    }


    public void OnClickRollTen()
    {
        
        if (isShowing || panelIsActive) return;

        panelIsActive = true;
        panelGacha.parent.gameObject.SetActive(true);
        ClearOldCards();

        grid.childAlignment = TextAnchor.UpperLeft;
        grid.padding = new RectOffset(200, 150, 40, 100);
        if(costGacha.CurrentTypeTen == GachaCostType.Ticket)
        {
            PlayerInventory.Instance.ConsumeItem(4, costGacha.TicketCostTen);
        }
        else
        {
            PlayerInventory.Instance.ConsumeItem(2, costGacha.DiamondCostTen);
        }
        StartCoroutine(RollTen());
    }



    IEnumerator RollOne()
    {
        isShowing = true;

        IGachaService gacha = GachaManager.Instance;
        GachaResult result = gacha.Roll();
        yield return ShowResult(result);

        isShowing = false;
        costGacha.RefreshUI();
    }

    IEnumerator RollTen()
    {
        isShowing = true;
        IGachaService gacha = GachaManager.Instance;
        for (int i = 0; i < 10; i++)
        {
            
            GachaResult result = gacha.Roll();
            yield return ShowResult(result);

            yield return new WaitForSeconds(0.1f);
        }

        isShowing = false;
        costGacha.RefreshUI();
    }

    void ClearOldCards()
    {
        foreach (Transform child in panelGacha)
            Destroy(child.gameObject);
    }



    //IEnumerator ShowResult(GachaResult result)
    //{
    //    HeroInfo hero = DatabaseManager.Instance.HeroDatabase.GetHero(result.heroId);
    //    if (hero == null) yield break;



    //    Transform item = Instantiate(
    //        result.type == GachaResultType.Hero
    //            ? prefabHeroSample
    //            : prefabHeroShardSample
    //    );

    //    item.SetParent(panelGacha, false);
    //    item.gameObject.SetActive(true);

    //    Image icon = item.Find("HeroIcon").GetComponent<Image>();
    //    Image light = item.Find("LightOverlay").GetComponent<Image>();

    //    yield return RevealWithLight(icon, light, hero.iconFace);
    //}

    IEnumerator ShowResult(GachaResult result)
    {
        Transform item = Instantiate(
            result.type == GachaResultType.Hero
                ? prefabHeroSample
                : prefabHeroShardSample
        );

        item.SetParent(panelGacha, false);
        item.gameObject.SetActive(true);

        Image icon = item.Find("HeroIcon").GetComponent<Image>();
        Image light = item.Find("LightOverlay").GetComponent<Image>();

        // HERO
        if (result.type == GachaResultType.Hero)
        {
            HeroInfo hero = DatabaseManager.Instance.HeroDatabase.GetHero(result.heroId);
            if (hero == null) yield break;

            yield return RevealWithLight(icon, light, hero.iconFace);
            yield break;
        }

        // SHARD / ITEM => dùng ItemDatabase
        ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(result.itemId);
        if (itemData == null) yield break;

        yield return RevealWithLight(icon, light, itemData.icon);
    }

    IEnumerator RevealWithLight(Image icon, Image light, Sprite sprite)
    {
        icon.enabled = false;
        light.enabled = true;
        light.color = new Color(1f, 1f, 1f, 0f);

        float t = 0f;
        float duration = 0.07f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        icon.sprite = sprite;
        icon.enabled = true;

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        light.enabled = false;
    }
    private void Update()
    {
        if (panelIsActive && !isShowing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                panelIsActive = false;
                panelGacha.parent.gameObject.SetActive(false);
            }
        }
    }

   

}
