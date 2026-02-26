using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UI_ShowGacha : MonoBehaviour
{

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject shardPrefab;
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
        if (costGacha.CurrentTypeTen == GachaCostType.Ticket)
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

    IEnumerator ShowResult(GachaResult result)
    {
        if (result.type == GachaResultType.Hero)
        {
            HeroInfo hero = DatabaseManager.Instance.HeroDatabase.GetHero(result.heroId);
            if (hero == null) yield break;

            GameObject go = Instantiate(heroPrefab, panelGacha, false);
            go.SetActive(true);

            UI_GachaResult ui = go.GetComponent<UI_GachaResult>();
            if (ui != null)
                ui.SetUp(hero);

            // Hero luôn là hero mới → chơi hiệu ứng
            yield return StartCoroutine(PlayNewHeroEffect(go));

            yield break;
        }

        if (result.type == GachaResultType.Item)
        {
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(result.itemId);
            if (itemData == null) yield break;

            GameObject go = Instantiate(itemPrefab, panelGacha, false);
            go.SetActive(true);

            UI_GachaResult ui = go.GetComponent<UI_GachaResult>();
            if (ui != null)
                ui.Setup(itemData, result.amount);

            yield break;
        }

        // Shard
        {
            int shardAmount = 10;

            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(result.itemId);
            if (itemData == null) yield break;

            GameObject go = Instantiate(shardPrefab, panelGacha, false);
            go.SetActive(true);

            UI_GachaResult ui = go.GetComponent<UI_GachaResult>();
            if (ui != null)
                ui.Setup(itemData, shardAmount);
        }
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
    IEnumerator PlayNewHeroEffect(GameObject heroGO)
    {
        // Tạo layer flash nằm trong hero
        GameObject flashObj = new GameObject("HeroFlash");
        flashObj.transform.SetParent(heroGO.transform, false);

        RectTransform rt = flashObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image flashImg = flashObj.AddComponent<Image>();
        flashImg.color = new Color(1, 1, 1, 0);
        flashImg.raycastTarget = false;

        float duration = 0.15f;
        float t = 0;

        // Fade in
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = t / duration;
            flashImg.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // Fade out
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = 1 - (t / duration);
            flashImg.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        Destroy(flashObj);
    }
    Image CreateFlashImage()
    {
        GameObject flashObj = new GameObject("Flash");
        flashObj.transform.SetParent(panelGacha.parent, false);

        RectTransform rt = flashObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = flashObj.AddComponent<Image>();
        img.raycastTarget = false;

        return img;
    }
}