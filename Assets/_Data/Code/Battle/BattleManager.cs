using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework.Interfaces;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Stage")]
    [SerializeField] private StageConfig stageConfig;
    public StageConfig StageConfig => stageConfig;

    [SerializeField] private SpriteRenderer backGround;
    public SpriteRenderer BackGround => backGround;

    [Header("Formation (scene setup)")]
    [SerializeField] private BattleFormation formation;
    public BattleFormation Formation => formation;

    [Header("Transform")]
    [SerializeField] private Transform backBottom;
    [SerializeField] private Transform backHeroExp;
    Dictionary<int, UI_HeroBattle> dictHeroBattle = new Dictionary<int, UI_HeroBattle>();
    Dictionary<int, bool> listHeroStatus = new Dictionary<int, bool>(); // true = live , false = dead
    [SerializeField] private UI_StageReward uiStageReward;
    [SerializeField] private BattleResult battleResult;
    public BattleResult BattleResult => battleResult;

    [Header("Growth (for HeroStatCalculator)")]
    [SerializeField] private HeroGrowthConfig growthConfig;

    private readonly List<GameObject> spawnedHeroes = new List<GameObject>();
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    // NEW: instance enemy theo heroId (tại wave hiện tại)
    private readonly Dictionary<int, HeroInstance> listEnemyInstance = new Dictionary<int, HeroInstance>();

    public bool IsWaveReady { get; private set; }

    [Header("Wave Ready Gate")]
    [Tooltip("Seconds to wait after spawning all units before setting IsWaveReady = true.")]
    [Min(0f)]
    private float readyDelaySeconds = 0.01f;

    private Coroutine readyRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        stageConfig = StageContext.selectedStage;

        if (battleResult == null)
            battleResult = GetComponent<BattleResult>();

        backGround.sprite = stageConfig.background;
    }

    void Start()
    {
        AddHeroBattle();
        listHeroStatus.Clear();
        LoadWave(1, keepHeroes: false);
    }

    void AddHeroBattle()
    {
        dictHeroBattle.Clear();

        foreach (Transform child in backBottom)
        {
            var ui = child.GetComponent<UI_HeroBattle>();
            if (ui != null)
                dictHeroBattle.Add(int.Parse(child.name), ui);
        }
    }

    // NEW: lấy level enemy theo heroId ở stage/wave hiện tại
    public HeroInstance GetEnemyInstance(int enemyHeroId)
    {
        if (enemyHeroId <= 0) return null;

        return listEnemyInstance.TryGetValue(enemyHeroId, out var inst)
            ? inst
            : null;
    }

    public void LoadWave(int wave, bool keepHeroes)
    {
        waveText.text = $"Wave {wave}/{stageConfig.waveStage}";
        IsWaveReady = false;

        if (readyRoutine != null)
        {
            StopCoroutine(readyRoutine);
            readyRoutine = null;
        }

        if (!keepHeroes)
        {
            ClearSpawnedHeroes();

            if (BattlefieldRegistry.Instance != null)
                BattlefieldRegistry.Instance.Clear();
        }
        else
        {
            ClearSpawnedEnemies();

            if (BattlefieldRegistry.Instance != null)
            {
                BattlefieldRegistry.Instance.Clear();
                ReRegisterExistingHeroes();
            }

            ResetHeroesToStartAndRunIn();
        }

        SpawnEnemiesForWave(wave);

        if (formation == null)
        {
            Debug.LogError("[BattleManager] formation is NULL");
            return;
        }

        if (formation.ListHeroRoot == null)
        {
            Debug.LogError("[BattleManager] formation.listHeroRoot is NULL (ListHero)");
            return;
        }

        if (formation.ListEnemyRoot == null)
        {
            Debug.LogError("[BattleManager] formation.listEnemyRoot is NULL (ListEnemy)");
            return;
        }

        if (growthConfig == null)
            Debug.LogWarning("[BattleManager] growthConfig is NULL. Units will fallback to HeroInfo base stats (no scaling).");

        DatabaseManager.Instance.HeroDatabase.Init();

        if (!keepHeroes)
        {
            SpawnPlayerHeroes();
            battleResult.HeroTotal = formation.ListHeroRoot.childCount;
        }

        readyRoutine = StartCoroutine(CoSetWaveReadyAfterDelay());
    }

    public void LoadWave(int wave)
    {
        LoadWave(wave, keepHeroes: wave > 1);
    }

    private void ResetHeroesToStartAndRunIn()
    {
        if (formation == null)
            return;

        for (int i = 0; i < spawnedHeroes.Count; i++)
        {
            var go = spawnedHeroes[i];
            if (go == null) continue;

            if (!int.TryParse(go.name, out int slotIndex))
                continue;

            Transform startT = formation.GetStart(slotIndex);
            Transform battleT = formation.GetBattle(slotIndex);
            if (startT == null || battleT == null) continue;

            go.transform.position = startT.position;

            var heroControl = go.GetComponent<HeroControl>();
            if (heroControl != null)
            {
                heroControl.SetBattleTarget(battleT.position);
                heroControl.GoBackBattleTarget();
            }
        }
    }

    private void ReRegisterExistingHeroes()
    {
        if (BattlefieldRegistry.Instance == null)
            return;

        for (int i = 0; i < spawnedHeroes.Count; i++)
        {
            var go = spawnedHeroes[i];
            if (go == null) continue;

            if (!int.TryParse(go.name, out int slotIndex))
                continue;

            BattlefieldRegistry.Instance.Register(go.transform, slotIndex, "Hero");
        }
    }

    private IEnumerator CoSetWaveReadyAfterDelay()
    {
        if (readyDelaySeconds > 0f)
            yield return new WaitForSeconds(readyDelaySeconds);

        IsWaveReady = true;
        readyRoutine = null;
    }

    private void SpawnPlayerHeroes()
    {
        var heroes = PlayerInventory.Instance.GetHeroViewList(DatabaseManager.Instance.HeroDatabase);
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
                Debug.LogError($"[BattleManager] Missing hero start/battle transform for slot {slotIndex}.");
                continue;
            }

            GameObject heroGo = Instantiate(heroData.info.HeroPrefab, formation.ListHeroRoot);
            heroGo.name = $"{slotIndex}";
            heroGo.transform.position = startT.position;
            heroGo.tag = "Hero";

            if (BattlefieldRegistry.Instance != null)
                BattlefieldRegistry.Instance.Register(heroGo.transform, slotIndex, "Hero");

            var statRuntime = heroGo.GetComponent<HeroStatRuntime>();
            if (statRuntime != null)
                statRuntime.Init(heroData.info, heroData.instance, growthConfig);
            else
                Debug.LogWarning($"[BattleManager] {heroGo.name} missing HeroStatRuntime. Add it to the hero prefab.");

            var heroControl = heroGo.GetComponent<HeroControl>();
            if (heroControl != null && heroControl.HeroStatRuntime != null)
            {
                heroControl.HeroStatRuntime.GainStatByEmpower();
            }

            foreach (var soul in heroData.info.soulID)
            {
                FightSoulInfo soulInfo = DatabaseManager.Instance.FightSoulDatabase.GetSoulInfo(soul);
                if (soulInfo != null && statRuntime != null)
                {
                    statRuntime.GainValueBySoul(heroData.instance, soulInfo);
                }
            }

            if (heroControl != null && dictHeroBattle.ContainsKey(slotIndex))
                dictHeroBattle[slotIndex].BindHeroControl(heroControl);

            if (heroControl != null)
            {
                heroControl.SetBattleTarget(battleT.position);
                heroControl.GoBackBattleTarget();
            }

            spawnedHeroes.Add(heroGo);
        }
    }

    private void SpawnEnemiesForWave(int wave)
    {
        if (stageConfig == null)
        {
            Debug.LogWarning("[BattleManager] stageConfig is NULL. Skip spawning enemies.");
            return;
        }

        if (stageConfig.enemySpawns == null || stageConfig.enemySpawns.Count == 0)
        {
            Debug.LogWarning("[BattleManager] stageConfig.enemySpawns empty. Skip spawning enemies.");
            return;
        }

        // reset cache mỗi wave
        listEnemyInstance.Clear();

        // NEW: build lookup heroId -> EnemySpawnData (treat as base config per heroId)
        Dictionary<int, EnemySpawnData> enemyDataByHeroId = null;
        if (stageConfig.enemies != null && stageConfig.enemies.Count > 0)
        {
            enemyDataByHeroId = new Dictionary<int, EnemySpawnData>(stageConfig.enemies.Count);
            for (int i = 0; i < stageConfig.enemies.Count; i++)
            {
                var e = stageConfig.enemies[i];
                if (e == null) continue;

                // If duplicated heroId exists, keep the last one (deterministic)
                enemyDataByHeroId[e.heroId] = e;
            }
        }

        // Collect entries for this wave and spawn deterministically by slot
        var waveEntries = new List<EnemySpawnEntry>(6);
        for (int i = 0; i < stageConfig.enemySpawns.Count; i++)
        {
            var entry = stageConfig.enemySpawns[i];
            if (entry == null) continue;
            if (entry.wave != wave) continue;
            waveEntries.Add(entry);
        }

        waveEntries.Sort((a, b) => a.slotIndex.CompareTo(b.slotIndex));

        for (int i = 0; i < waveEntries.Count; i++)
        {
            var entry = waveEntries[i];

            int slotIndex = entry.slotIndex;
            int enemyHeroId = entry.heroId;

            Transform startT = formation.GetEnemyStart(slotIndex);
            Transform battleT = formation.GetEnemyBattle(slotIndex);

            if (startT == null || battleT == null)
            {
                Debug.LogError($"[BattleManager] Missing enemy start/battle transform for slot {slotIndex}.");
                continue;
            }

            var enemyInfo = DatabaseManager.Instance.HeroDatabase.GetHero(enemyHeroId);
            if (enemyInfo == null || enemyInfo.HeroPrefab == null)
            {
                Debug.LogWarning($"[BattleManager] Enemy heroId={enemyHeroId} missing in HeroDatabase or missing HeroPrefab.");
                continue;
            }

            GameObject enemyGo = Instantiate(enemyInfo.HeroPrefab, formation.ListEnemyRoot);
            enemyGo.name = $"Enemy_{enemyHeroId}_Slot{slotIndex}";
            enemyGo.transform.position = startT.position;
            enemyGo.tag = "Enemy";

            if (BattlefieldRegistry.Instance != null)
                BattlefieldRegistry.Instance.Register(enemyGo.transform, slotIndex, "Enemy");

            // flip to face heroes
            {
                Vector3 s = enemyGo.transform.localScale;
                enemyGo.transform.localScale = new Vector3(-Mathf.Abs(s.x), s.y, s.z);
            }

            // NEW: always resolve EnemySpawnData by heroId (base config)
            EnemySpawnData enemyData = null;
            if (enemyDataByHeroId != null)
                enemyDataByHeroId.TryGetValue(enemyHeroId, out enemyData);

            // NEW: always create enemyInstance so stats scale (power always available)
            int level = (enemyData != null && enemyData.level > 0) ? enemyData.level : 1;
            int star = (enemyData != null && enemyData.star > 0) ? enemyData.star : 1;
            int rank = (enemyData != null && enemyData.rank > 0) ? enemyData.rank : 1;

            HeroInstance enemyInstance = new HeroInstance
            {
                heroId = enemyHeroId,
                level = level,
                currentExp = 0,
                star = star,
                rank = rank,
                shard = 0
            };

            listEnemyInstance[enemyHeroId] = enemyInstance;

            var statRuntime = enemyGo.GetComponent<HeroStatRuntime>();
            if (statRuntime != null)
                statRuntime.Init(enemyInfo, enemyInstance, growthConfig);

            var enemyControl = enemyGo.GetComponent<HeroControl>();
            if (enemyControl != null && enemyControl.HeroInfo != null && enemyControl.HeroInfo.ID == 507)
            {
                statRuntime.CurrentMana = 1000f;
            }

            if (enemyControl != null)
            {
                enemyControl.SetBattleTarget(battleT.position);
                enemyControl.GoBackBattleTarget();
            }

            spawnedEnemies.Add(enemyGo);
        }
    }

    private void ClearSpawnedEnemies()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
            if (spawnedEnemies[i] != null)
                Destroy(spawnedEnemies[i]);

        spawnedEnemies.Clear();
    }

    private void ClearSpawnedHeroes()
    {
        ClearSpawnedEnemies();

        for (int i = 0; i < spawnedHeroes.Count; i++)
            if (spawnedHeroes[i] != null)
                Destroy(spawnedHeroes[i]);

        spawnedHeroes.Clear();
    }

    public void SetActiveForUIBatle(bool value)
    {
        foreach (Transform child in backBottom)
        {
            child.gameObject.SetActive(value);
        }
    }

    private void ClearSpawned()
    {
        ClearSpawnedHeroes();
    }

    // ========== sàn đấu (Arena)
    [System.Serializable]
    private struct Arena
    {
        public string skillApply;
        public string nameHero; // tên hero
        public int heroId; // NEW: id của hero
        public int order; // dùng để biết độ ưu tiên của sàn
        public Sprite arenaSprite;

        public Arena(string skillApply, string nameHero, int heroId, int order, Sprite sprite = null)
        {
            this.skillApply = skillApply;
            this.nameHero = nameHero;
            this.heroId = heroId;
            this.order = order;
            this.arenaSprite = sprite;
        }
    }

    private Stack<Arena> stack = new Stack<Arena>();
    public bool HasArenaWithSkill(string skillName)
    {
        if (string.IsNullOrEmpty(skillName) || stack.Count == 0)
            return false;

        var stackList = new List<Arena>(stack);

        for (int i = 0; i < stackList.Count; i++)
        {
            if (stackList[i].skillApply == skillName)
            {
                Debug.Log($"[Arena] Found existing arena with skill: {skillName}");
                return true;
            }
        }

        Debug.Log($"[Arena] No arena found with skill: {skillName}");
        return false;
    }

    public void PutArenaOnStack(string skillName, string nameHero, int heroId, int order, Sprite arenaSprite = null)
    {
        if (string.IsNullOrEmpty(skillName) || string.IsNullOrEmpty(nameHero) || heroId <= 0) return;

        Arena newArena = new Arena(skillName, nameHero, heroId, order, arenaSprite);

        if (stack.Count == 0)
        {
            stack.Push(newArena);
            UpdateBackgroundSprite();
            SyncArenaDebug();
            Debug.Log($"[Arena] Thêm arena mới: {skillName} của hero {nameHero} (id={heroId}, order={order})");
            return;
        }

        Arena currentArena = stack.Peek();

        if (currentArena.order > newArena.order)
        {
            stack.Pop();
            stack.Push(newArena);
            stack.Push(currentArena);

            UpdateBackgroundSprite();
            SyncArenaDebug();
            Debug.Log($"[Arena] Arena mới '{skillName}' của {nameHero} được đặt TRƯỚC arena hiện tại '{currentArena.skillApply}' (order: {newArena.order} < {currentArena.order})");
        }
        else
        {
            var temp = new List<Arena>();
            while (stack.Count > 0)
                temp.Add(stack.Pop());

            stack.Push(newArena);

            for (int i = temp.Count - 1; i >= 0; i--)
                stack.Push(temp[i]);

            Debug.Log($"[Arena] Arena mới '{skillName}' của {nameHero} được đặt SAU arena hiện tại '{currentArena.skillApply}' (order: {newArena.order} >= {currentArena.order})");
        }
    }

    public void RemoveArenaByHeroName(string nameHero)
    {
        if (string.IsNullOrEmpty(nameHero) || stack.Count == 0) return;

        bool isCurrentArenaRemoved = false;
        Arena currentArena = stack.Peek();

        if (currentArena.nameHero == nameHero)
        {
            isCurrentArenaRemoved = true;
            stack.Pop();
            Debug.Log($"[Arena] Xóa arena đang apply '{currentArena.skillApply}' của hero {nameHero}");
        }

        var tempList = new List<Arena>();
        while (stack.Count > 0)
        {
            var arena = stack.Pop();
            if (arena.nameHero != nameHero)
            {
                tempList.Add(arena);
            }
            else
            {
                Debug.Log($"[Arena] Xóa arena '{arena.skillApply}' của hero {nameHero} từ stack");
            }
        }

        for (int i = tempList.Count - 1; i >= 0; i--)
            stack.Push(tempList[i]);

        if (isCurrentArenaRemoved)
        {
            UpdateBackgroundSprite();
        }

        SyncArenaDebug();
        Debug.Log($"[Arena] Sau khi xóa arena của {nameHero}, arena hiện tại: {(stack.Count > 0 ? stack.Peek().skillApply : "Không có")}");
    }

    public void RemoveArenaByHeroId(int heroId)
    {
        if (heroId <= 0 || stack.Count == 0) return;

        bool isCurrentArenaRemoved = false;
        Arena currentArena = stack.Peek();

        if (currentArena.heroId == heroId)
        {
            isCurrentArenaRemoved = true;
            stack.Pop();
            Debug.Log($"[Arena] Xóa arena đang apply '{currentArena.skillApply}' của hero {heroId}");
        }

        var tempList = new List<Arena>();
        while (stack.Count > 0)
        {
            var arena = stack.Pop();
            if (arena.heroId != heroId)
            {
                tempList.Add(arena);
            }
            else
            {
                Debug.Log($"[Arena] Xóa arena '{arena.skillApply}' của hero {heroId} từ stack");
            }
        }

        for (int i = tempList.Count - 1; i >= 0; i--)
            stack.Push(tempList[i]);

        if (isCurrentArenaRemoved)
        {
            UpdateBackgroundSprite();
        }

        SyncArenaDebug();
        Debug.Log($"[Arena] Sau khi xóa arena của hero {heroId}, arena hiện tại: {(stack.Count > 0 ? stack.Peek().skillApply : "Không có")}");
    }

    private void UpdateBackgroundSprite()
    {
        if (backGround == null) return;

        if (stack.Count > 0)
        {
            Arena currentArena = stack.Peek();
            if (currentArena.arenaSprite != null)
            {
                backGround.sprite = currentArena.arenaSprite;
                Debug.Log($"[Arena] Background thay đổi thành: {currentArena.arenaSprite.name}");
            }
            else
            {
                backGround.sprite = stageConfig.background;
                Debug.Log("[Arena] Arena không có sprite, dùng stage background");
            }
        }
        else
        {
            backGround.sprite = stageConfig.background;
            Debug.Log("[Arena] Stack rỗng, quay về stage background");
        }
    }

    [System.Serializable]
    private struct ArenaDebugItem
    {
        public string skillApply;
        public string nameHero; // NEW
        public int heroId; // NEW
        public int order;
        public string spriteeName;
    }

    [Header("Debug Arena (Inspector)")]
    [SerializeField] private List<ArenaDebugItem> arenaDebug = new List<ArenaDebugItem>();

    private void SyncArenaDebug()
    {
        arenaDebug.Clear();

        if (stack.Count == 0)
            return;

        var stackList = new List<Arena>(stack);

        for (int i = 0; i < stackList.Count; i++)
        {
            var arena = stackList[i];
            arenaDebug.Add(new ArenaDebugItem
            {
                skillApply = arena.skillApply,
                nameHero = arena.nameHero,
                heroId = arena.heroId,
                order = arena.order,
                spriteeName = arena.arenaSprite != null ? arena.arenaSprite.name : "None"
            });
        }
    }
}