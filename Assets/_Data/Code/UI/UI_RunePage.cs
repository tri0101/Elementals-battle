using System.Collections.Generic;
using UnityEngine;

public class UI_RunePage : MonoBehaviour
{
    public List<UI_RuneSlot> slots = new List<UI_RuneSlot>();
    public UI_StatPanel uiStat;
    private void Awake()
    {
        slots.Clear();

        // Lấy toàn bộ UI_RuneSlot là con của RunePage
        foreach (Transform child in transform)
        {
            UI_RuneSlot slot = child.GetComponent<UI_RuneSlot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }

        Debug.Log("Total Rune Slots: " + slots.Count);
    }

    public bool TryAddRune(RuneData rune, UIRuneItem source)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetRune(rune, source);
                return true;
            }
        }
        return false;
    }
    private void Update()
    {
        uiStat.UpdateStats(CalculateTotalStats());
    }
    public Dictionary<RuneStat, float> CalculateTotalStats()
    {
        Dictionary<RuneStat, float> totalStats = new();

        foreach (var slot in slots)
        {
            if (slot.currentRune == null) continue;

            foreach (var statValue in slot.currentRune.stats)
            {
                if (!totalStats.ContainsKey(statValue.stat))
                    totalStats[statValue.stat] = 0;

                totalStats[statValue.stat] += statValue.value;
            }
        }

        return totalStats;
    }

}
