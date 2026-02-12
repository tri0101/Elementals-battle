using System.Collections.Generic;
using UnityEngine;
public static class DropSystem
{
    public static Dictionary<int, int> RollDrops()
    {
        StageConfig stageConfig = StageContext.selectedStage;
        Dictionary<int, int> items = new Dictionary<int, int>();


        if (stageConfig == null || stageConfig.dropItems == null || stageConfig.dropItems.Count == 0)
            return items;

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
            items[drop.itemId] = amount;


            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddItem(drop.itemId, amount);
            }
            else
            {
                Debug.LogWarning("PlayerInventory.Instance is null. Drop was rolled but not added to inventory.");
            }
        }
        return items;
        
    }
}
