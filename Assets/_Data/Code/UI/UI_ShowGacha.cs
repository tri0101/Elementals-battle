using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShowGacha : MonoBehaviour
{
    public Transform prefabHeroSample;
    public Transform prefabHeroShardSample;
    public HeroDatabase heroDatabase;

    public void OnClickRoll()
    {
        GachaResult result = GachaManager.Instance.Roll();
        ShowResult(result);
    }
    public void OnClickRollTen()
    {
        List<GachaResult> results = new();

        for (int i = 0; i < 10; i++)
            results.Add(GachaManager.Instance.Roll());

        foreach (var result in results)
            ShowResult(result);
    }
    void ShowResult(GachaResult result)
    {
        HeroInfo hero = heroDatabase.GetHero(result.heroId);
        if (hero == null) return;

        Transform item;

        if (result.type == GachaResultType.Hero)
        {
            item = Instantiate(prefabHeroSample, transform);
        }
        else
        {
            item = Instantiate(prefabHeroShardSample, transform);
        }

        
        Image icon = item.Find("HeroIcon").GetComponent<Image>();
        icon.sprite = hero.iconFace;
        item.SetParent(transform);
        item.gameObject.SetActive(true);
    }

}
