using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LimitedOfferEntry
{
    public int id;
    public int exchangeCount;
}



public class LimitedOfferState : Subject
{
    public static LimitedOfferState Instance { get; private set; }

    readonly Dictionary<int, int> exchangeById = new Dictionary<int, int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetRedeemedCount(int id)
    {
        return exchangeById.TryGetValue(id, out int v) ? v : 0;
    }

    public void IncrementRedeem(int id)
    {
        exchangeById[id] = GetRedeemedCount(id) + 1;
        NotifyObservers();
    }

    public bool CanRedeem(BannerLimitedOfferData data)
    {
        if (data == null) return false;
        return GetRedeemedCount(data.id) < data.exchangeLimit;
    }

    //public TokenExchangeSaveData Export()
    //{
    //    var data = new TokenExchangeSaveData();

    //    foreach (var kv in exchangeById)
    //    {
    //        data.entries.Add(new TokenExchangeEntry
    //        {
    //            heroId = kv.Key,
    //            redeemedCount = kv.Value
    //        });
    //    }

    //    return data;
    //}

    //public void Import(TokenExchangeSaveData data)
    //{
    //    exchangeById.Clear();

    //    if (data == null || data.entries == null)
    //        return;

    //    for (int i = 0; i < data.entries.Count; i++)
    //    {
    //        var e = data.entries[i];
    //        if (e == null) continue;
    //        exchangeById[e.heroId] = Mathf.Max(0, e.redeemedCount);
    //    }
    //}
}