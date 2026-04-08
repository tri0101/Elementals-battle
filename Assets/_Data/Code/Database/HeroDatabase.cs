using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/HeroDatabase")]
public class HeroDatabase : ScriptableObject
{
    public List<HeroInfo> heroes;

    private Dictionary<int, HeroInfo> heroDict;

    private void OnEnable()
    {
        Init();
    }
    
    public void Init()
    {
        heroDict = new Dictionary<int, HeroInfo>();
        foreach (var hero in heroes)
            heroDict[hero.ID] = hero;
    }

    public HeroInfo GetHero(int id)
    {
        if (heroDict == null) Init();

        return heroDict.TryGetValue(id, out var hero) ? hero : null;
    }
}
