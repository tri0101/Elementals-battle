using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Refs")]
    public HeroDatabase heroDatabase;

    [Header("Formation (scene setup)")]
    public BattleFormation formation;

    [Header("Move")]
    [Min(0.01f)]
    private float moveSpeed = 35f;

    private readonly List<GameObject> spawnedHeroes = new List<GameObject>();

    void Start()
    {
        LoadFormationToBattleScene();
    }

    public void LoadFormationToBattleScene()
    {
        ClearSpawned();

        if (formation == null)
        {
            Debug.LogError("[BattleManager] formation is NULL");
            return;
        }

        if (formation.listHeroRoot == null)
        {
            Debug.LogError("[BattleManager] formation.listHeroRoot is NULL (ListHero)");
            return;
        }

        if (heroDatabase == null)
        {
            Debug.LogError("[BattleManager] heroDatabase is NULL");
            return;
        }

        var heroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);
        var heroById = new Dictionary<int, HeroViewData>();
        foreach (var h in heroes)
        {
            if (h?.instance == null) continue;
            heroById[h.instance.heroId] = h;
        }

        int[] ids = FormationManager.Load();
        if (ids == null || ids.Length == 0)
        {
            Debug.LogWarning("[BattleManager] Formation empty.");
            return;
        }

        // dùng slot 1..6, bỏ slot 0
        int maxSlot = Mathf.Min(6, ids.Length - 1);
        for (int slotIndex = 1; slotIndex <= maxSlot; slotIndex++)
        {
            int heroId = ids[slotIndex];
            if (heroId == -1) continue;

            if (!heroById.TryGetValue(heroId, out var heroData) || heroData == null)
            {
                Debug.LogWarning($"[BattleManager] Slot {slotIndex} heroId={heroId} not found in inventory.");
                continue;
            }

            if (heroData.info == null || heroData.info.HeroPrefab == null)
            {
                Debug.LogWarning($"[BattleManager] Slot {slotIndex} heroId={heroId} missing HeroPrefab in HeroInfo.");
                continue;
            }

            Transform startT = formation.GetStart(slotIndex);
            Transform battleT = formation.GetBattle(slotIndex);

            if (startT == null || battleT == null)
            {
                Debug.LogError($"[BattleManager] Missing start/battle transform for slot {slotIndex}.");
                continue;
            }

            GameObject heroGo = Instantiate(heroData.info.HeroPrefab, formation.listHeroRoot);
            heroGo.name = $"Hero_{heroId}_Slot{slotIndex}";
            heroGo.transform.position = startT.position;
            var heroControl = heroGo.GetComponent<HeroControl>();
            heroControl.SetBattleTarget(battleT.position);
            spawnedHeroes.Add(heroGo);

            //StartCoroutine(MoveToBattlePos(heroGo.transform, battleT.position));
        }
    }

    private IEnumerator MoveToBattlePos(Transform hero, Vector3 targetPos)
    {
        if (hero == null) yield break;

        // chạy tới khi đủ gần
        while (hero != null && (hero.position - targetPos).sqrMagnitude > 0.01f)
        {
            hero.position = Vector3.MoveTowards(hero.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (hero != null)
            hero.position = targetPos;

        // tạm thời: đứng yên tại battle pos
        // nếu muốn: gọi animation idle trong HeroControl ở đây
        // var hc = hero.GetComponent<HeroControl>();
        // if (hc != null) hc.ChangeAnimationState("Idle");
    }

    private void ClearSpawned()
    {
        for (int i = 0; i < spawnedHeroes.Count; i++)
            if (spawnedHeroes[i] != null)
                Destroy(spawnedHeroes[i]);

        spawnedHeroes.Clear();
    }
}