using UnityEngine;

public class UI_FormationManager : MonoBehaviour
{
    public Transform slotRoot;
    const int MAX_SLOT = 6;


    private int?[] formationHeroIds = new int?[MAX_SLOT];

    public bool IsHeroInFormation(HeroInfo hero)
    {
        if (hero == null) return false;

        for (int i = 0; i < MAX_SLOT; i++)
            if (formationHeroIds[i] == hero.ID)
                return true;

        return false;
    }

    public bool TryAddHero(UI_HeroChooseItem heroItem)
    {
        if (heroItem == null || heroItem.IsInFormation)
            return false;

        int heroId = heroItem.Data.instance.heroId;


        if (IsHeroInFormation(heroItem.Data.info))
            return false;

        for (int i = 0; i < MAX_SLOT; i++)
        {
            if (formationHeroIds[i] == null)
            {
                Transform slot = slotRoot.Find($"Slot{i + 1}");
                if (slot == null) continue;

                formationHeroIds[i] = heroId;
                PlaceHero(heroItem, slot);
                return true;
            }
        }

        Debug.Log("Formation full");
        return false;
    }

    public void RemoveHero(UI_HeroChooseItem heroItem, Transform inventoryContent)
    {
        if (heroItem == null) return;

        int heroId = heroItem.Data.instance.heroId;

        for (int i = 0; i < MAX_SLOT; i++)
            if (formationHeroIds[i] == heroId)
                formationHeroIds[i] = null;

        heroItem.transform.SetParent(inventoryContent, false);
        ResetRect(heroItem.GetComponent<RectTransform>());
        heroItem.SetInFormation(false);
    }

  
    void PlaceHero(UI_HeroChooseItem hero, Transform slot)
    {
        hero.transform.SetParent(slot, false);
        StretchToParent(hero.GetComponent<RectTransform>());
        hero.SetInFormation(true);
    }

    void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }

    void ResetRect(RectTransform rt)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }
}
