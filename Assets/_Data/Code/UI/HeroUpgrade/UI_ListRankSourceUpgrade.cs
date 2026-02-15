using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_ListRankSourceUpgrade : MonoBehaviour, IObserver
{
    [Header("Config")]
    [SerializeField] private HeroRankConfig rankConfig;
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private Transform contentForAnim;
    [SerializeField] private Transform panelUpgradeSuccess;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TextMeshProUGUI amountTextCoin;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private UI_ListHeroUpgrade uiHeroUpgrade;

    HeroViewData currentHero;

    bool isProcessing = false;
    void OnEnable()
    {
        PlayerInventory.Instance.AddObserver(this);
    }
    void OnDisable()
    {
        PlayerInventory.Instance.RemoveObbserver(this);
    }
    public void Setup(HeroViewData hero)
    {
        currentHero = hero;


        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        Build();
    }

    void Build()
    {
        Clear();
        if (currentHero == null) return;


        RankRequirement req = rankConfig.rankRequirements
            .Find(r => r.rank == currentHero.instance.rank);

        if (req == null)
        {

            if (amountTextCoin != null) amountTextCoin.text = "-";
            if (upgradeButton != null) upgradeButton.interactable = false;
            return;
        }

        CreateSlot(100, GetRequiredAmount(req, 100));


        CreateSlotByRole(req, currentHero.info.role, true);


        CreateSlotByRole(req, currentHero.info.role, false);

        ItemCost coinCost = req.costs.Find(c => c.itemId == 1);
        int requiredCoin = coinCost != null ? coinCost.amount : 0;
        int ownedCoin = GetOwned(1);
        if (amountTextCoin != null)
            amountTextCoin.text = $"{requiredCoin}";
        RefreshAmountCoin();

        var compiled = CompileRequirements(req, currentHero.info.role);
        bool can = CanUpgrade(compiled);
       
        if (upgradeButton != null)
            upgradeButton.interactable = can &&  !isProcessing;
    }
    void RefreshAmountCoin()
    {
        if(int.Parse(amountTextCoin.text) > PlayerInventory.Instance.GetItemQuantity(1))
        {
            amountTextCoin.color = Color.red;
        }
        else
        {
            amountTextCoin.color = Color.white;
            upgradeButton.interactable = true && !isProcessing;
        }
    }
    public void OnNotify(object data)
    {

        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 1)
                RefreshAmountCoin();
        }
    }

    void CreateSlot(int itemId, int required)
    {
        ItemData itemData = itemDatabase.GetItem(itemId);
        if (itemData == null) return;

        int owned = GetOwned(itemId);

        var go = Instantiate(itemPrefab, content);
        var uiItem = go.GetComponent<UI_RankSourceUpgradeItem>();
        uiItem.Setup(itemData, owned, required);


        // rank 1-4 => đen, rank 5-8 => xanh
        if (uiItem != null && uiItem.Icon != null && currentHero != null && currentHero.instance != null)
        {
            int rank = currentHero.instance.rank;
            if (rank >= 1 && rank <= 4)
            {
                uiItem.GetComponent<Image>().color = Color.black;
            }
            else if (rank >= 5 && rank <= 8)
            {
                uiItem.GetComponent<Image>().color = new Color(73f / 255f, 1f, 115f / 255f);
            }
            else
            {
                uiItem.GetComponent<Image>().color = Color.white;
            }
        }
    }

    void CreateSlotByRole(RankRequirement req, RoleHero role, bool main)
    {
        List<string> keys = main
            ? GetMainKeys(role)
            : GetSecondaryKeys(role);

        int multiplier = main ? 2 : 1;

        foreach (string key in keys)
        {
            ItemCost cost = req.costs.Find(c =>
            {
                ItemData data = itemDatabase.GetItem(c.itemId);
                return data != null && data.name.Contains(key);
            });

            if (cost == null) continue;

            CreateSlot(cost.itemId, cost.amount * multiplier);
        }
    }

    int GetRequiredAmount(RankRequirement req, int itemId)
    {
        ItemCost cost = req.costs.Find(c => c.itemId == itemId);
        return cost != null ? cost.amount : 0;
    }

    int GetOwned(int itemId)
    {
        
        return PlayerInventory.Instance.GetItemQuantity(itemId);
    }


    void Clear()
    {
        foreach (Transform c in content)
            Destroy(c.gameObject);
    }


    List<string> GetMainKeys(RoleHero role)
    {
        switch (role)
        {
            case RoleHero.Tank:
                return new() { "Shield" };

            case RoleHero.DPS:
                return new() { "Sword" };

            case RoleHero.Support:
                return new() { "Star" };

            default:
                return new();
        }
    }


    List<string> GetSecondaryKeys(RoleHero role)
    {
        switch (role)
        {
            case RoleHero.Tank:
                return new() { "Sword", "Star" };

            case RoleHero.DPS:
                return new() { "Shield", "Star" };

            case RoleHero.Support:
                return new() { "Shield", "Sword" };

            default:
                return new();
        }
    }




    List<ItemCost> CompileRequirements(RankRequirement req, RoleHero role)
    {
        var list = new List<ItemCost>();
        if (req == null) return list;


        AddOrAccumulate(list, 100, GetRequiredAmount(req, 100));

        CompileRoleSlotsToList(list, req, role, true);


        CompileRoleSlotsToList(list, req, role, false);


        ItemCost coin = req.costs.Find(c => c.itemId == 1);
        if (coin != null) AddOrAccumulate(list, 1, coin.amount);

        return list;
    }


    void CompileRoleSlotsToList(List<ItemCost> outList, RankRequirement req, RoleHero role, bool main)
    {
        List<string> keys = main ? GetMainKeys(role) : GetSecondaryKeys(role);
        int multiplier = main ? 2 : 1;

        foreach (string key in keys)
        {
            ItemCost cost = req.costs.Find(c =>
            {
                ItemData data = itemDatabase.GetItem(c.itemId);
                return data != null && data.name.Contains(key);
            });

            if (cost == null) continue;

            AddOrAccumulate(outList, cost.itemId, cost.amount * multiplier);
        }
    }

    void AddOrAccumulate(List<ItemCost> list, int itemId, int amount)
    {
        if (amount <= 0) return;
        var e = list.Find(x => x.itemId == itemId);
        if (e != null) e.amount += amount;
        else list.Add(new ItemCost { itemId = itemId, amount = amount });
    }


    bool CanUpgrade(List<ItemCost> compiled)
    {
        if (compiled == null || compiled.Count == 0) return false;
        foreach (var c in compiled)
        {
            if(c.itemId == 1) // coin
            {
                continue;
            }
            int owned = GetOwned(c.itemId);
            if (owned < c.amount) return false;
        }
        return true;
    }

    void OnUpgradeClicked()
    {
        if (isProcessing) return; 

        if (currentHero == null) return;
        if(int.Parse(amountTextCoin.text) > PlayerInventory.Instance.GetItemQuantity(1))
        {
            UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyCoin();
            return;
        }
        RankRequirement req = rankConfig.rankRequirements
            .Find(r => r.rank == currentHero.instance.rank);

        if (req == null) return;

        var compiled = CompileRequirements(req, currentHero.info.role);


        HeroInstance prevSnapshot = new HeroInstance
        {
            heroId = currentHero.instance.heroId,
            level = currentHero.instance.level,
            currentExp = currentHero.instance.currentExp,
            star = currentHero.instance.star,
            rank = currentHero.instance.rank,
            shard = currentHero.instance.shard
        };


        int prevPower = -1;
        var growth = uiHeroUpgrade != null && uiHeroUpgrade.Header != null ? uiHeroUpgrade.Header.GrowthConfig : null;
        if (growth != null)
        {
            var prevStat = HeroStatCalculator.Calculate(currentHero.info, prevSnapshot, growth);
            prevPower = Mathf.RoundToInt(prevStat.power);
        }


        GameObject prevClone = null;
        if (uiHeroUpgrade != null && uiHeroUpgrade.Content != null)
        {
            foreach (Transform ch in uiHeroUpgrade.Content)
            {
                var comp = ch.GetComponent<UI_HeroUpgradeItem>();
                if (comp == null || comp.Icon == null) continue;

                if (comp.Icon.sprite == currentHero.info.iconFace)
                {
                    prevClone = Instantiate(ch.gameObject);
                    var btn = prevClone.GetComponent<Button>();
                    if (btn != null)
                        btn.enabled = false;
                    prevClone.SetActive(false);
                    break;
                }
            }
        }

        isProcessing = true;
        if (upgradeButton != null) upgradeButton.interactable = false;


        StartCoroutine(AnimateItemsThenUpgrade(req, compiled, prevSnapshot, prevPower, prevClone));
    }

    IEnumerator AnimateItemsThenUpgrade(RankRequirement req, List<ItemCost> compiled, HeroInstance prevSnapshot, int prevPower, GameObject prevUIClone)
    {
        // Danh sách các gameobject đang animate để sau đó destroy
        List<GameObject> animatedGameObjects = new List<GameObject>();

        if (contentForAnim != null && content != null)
        {
            // Lấy tối đa 4 child đầu tiên của content
            List<Transform> items = new List<Transform>();
            foreach (Transform c in content)
            {
                items.Add(c);
                if (items.Count >= 4) break;
            }

            if (items.Count > 0)
            {
                // Reparent giữ world position để không nhảy vị trí trên màn hình
                foreach (var t in items)
                {
                    if (t == null) continue;
                    t.SetParent(contentForAnim, worldPositionStays: true);
                    animatedGameObjects.Add(t.gameObject);
                }

                // Tính vị trí target ở world space dựa trên contentForAnim (RectTransform.TransformPoint)
                RectTransform rtParent = contentForAnim as RectTransform;
                Vector3 targetLocal = new Vector3(-225f, 50f, 0f); // vị trí đích trong local của contentForAnim
                Vector3 targetWorld = rtParent != null ? rtParent.TransformPoint(targetLocal) : (Vector3)targetLocal;

                float duration = 0.45f;
                float elapsed = 0f;

                Vector3[] starts = new Vector3[items.Count];
                Vector3[] startScales = new Vector3[items.Count];
                Vector3[] targetScales = new Vector3[items.Count];

                for (int i = 0; i < items.Count; i++)
                {
                    starts[i] = items[i].position;
                    startScales[i] = items[i].localScale;
                    // target scale: x,y -> 0.5 (giữ z ban đầu hoặc 1 nếu z==0)
                    float targetZ = startScales[i].z != 0f ? startScales[i].z : 1f;
                    targetScales[i] = new Vector3(0.5f, 0.5f, targetZ);
                }

                // Animate vị trí + scale theo thời gian với easing SmoothStep
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float eased = Mathf.SmoothStep(0f, 1f, t);

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] == null) continue;
                        items[i].position = Vector3.Lerp(starts[i], targetWorld, eased);
                        items[i].localScale = Vector3.Lerp(startScales[i], targetScales[i], eased);
                    }

                    yield return null;
                }

                // Đảm bảo vị trí + scale cuối cùng chính xác
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] == null) continue;
                    items[i].position = targetWorld;
                    items[i].localScale = targetScales[i];
                }

                // Pause ngắn để cảm nhận animation
                yield return new WaitForSeconds(0.08f);

                // Ẩn và destroy các object animate để tránh duplicate visuals
                foreach (var go in animatedGameObjects)
                {
                    if (go == null) continue;
                    go.SetActive(false);
                    Destroy(go);
                }

                // Delay 1 frame để cập nhật hierarchy UI
                yield return null;
            }
        }

        // Sau animation: gọi service để thực hiện consume items và tăng rank
        bool ok = HeroUpgradeService.Instance.UpgradeRank(currentHero.instance, compiled);
        if (!ok)
        {
            Debug.Log("Upgrade failed or not enough materials.");
            Build(); // rebuild UI để phản ánh trạng thái (defensive)
            // cleanup prev clone nếu có
            if (prevUIClone != null) Destroy(prevUIClone);
            isProcessing = false;
            if (upgradeButton != null)
            {
                var compiledReq = rankConfig.rankRequirements.Find(r => r.rank == currentHero.instance.rank);
                var compList = compiledReq != null ? CompileRequirements(compiledReq, currentHero.info.role) : null;
                upgradeButton.interactable = compList != null && CanUpgrade(compList);
            }
            yield break;
        }

        // Tính power mới sau khi nâng (dùng growthConfig từ header nếu có)
        int newPower = -1;
        var growth2 = uiHeroUpgrade != null && uiHeroUpgrade.Header != null ? uiHeroUpgrade.Header.GrowthConfig : null;
        if (growth2 != null)
        {
            var newStat = HeroStatCalculator.Calculate(currentHero.info, currentHero.instance, growth2);
            newPower = Mathf.RoundToInt(newStat.power);
        }

        // Rebuild UI & refresh list/header để có element UI mới phản ánh rank mới
        Build();
        if (uiHeroUpgrade != null)
            uiHeroUpgrade.Refresh();

        // Capture visual "after" bằng cách clone lại UI_HeroUpgradeItem tương ứng (sau khi refresh)
        GameObject afterClone = null;
        if (uiHeroUpgrade != null && uiHeroUpgrade.Content != null)
        {
            foreach (Transform ch in uiHeroUpgrade.Content)
            {
                var comp = ch.GetComponent<UI_HeroUpgradeItem>();
                if (comp == null || comp.Icon == null) continue;
                if (comp.Icon.sprite == currentHero.info.iconFace)
                {
                    afterClone = Instantiate(uiHeroUpgrade.HeroUpgradeItemPrefab);
                    afterClone.GetComponent<UI_HeroUpgradeItem>()
                        .Setup(currentHero);
                    var btn = afterClone.GetComponent<Button>();
                    if (btn != null)
                        btn.enabled = false;
                    afterClone.SetActive(true);
                    break;
                }
            }
        }

        // Hiển thị panel success: gọi ShowSucces của UI_HeroUpSuccess, nội bộ hàm đó sẽ clear children cũ
        if (panelUpgradeSuccess != null)
        {
            var successUI = panelUpgradeSuccess.GetComponent<UI_HeroUpSuccess>();
            if (successUI != null)
            {
                successUI.ShowSucces(prevUIClone, afterClone, prevPower, newPower);
            }
            else
            {
                panelUpgradeSuccess.gameObject.SetActive(true);
            }
        }

        // Hủy cleanup các clone tạm (ShowSucces đã instantiate lại nội dung hiển thị nếu cần)
        if (prevUIClone != null) Destroy(prevUIClone);
        if (afterClone != null) Destroy(afterClone);


        if (uiHeroUpgrade != null)
            uiHeroUpgrade.Refresh();

       
        isProcessing = false;
        if (upgradeButton != null)
        {
            var compiledReq = rankConfig.rankRequirements.Find(r => r.rank == currentHero.instance.rank);
            var compList = compiledReq != null ? CompileRequirements(compiledReq, currentHero.info.role) : null;
            upgradeButton.interactable = compList != null && CanUpgrade(compList);
        }
    }
}