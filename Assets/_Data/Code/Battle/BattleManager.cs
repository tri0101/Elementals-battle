using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;


    [Header("Stage")]
    [SerializeField] private StageConfig stageConfig;
    public StageConfig StageConfig => stageConfig;

    [Header("Formation (scene setup)")]
    [SerializeField] private BattleFormation formation;

    [Header("Transform")]
    [SerializeField] private Transform backBottom;
    [SerializeField] private Transform backHeroExp;
    Dictionary<int, UI_HeroBattle> dictHeroBattle = new Dictionary<int, UI_HeroBattle>();
    Dictionary<int, bool> listHeroStatus = new Dictionary<int, bool>();// true = live , false = dead
    [SerializeField] private UI_StageReward uiStageReward;
    [SerializeField] private BattleResult battleResult;
    public BattleResult BattleResult => battleResult;
    [Header("Growth (for HeroStatCalculator)")]
    [SerializeField] private HeroGrowthConfig growthConfig;

    private readonly List<GameObject> spawnedHeroes = new List<GameObject>();
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    public bool IsWaveReady { get; private set; }

    [Header("Wave Ready Gate")]
    [Tooltip("Seconds to wait after spawning all units before setting IsWaveReady = true.")]
    [Min(0f)]
    private float readyDelaySeconds = 0.5f;

    private Coroutine readyRoutine;

    void Awake()
    {
        stageConfig = StageContext.selectedStage;
        if (battleResult == null)
            battleResult = GetComponent<BattleResult>();
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
           

        SpawnEnemiesForWave(wave);

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

            // Init base stat (HP/Mana are managed by HeroReceiveDamagee/HeroStatRuntime during gameplay)
            var statRuntime = heroGo.GetComponent<HeroStatRuntime>();
            if (statRuntime != null)
                statRuntime.Init(heroData.info, heroData.instance, growthConfig);
            else
                Debug.LogWarning($"[BattleManager] {heroGo.name} missing HeroStatRuntime. Add it to the hero prefab.");

            var heroControl = heroGo.GetComponent<HeroControl>();
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

        Dictionary<int, EnemySpawnData> enemyDataByHeroId = null;
        if (stageConfig.enemies != null && stageConfig.enemies.Count > 0)
        {
            enemyDataByHeroId = new Dictionary<int, EnemySpawnData>(stageConfig.enemies.Count);
            for (int i = 0; i < stageConfig.enemies.Count; i++)
            {
                var e = stageConfig.enemies[i];
                if (e == null) continue;
                enemyDataByHeroId[e.heroId] = e;
            }
        }

        for (int i = 0; i < stageConfig.enemySpawns.Count; i++)
        {
            var entry = stageConfig.enemySpawns[i];
            if (entry == null) continue;
            if (entry.wave != wave) continue;

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

            {
                Vector3 s = enemyGo.transform.localScale;
                enemyGo.transform.localScale = new Vector3(-Mathf.Abs(s.x), s.y, s.z);
            }

            HeroInstance enemyInstance = null;
            if (enemyDataByHeroId != null && enemyDataByHeroId.TryGetValue(enemyHeroId, out var enemyData) && enemyData != null)
            {
                enemyInstance = new HeroInstance
                {
                    heroId = enemyHeroId,
                    level = enemyData.level,
                    currentExp = 0,
                    star = enemyData.star,
                    rank = enemyData.rank,
                    shard = 0
                };
            }

            var statRuntime = enemyGo.GetComponent<HeroStatRuntime>();
            if (statRuntime != null)
                statRuntime.Init(enemyInfo, enemyInstance, growthConfig);

            var enemyControl = enemyGo.GetComponent<HeroControl>();
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
        foreach(Transform child in backBottom)
        {
           child.gameObject.SetActive(value);
        }
    }
    private void ClearSpawned()
    {
        ClearSpawnedHeroes();
    }
}