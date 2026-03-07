using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class BannerTokenExchangeData
{
    public int heroId;
    public int redeemLimit; //số lần được phép đổi
    public int amountCost;
    
}
[CreateAssetMenu(menuName = "Gacha/BanerTokenExchange")]
public class BannerTokenExchange : ScriptableObject
{
    [Header("Banner Token Exchange Data")]
    public List<BannerTokenExchangeData> exchangeDataList;
}
