using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class TokenExchangeEntry
{
    public int heroId;
    public int redeemedCount;
}



public class TokenExchangeState : Subject
{
    public static TokenExchangeState Instance { get; private set; }

    readonly Dictionary<int, int> redeemedByHeroId = new Dictionary<int, int>();

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

    public int GetRedeemedCount(int heroId)
    {
        return redeemedByHeroId.TryGetValue(heroId, out int v) ? v : 0;
    }

    public void IncrementRedeem(int heroId)
    {
        redeemedByHeroId[heroId] = GetRedeemedCount(heroId) + 1;
        NotifyObservers();
    }

    public bool CanRedeem(BannerTokenExchangeData data)
    {
        if (data == null) return false;
        return GetRedeemedCount(data.heroId) < data.redeemLimit;
    }

    public TokenExchangeSaveData Export()
    {
        var data = new TokenExchangeSaveData();

        foreach (var kv in redeemedByHeroId)
        {
            data.entries.Add(new TokenExchangeEntry
            {
                heroId = kv.Key,
                redeemedCount = kv.Value
            });
        }

        return data;
    }

    public void Import(TokenExchangeSaveData data)
    {
        redeemedByHeroId.Clear();

        if (data == null || data.entries == null)
            return;

        for (int i = 0; i < data.entries.Count; i++)
        {
            var e = data.entries[i];
            if (e == null) continue;
            redeemedByHeroId[e.heroId] = Mathf.Max(0, e.redeemedCount);
        }
    }
}