using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BattleResult : MonoBehaviour
{

    [Header("Transform")]
    public Transform backHeroExp;
    public Transform listHeroPrefab;
    [Header("Stage")]
    public StageConfig stageConfig;

    [Header("Drop Result")]
    public Dictionary<int, int> itemDrop = new Dictionary<int, int>(); // itemID , amount

    Dictionary<int, bool> listHeroStatus = new Dictionary<int, bool>();// true = live , false = dead
    public UI_StageReward uiStageReward;
    public int heroTotal;

    private void Awake()
    {
        stageConfig = StageContext.selectedStage;

    }
    void Start()
    {

        listHeroStatus.Clear();

    }

    public void SetExpPlus()
    {
        foreach (Transform child in backHeroExp)
        {
            var ui = child.GetComponent<UI_HeroExpPlus>();
            if (ui != null)
            {
                if (listHeroStatus[int.Parse(ui.name)])
                {
                    ui.SetExpPlus(stageConfig.expForAliveHero);
                }
                else
                {
                    ui.SetExpPlus(stageConfig.expForAliveHero / 2);
                }
            }

        }
    }
    public void SetList(int indexSlot, bool statusValue)
    {

        listHeroStatus[indexSlot] = statusValue;
    }

    public void CheckHeroesLost()
    {
        int activeCount = 0;
        int totalHero = listHeroPrefab.childCount;

        foreach (Transform child in listHeroPrefab)
        {
            if (child.gameObject.activeSelf)
            {
                activeCount++;
            }
        }

        if (activeCount >= totalHero || totalHero == 1)
        {
            uiStageReward.SetStarGain(3);
            ProgressManager.Instance.UpdateStarGain(3, stageConfig.stageID);
        }
        else
        {
            switch (stageConfig.stageCondition)
            {
                case StageCondition.HeroLost:
                    int heroLostParam = stageConfig.conditionParam;
                    if ((totalHero - activeCount) <= heroLostParam)
                    {
                        uiStageReward.SetStarGain(2);
                        ProgressManager.Instance.UpdateStarGain(2, stageConfig.stageID);
                    }
                    else
                    {
                        uiStageReward.SetStarGain(1);
                        ProgressManager.Instance.UpdateStarGain(1, stageConfig.stageID);
                    }
                    break;
            }
        }
    }

    public void  RollDropsAndAddToInventory()
    {
        itemDrop.Clear();

        if (stageConfig == null || stageConfig.dropItems == null || stageConfig.dropItems.Count == 0)
            return; ;

        for (int i = 0; i < stageConfig.dropItems.Count; i++)
        {
            DropItemData drop = stageConfig.dropItems[i];
            if (drop == null) continue;

            float rate = Mathf.Clamp01(drop.dropRate);
            if (Random.value > rate)
                continue;

            int min = Mathf.Max(0, drop.minAmount);
            int max = Mathf.Max(min, drop.maxAmount);
            int amount = Random.Range(min, max + 1); // int max is exclusive, so +1

            if (amount <= 0)
                continue;
            itemDrop[drop.itemId] = amount;


            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddItem(drop.itemId, amount);
            }
            else
            {
                Debug.LogWarning("PlayerInventory.Instance is null. Drop was rolled but not added to inventory.");
            }
        }
        uiStageReward.SetItemDrop(itemDrop);
    }
    
}