using System.Collections.Generic;
using UnityEngine;

public class UI_ListHeroBattle : MonoBehaviour
{
    public HeroDatabase heroDatabase;
    public Transform content;
    public GameObject heroBattlePrefab;

    void OnEnable()
    {
        LoadHeroes();
    }

    void LoadHeroes()
    {
        Clear();

        // Build dictionary heroId -> HeroViewData để lookup nhanh
        var heroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);
        var heroById = new Dictionary<int, HeroViewData>();
        foreach (var h in heroes)
        {
            if (h?.instance == null) continue;
            heroById[h.instance.heroId] = h;
        }

        // Load formation, dùng index 1..6 (bỏ 0)
        int[] ids = FormationManager.Load();

        // defensive: nếu mảng ngắn hơn 7 thì cũng không crash
        int maxIndex = Mathf.Min(6, ids.Length - 1);
        for (int slotIndex = 1; slotIndex <= maxIndex; slotIndex++)
        {
            int heroId = ids[slotIndex];
            if (heroId == -1) continue;

            if (!heroById.TryGetValue(heroId, out var heroData) || heroData == null)
            {
                Debug.LogWarning($"[UI_ListHeroBattle] Formation slot {slotIndex} heroId={heroId} not found in inventory.");
                continue;
            }

            // Tạo UI item
            var go = Instantiate(heroBattlePrefab, content);
            go.name = $"HeroBattle_Slot{slotIndex}_Hero{heroId}";

            go.GetComponent<UI_HeroBattle>()
              .Setup(heroData, OnHeroSelected);

            // Slot 1..6 phải hiển thị từ phải -> trái:
            // thêm xong đẩy về đầu để đảo trật tự hiển thị
            go.transform.SetAsFirstSibling();
        }
    }

    void OnHeroSelected(HeroViewData hero)
    {
        // TODO: xử lý chọn hero nếu cần
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}