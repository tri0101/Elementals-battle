using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "DBS/FightSoulDtabase")]

public class FightSoulDatabase : ScriptableObject
{
    public List<FightSoulInfo> souls;

    private Dictionary<int, FightSoulInfo> soulDict;
    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        soulDict = new Dictionary<int, FightSoulInfo>();
        foreach (var soul in souls)
            soulDict[soul.soulID] = soul;
    }

    public FightSoulInfo GetSoulInfo(int id)
    {
        if (soulDict == null) Init();

        return soulDict.TryGetValue(id, out var hero) ? hero : null;
    }
    public FightSoulType GetTypeSoul(int id)
    {
        if(soulDict == null) Init();
        return soulDict.TryGetValue(id, out var soul) ? soul.fightSoulType : FightSoulType.ManaSoul; 
    }
}
