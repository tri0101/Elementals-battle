using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class ItemAndAmount
{
    public int itemId;
    public int amount;
}
[System.Serializable]
public class BannerLimitedOfferData
{
    public int id;
    public List<ItemAndAmount> itemNeed;
    public List<ItemAndAmount> itemExchange;
    public int exchangeLimit;

}
[CreateAssetMenu(menuName = "Gacha/BannerLimitedOffer")]
public class BannerLimitedOffer : ScriptableObject
{
    [Header("Banner Token Exchange Data")]
    public List<BannerLimitedOfferData> exchangeDataList;
}
