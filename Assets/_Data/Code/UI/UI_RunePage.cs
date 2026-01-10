using System.Collections.Generic;
using UnityEngine;

public class UI_RunePage : MonoBehaviour
{
    public List<UI_RuneSlot> slots = new List<UI_RuneSlot>();

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

    public bool TryAddRune(RuneData rune)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetRune(rune);
                return true;
            }
        }
        return false;
    }
}
