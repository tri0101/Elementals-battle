using UnityEngine;
[System.Serializable]
public class DropItemData
{
    public int itemId;
    public int minAmount;
    public int maxAmount;
    [Range(0f, 1f)]
    public float dropRate; // 0.3 = 30%
}
