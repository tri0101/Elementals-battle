using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FormationManager : MonoBehaviour
{
    [Header("UI")]
    public Transform slotRoot;
    public GameObject heroItemPrefab;

    [Header("Refs")]
    public UI_PanelChooseHero panelChooseHero;

    private const int MAX_SLOT = 6;
    private int[] formationHeroIds;

 
    private bool isBusy = false;
    public bool IsBusy => isBusy;

    void Awake()
    {
        formationHeroIds = FormationManager.Load();
    }

    void OnEnable()
    {
        StartCoroutine(RebuildNextFrame());
    }

    void OnDisable()
    {
        FormationManager.Save(formationHeroIds);
    }

    private bool BeginOp()
    {
        if (isBusy) return false;
        isBusy = true;
        return true;
    }

    private void EndOp()
    {
        isBusy = false;
    }

    IEnumerator RebuildNextFrame()
    {
        yield return null;
        RebuildFormationUI();
    }

    public bool IsHeroInFormation(int heroId)
    {
        for (int i = 1; i <= MAX_SLOT; i++)
            if (formationHeroIds[i] == heroId) return true;
        return false;
    }
    public bool CheckEmpty()
    {
        if (formationHeroIds == null || formationHeroIds.Length <= 1)
            return true;

        for (int i = 1; i <= MAX_SLOT && i < formationHeroIds.Length; i++)
        {
            if (formationHeroIds[i] != -1)
                return false;
        }
        return true;
    }

    public bool TryAddHero(UI_HeroChooseItem heroItem)
    {
        if (heroItem == null || heroItem.IsInFormation) return false;
        if (!BeginOp()) return false;

        try
        {
            int heroId = heroItem.Data.instance.heroId;

            for (int i = 1; i <= MAX_SLOT; i++)
            {
                if (formationHeroIds[i] != -1) continue;

                Transform slot = slotRoot.Find($"Slot{i}");
                if (slot == null)
                {
                    Debug.LogError($"Slot{i} NULL");
                    return false;
                }

                formationHeroIds[i] = heroId;
                PlaceHero(heroItem, slot);

                FormationManager.Save(formationHeroIds);
                panelChooseHero.RefreshPower();
                // inventory item vừa bị lấy đi -> refresh list ở cuối frame cho an toàn
                StartCoroutine(RefreshInventoryNextFrame());
                return true;
            }

            return false;
        }
        finally
        {
            EndOp();
        }
    }

    public void RemoveHero(UI_HeroChooseItem heroItem, Transform inventory)
    {
        if (heroItem == null || inventory == null) return;
        if (!BeginOp()) return;

        try
        {
            int heroId = heroItem.Data.instance.heroId;

            for (int i = 1; i <= MAX_SLOT; i++)
                if (formationHeroIds[i] == heroId) formationHeroIds[i] = -1;

            heroItem.transform.SetParent(inventory, false);
            ResetRect(heroItem.GetComponent<RectTransform>());
            heroItem.SetInFormation(false);

            FormationManager.Save(formationHeroIds);
            panelChooseHero.RefreshPower();
           
            StartCoroutine(RefreshInventoryNextFrame());
        }
        finally
        {
            EndOp();
        }
    }

    public void SwapHeroes(UI_HeroChooseItem heroA, UI_HeroChooseItem heroB)
    {
        if (heroA == null || heroB == null) return;
        if (!BeginOp()) return;

        try
        {
            int indexA = GetHeroIndexInFormation(heroA.Data.instance.heroId);
            int indexB = GetHeroIndexInFormation(heroB.Data.instance.heroId);
            if (indexA == -1 || indexB == -1) return;

            int temp = formationHeroIds[indexA];
            formationHeroIds[indexA] = formationHeroIds[indexB];
            formationHeroIds[indexB] = temp;

            Transform slotA = slotRoot.Find($"Slot{indexA}");
            Transform slotB = slotRoot.Find($"Slot{indexB}");
            if (slotA == null || slotB == null) return;

            PlaceHero(heroA, slotB);
            PlaceHero(heroB, slotA);

            FormationManager.Save(formationHeroIds);
            panelChooseHero.RefreshPower();
        }
        finally
        {
            EndOp();
        }
    }

    public void MoveHeroToSlot(UI_HeroChooseItem heroItem, Transform targetSlot)
    {
        if (heroItem == null || targetSlot == null) return;
        if (!BeginOp()) return;

        try
        {
            int heroId = heroItem.Data.instance.heroId;
            int currentIndex = GetHeroIndexInFormation(heroId);
            //int targetIndex = GetSlotIndex(targetSlot);
            int targetIndex = targetSlot.name[^1] - '0';

            if (currentIndex == -1 || targetIndex == -1) return;
            if (currentIndex == targetIndex) return;


            if (formationHeroIds[targetIndex] != -1) return;

            formationHeroIds[currentIndex] = -1;
            formationHeroIds[targetIndex] = heroId;

            PlaceHero(heroItem, targetSlot);

            FormationManager.Save(formationHeroIds);
            panelChooseHero.RefreshPower();
        }
        finally
        {
            EndOp();
        }
    }

    private IEnumerator RefreshInventoryNextFrame()
    {
        
        yield return null;
        if (panelChooseHero != null)
            panelChooseHero.LoadHeroes();
    }

    void RebuildFormationUI()
    {
        if (slotRoot == null || heroItemPrefab == null) return;

        foreach (Transform slot in slotRoot)
        {
            foreach (Transform child in slot)
            {
                if (child.name.StartsWith("Hero"))
                    Destroy(child.gameObject);
            }
        }

        var allHeroes = PlayerInventory.Instance.GetHeroViewList(DatabaseManager.Instance.HeroDatabase);

        for (int i = 1; i <= MAX_SLOT; i++)
        {
            int savedId = formationHeroIds[i];
            if (savedId == -1) continue;

            HeroViewData heroData = allHeroes.Find(h => h.instance.heroId == savedId);
            if (heroData == null) continue;

            Transform slot = slotRoot.Find($"Slot{i}");
            if (slot == null) continue;

            GameObject go = Instantiate(heroItemPrefab, slot);
            var item = go.GetComponent<UI_HeroChooseItem>();
            item.Setup(heroData, this, panelChooseHero);
            item.SetInFormation(true);
            Stretch(go.GetComponent<RectTransform>());
        }
    }

    public void PlaceHero(UI_HeroChooseItem hero, Transform slot)
    {
        if (slot == null)
        {
            Debug.LogError("PlaceHero: slot NULL");
            return;
        }

        hero.transform.SetParent(slot, false);
        Stretch(hero.GetComponent<RectTransform>());
        hero.SetInFormation(true);
    }

    void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }

    public void ResetRect(RectTransform rt)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }

    private int GetHeroIndexInFormation(int heroId)
    {
        for (int i = 1; i <= formationHeroIds.Length; i++)
            if (formationHeroIds[i] == heroId) return i;
        return -1;
    }

    private int GetSlotIndex(Transform slot)
    {
        for (int i = 0; i < slotRoot.childCount; i++)
            if (slotRoot.GetChild(i) == slot) return i;
        return -1;
    }

    // test
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var slotDetails = CheckFormationStatus();
            foreach (var detail in slotDetails)
                Debug.Log(detail);
        }
    }

    public List<string> CheckFormationStatus()
    {
        List<string> slotDetails = new List<string>();
        var allHeroes = PlayerInventory.Instance.GetHeroViewList(DatabaseManager.Instance.HeroDatabase);

        for (int i = 1; i <= MAX_SLOT; i++)
        {
            if (formationHeroIds[i] == -1)
            {
                slotDetails.Add($"Slot {i}: Empty");
            }
            else
            {
                int heroId = formationHeroIds[i];
                HeroViewData heroData = allHeroes.Find(h => h.instance.heroId == heroId);
                if (heroData != null) slotDetails.Add($"Slot {i}: {heroData.info.name}");
                else slotDetails.Add($"Slot {i}: Unknown Hero");
            }
        }

        return slotDetails;
    }
}