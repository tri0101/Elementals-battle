using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_Exchange : MonoBehaviour
{
    public Transform panelBuyStamina;
    public Transform panelBuyCoin;

    [Header("Success UI")]
    public Transform panelSuccessPrefab;
    public Transform successParent;

    private int timesBuyStamina = 0;
    private int timesBuyCoin = 0;

    private int coinRewardBase = 25000;
    private int staminaRewardBase = 120;

    [Header("UI")]
    public TextMeshProUGUI staminaReward;
    public TextMeshProUGUI coinReward;
    public TextMeshProUGUI diamondCostBuyStamina;
    public TextMeshProUGUI diamondCostBuyCoin;

    public Button buttonBuyStamina;
    public Button buttonBuyCoin;
    public Button buttonBackStamina;
    public Button buttonBackCoin;

    private void Awake()
    {
        buttonBuyStamina.onClick.AddListener(OnClickBuyStamina);
        buttonBuyCoin.onClick.AddListener(OnClickBuyCoin);
        buttonBackStamina.onClick.AddListener(OnClickBackStamina);
        buttonBackCoin.onClick.AddListener(OnClickBackCoin);
    }

    // ================= PANEL =================

    public void ShowPanelBuyStamina()
    {
        panelBuyStamina.gameObject.SetActive(true);
        panelBuyCoin.gameObject.SetActive(false);

        staminaReward.text = staminaRewardBase.ToString();
        RefreshStaminaUI();
    }

    public void ShowPanelBuyCoin()
    {
        panelBuyCoin.gameObject.SetActive(true);
        panelBuyStamina.gameObject.SetActive(false);

        coinReward.text = coinRewardBase.ToString();
        RefreshCoinUI();
    }

    void OnClickBackStamina()
    {
        panelBuyStamina.gameObject.SetActive(false);
    }

    void OnClickBackCoin()
    {
        panelBuyCoin.gameObject.SetActive(false);
    }

    // ================= BUY =================

    void OnClickBuyStamina()
    {
        int cost = GetStaminaCost();
        int diamondCurrent = PlayerInventory.Instance.GetItemQuantity(2);

        if (diamondCurrent < cost)
        {
            RefreshStaminaUI();
            return;
        }

        PlayerInventory.Instance.AddItem(2, -cost);
        timesBuyStamina++;
        PlayerInventory.Instance.AddItem(3, staminaRewardBase);

        RefreshStaminaUI();
        SpawnSuccessUI();
    }

    void OnClickBuyCoin()
    {
        int cost = GetCoinCost();
        int diamondCurrent = PlayerInventory.Instance.GetItemQuantity(2);

        if (diamondCurrent < cost)
        {
            RefreshCoinUI();
            return;
        }

        PlayerInventory.Instance.AddItem(2, -cost);
        timesBuyCoin++;
        PlayerInventory.Instance.AddItem(1, coinRewardBase);

        RefreshCoinUI();
        SpawnSuccessUI();
    }

    // ================= SUCCESS EFFECT =================

    void SpawnSuccessUI()
    {
        Transform clone = Instantiate(panelSuccessPrefab, successParent);
        clone.gameObject.SetActive(true);
        RectTransform rect = clone.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);

        StartCoroutine(MoveAndDestroy(rect));
    }

    IEnumerator MoveAndDestroy(RectTransform rect)
    {
        float duration = 0.25f;
        float time = 0f;

        float startY = 0;
        float endY = 75f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float newY = Mathf.Lerp(startY, endY, t);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, newY);

            yield return null;
        }

        Destroy(rect.gameObject);
    }

    // ================= REFRESH UI =================

    void RefreshStaminaUI()
    {
        int cost = GetStaminaCost();
        int diamondCurrent = PlayerInventory.Instance.GetItemQuantity(2);

        diamondCostBuyStamina.text = cost.ToString();

        bool canBuy = diamondCurrent >= cost;

        diamondCostBuyStamina.color = canBuy ? Color.white : Color.red;
        buttonBuyStamina.interactable = canBuy;
    }

    void RefreshCoinUI()
    {
        int cost = GetCoinCost();
        int diamondCurrent = PlayerInventory.Instance.GetItemQuantity(2);

        diamondCostBuyCoin.text = cost.ToString();

        bool canBuy = diamondCurrent >= cost;

        diamondCostBuyCoin.color = canBuy ? Color.white : Color.red;
        buttonBuyCoin.interactable = canBuy;
    }

    // ================= COST =================

    int GetStaminaCost()
    {
        int cost = (timesBuyStamina / 2 + 1) * 50;
        return Mathf.Min(cost, 300);
    }

    int GetCoinCost()
    {
        int level = timesBuyCoin / 2 + 1;

        switch (level)
        {
            case 1: return 10;
            case 2: return 20;
            case 3: return 40;
            case 4: return 80;
            default: return 100;
        }
    }
}
