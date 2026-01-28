using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FormationSlot
{
    public int slot;
    public int heroId;
}

[Serializable]
public class FormationData
{
    public List<FormationSlot> slots = new List<FormationSlot>();

    public FormationData()
    {
        slots = new List<FormationSlot>();
        for (int i = 1; i <= 6; i++)
            slots.Add(new FormationSlot { slot = i, heroId = 0 });
    }

    public FormationSlot GetSlot(int slotIndex) => slots.Find(s => s.slot == slotIndex);

    public void SetHero(int slotIndex, int heroId)
    {
        var s = GetSlot(slotIndex);
        if (s != null) s.heroId = heroId;
    }

    public void RemoveHero(int slotIndex)
    {
        var s = GetSlot(slotIndex);
        if (s != null) s.heroId = 0;
    }

    public bool HasHero(int heroId)
    {
        if (heroId <= 0) return false;
        foreach (var s in slots)
            if (s != null && s.heroId == heroId) return true;
        return false;
    }
}