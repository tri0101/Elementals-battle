using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FormationManager : MonoBehaviour
{
    [Header("UI")]
    public Transform slotRoot;
    public GameObject heroItemPrefab;

    [Header("Refs")]
    public HeroDatabase heroDatabase;
    public UI_PanelChooseHero panelChooseHero;

    private const int MAX_SLOT = 6;
    private int[] formationHeroIds;

    //Khóa 
    private bool isBusy = false;

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

    IEnumerator RebuildNextFrame()
    {
        yield return null;
        RebuildFormationUI();
    }

    public bool IsHeroInFormation(int heroId)
    {
        for (int i = 0; i < MAX_SLOT; i++)
            if (formationHeroIds[i] == heroId) return true;
        return false;
    }

    public bool TryAddHero(UI_HeroChooseItem heroItem)
    {
        if (isBusy) return false;
        if (heroItem == null || heroItem.IsInFormation) return false;

        isBusy = true;

        int heroId = heroItem.Data.instance.heroId;

        for (int i = 0; i < MAX_SLOT; i++)
        {
            if (formationHeroIds[i] == -1)
            {
                Transform slot = slotRoot.Find($"Slot{i + 1}");
                if (slot == null)
                {
                    Debug.LogError($"Slot{i + 1} NULL");
                    isBusy = false;
                    return false;
                }

                formationHeroIds[i] = heroId;
                PlaceHero(heroItem, slot);
                FormationManager.Save(formationHeroIds);
                panelChooseHero.RefreshPower();

                isBusy = false;
                return true;
            }
        }

        isBusy = false;
        return false;
    }

    public void RemoveHero(UI_HeroChooseItem heroItem, Transform inventory)
    {
        if (heroItem == null) return;
        int heroId = heroItem.Data.instance.heroId;

        for (int i = 0; i < MAX_SLOT; i++)
            if (formationHeroIds[i] == heroId) formationHeroIds[i] = -1;

        heroItem.transform.SetParent(inventory, false);
        ResetRect(heroItem.GetComponent<RectTransform>());
        heroItem.SetInFormation(false);
        FormationManager.Save(formationHeroIds);
        panelChooseHero.RefreshPower();
        panelChooseHero.LoadHeroes();
    }

    void RebuildFormationUI()
    {
        if (slotRoot == null || heroItemPrefab == null) return;

        foreach (Transform slot in slotRoot)
            foreach (Transform child in slot)
            {
                if (child.name.StartsWith("Hero"))
                {
                    Destroy(child.gameObject);
                }
               
            }

        var allHeroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);

        for (int i = 0; i < MAX_SLOT; i++)
        {
            int savedId = formationHeroIds[i];
            if (savedId == -1) continue;

            HeroViewData heroData = allHeroes.Find(h => h.instance.heroId == savedId);
            if (heroData == null) continue;

            Transform slot = slotRoot.Find($"Slot{i + 1}");
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
    public void SwapHeroes(UI_HeroChooseItem heroA, UI_HeroChooseItem heroB)
    {
        if (heroA == null || heroB == null) return;

        int indexA = GetHeroIndexInFormation(heroA.Data.instance.heroId);
        int indexB = GetHeroIndexInFormation(heroB.Data.instance.heroId);

        if (indexA == -1 || indexB == -1) return;

       
        int temp = formationHeroIds[indexA];
        formationHeroIds[indexA] = formationHeroIds[indexB];
        formationHeroIds[indexB] = temp;

     
        Transform slotA = slotRoot.Find($"Slot{indexA + 1}");
        Transform slotB = slotRoot.Find($"Slot{indexB + 1}");

        heroA.transform.SetParent(slotB, false);
        heroB.transform.SetParent(slotA, false);

        PlaceHero(heroA, slotB);
        PlaceHero(heroB, slotA);


        FormationManager.Save(formationHeroIds);
        panelChooseHero.RefreshPower();
    }

    private int GetHeroIndexInFormation(int heroId)
    {
        for (int i = 0; i < formationHeroIds.Length; i++)
        {
            if (formationHeroIds[i] == heroId) return i;
        }
        return -1;
    }
    private int GetSlotIndex(Transform slot)
    {
        for (int i = 0; i < slotRoot.childCount; i++)
        {
            if (slotRoot.GetChild(i) == slot)
                return i;
        }
        return -1;
    }
    public void MoveHeroToSlot(UI_HeroChooseItem heroItem, Transform targetSlot)
    {
        if (heroItem == null || targetSlot == null) return;

        int heroId = heroItem.Data.instance.heroId;

        
        int currentIndex = GetHeroIndexInFormation(heroId);
        int targetIndex = GetSlotIndex(targetSlot);

        if (currentIndex == -1 || targetIndex == -1) return;

        
        formationHeroIds[currentIndex] = -1; 
        formationHeroIds[targetIndex] = heroId; 

        
        heroItem.transform.SetParent(targetSlot, false);
        PlaceHero(heroItem, targetSlot);

        FormationManager.Save(formationHeroIds);
        panelChooseHero.RefreshPower();
    }




    //test tạm
    void Update()
    {
        // Kiểm tra nếu phím Space được nhấn
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Gọi hàm CheckFormationStatus
            var slotDetails = CheckFormationStatus();

            // Debug thông tin
            foreach (var detail in slotDetails)
            {
                Debug.Log(detail);
            }
        }
    }
    public List<string> CheckFormationStatus()
    {
        List<string> slotDetails = new List<string>();

        // Lấy danh sách tất cả hero từ HeroDatabase
        var allHeroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);

        for (int i = 0; i < MAX_SLOT; i++)
        {
            if (formationHeroIds[i] == -1)
            {
                // Slot trống
                slotDetails.Add($"Slot {i + 1}: Empty");
            }
            else
            {
                // Tìm hero trong slot
                int heroId = formationHeroIds[i];
                HeroViewData heroData = allHeroes.Find(h => h.instance.heroId == heroId);
                if (heroData != null)
                {
                    slotDetails.Add($"Slot {i + 1}: {heroData.info.name}");
                }
                else
                {
                    slotDetails.Add($"Slot {i + 1}: Unknown Hero");
                }
            }
        }

        return slotDetails;
    }

      
}