using System;
using System.Collections.Generic;

[Serializable]
public class FormationData
{
    // slotIndex (1..6) -> heroId
    public Dictionary<int, int> slots = new Dictionary<int, int>();

    public FormationData()
    {
        for (int i = 1; i <= 6; i++)
            slots[i] = 0; // 0 = empty
    }

    public int GetHero(int slot)
    {
        return slots.ContainsKey(slot) ? slots[slot] : 0;
    }

    public void SetHero(int slot, int heroId)
    {
        slots[slot] = heroId;
    }

    public void RemoveHero(int slot)
    {
        if (slots.ContainsKey(slot))
            slots[slot] = 0;
    }
}
