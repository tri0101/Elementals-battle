using UnityEngine;

public class UI_FormationManager : MonoBehaviour
{
    public UI_FormationSlot[] slots = new UI_FormationSlot[6];
    public HeroDatabase heroDatabase;

    FormationData formation;

    void OnEnable()
    {
        formation = FormationManager.LoadFormation();
        RefreshAllSlots();

      
        TryApplyPendingSelection();
    }

    void Update()
    {
        
        if (FormationContext.SelectedHero != null && FormationContext.SelectedSlotIndex > 0)
        {
            TryApplyPendingSelection();
        }
    }

    void RefreshAllSlots()
    {
        if (formation == null) formation = FormationManager.LoadFormation();

        for (int i = 0; i < slots.Length; i++)
        {
            int heroId = FormationManager.GetHeroAtSlot(formation, i + 1);
            if (slots[i] != null)
                slots[i].SetupSlot(heroId, heroDatabase);
        }
    }

    void TryApplyPendingSelection()
    {
        if (FormationContext.SelectedHero == null || FormationContext.SelectedSlotIndex < 1 || FormationContext.SelectedSlotIndex > 6)
            return;

        int slot = FormationContext.SelectedSlotIndex;
        var hv = FormationContext.SelectedHero;
        if (hv == null)
        {
            FormationContext.Clear();
            return;
        }

      
        bool ok = FormationManager.AssignHeroToSlot(formation, slot, hv.instance.heroId);
        if (!ok)
        {
            Debug.LogWarning("Hero already placed in other slot. Remove first or choose another hero.");
           
        }
        else
        {
            
            formation = FormationManager.LoadFormation();
            RefreshAllSlots();
        }

        FormationContext.Clear();
    }

    
    public void RemoveHeroFromSlot(int slotIndex)
    {
        formation = FormationManager.LoadFormation();
        formation.RemoveHero(slotIndex);
        FormationManager.SaveFormation(formation);
        RefreshAllSlots();
    }
}