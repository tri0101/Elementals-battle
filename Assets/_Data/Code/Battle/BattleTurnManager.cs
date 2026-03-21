using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BattleTurnManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject clearImage;
    [SerializeField] private GameObject winExpPlusImage;
    [Header("Refs")]
    [SerializeField] private BattleManager battleManager;


    [Header("Turn Config")]
    [Min(1)] private int maxTurns = 20;

    [Tooltip("Delay between each hero action (seconds).")]
    [Min(0f)] private float delayBetweenActions = 0.15f;

    [Header("Ultimate Phase")]
    [Tooltip("Delay between each ultimate cast and the next unit (seconds).")]
    [Min(0f)] private float delayBetweenUltimates = 0.05f;

    [Header("Wave / Clear")]
    [Tooltip("Seconds to wait after enemies die (before checking heroes finished clear).")]
    [Min(0f)] private float afterClearDelay = 0.25f;

    private const string TeamHero = "Hero";
    private const string TeamEnemy = "Enemy";

    private bool heroTeamStarts;
    private bool isEnd;

    private int currentWave = 1;

    private void Awake()
    {
        if (battleManager == null)
            battleManager = GetComponent<BattleManager>();
       
    }

    private void Start()
    {
        isEnd = false;
        battleManager.SetActiveForUIBatle(false);
        StartCoroutine(CoBattleLoop());
    }

  
    private IEnumerator CoBattleLoop()
    {
        // initial wait for wave spawn ready
        while (battleManager == null || !battleManager.IsWaveReady)
            yield return null;
       
        currentWave = 1;
        yield return CoPassiveBattlePhase();
        while (!isEnd)
        {
            heroTeamStarts = DecideHeroTeamStarts();

            // ===== TURN LOOP FOR CURRENT WAVE =====
            for (int turn = 1; turn <= maxTurns; turn++)
            {
                if (turnText != null)
                    turnText.text = $"{turn}/20";

                SetCanSkill();
 
                battleManager.SetActiveForUIBatle(true);
                if(currentWave == 1 && turn == 1) yield return new WaitForSeconds(2f);
                else if(turn == 1) yield return new WaitForSeconds(2f);
                else yield return new WaitForSeconds(1f);
                // Wave clear check at turn start
                if (AreAllTeamDead(TeamEnemy))
                {
                    yield return CoHandleWaveCleared();
                    break; // break turn loop, next wave (or end)
                }

                yield return CoUltimatePhase();

                if (AreAllTeamDead(TeamEnemy))
                {
                    yield return CoHandleWaveCleared();
                    break;
                }

                yield return CoNormalSkillPhase();

                if (AreAllTeamDead(TeamEnemy))
                {
                    battleManager.SetActiveForUIBatle(false);
                    yield return CoHandleWaveCleared();
                    break;
                }
             
            }

            // If we handled clear and loaded new wave, wait until ready then continue.
            // If no more waves, CoHandleWaveCleared() will break the outer loop.
            if (battleManager == null)
                yield break;

            // If battleManager just loaded a new wave, wait for ready
            while (!battleManager.IsWaveReady)
                yield return null;

            // If we reached maxTurns without clearing enemies, you can decide what to do:
            // - fail, or
            // - continue turns.
            // Current behavior: continue waves loop; if enemies still alive, it will just run again.
        }
    }

    private IEnumerator CoHandleWaveCleared()
    {
        // 1) Call SetClear() for all alive heroes
        if (currentWave >= battleManager.StageConfig.waveStage)
        {
            isEnd = true;
            Time.timeScale = 1f;
            for (int slot = 1; slot <= 6; slot++)
            {
                var hero = GetUnitAtSlot(TeamHero, slot);
                if (hero == null) continue;
                if (IsDead(hero))
                {
                    
                    battleManager.BattleResult.SetList(slot, false);
                    battleManager.BattleResult.SetListByID(hero.HeroInfo.ID, false);
                }
                else
                {
                    battleManager.BattleResult.SetListByID(hero.HeroInfo.ID, true);
                    battleManager.BattleResult.SetList(slot, true);
                }
            }
            ProgressManager.Instance.UpdateStage(battleManager.StageConfig.stageID); // cập nhật stage
            
            battleManager.BattleResult.SetUIExpPlus(); // gán exp plus UI
            battleManager.BattleResult.SetExpPlus(); // gán exp plus
            battleManager.BattleResult.SetExpForPlayer(); // gán exp cho player
            winExpPlusImage.SetActive(true); // hiện thị exp plus
            battleManager.BattleResult.CheckHeroesLost(); // tính số sao nhận được 
            battleManager.BattleResult.SetUpRollItems(); // tính toán rớt đồ
            //battleManager.battleResult.uiStageReward.gameObject.SetActive(true);
            yield break;
        }
        for (int slot = 1; slot <= 6; slot++)
        {
            var hero = GetUnitAtSlot(TeamHero, slot);
            if (hero == null) continue;
            if (IsDead(hero)) continue;

            
                hero.SetClear();
        }
     
        if (afterClearDelay > 0f)
            yield return new WaitForSeconds(afterClearDelay);
       
        // 2) Wait until all alive heroes finished clear movement/animation (IsClear == false)
        yield return new WaitUntil(AllAliveHeroesClearFinished);
        
        clearImage.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        // 3) Load next wave if exists
        if (battleManager == null || battleManager.StageConfig == null)
            yield break;

        int maxWave = Mathf.Max(1, battleManager.StageConfig.waveStage);
        if (currentWave >= maxWave)
        {
            // No more wave -> stop battle loop
            yield break;
        }

        currentWave++;

        // Important: BattleManager.LoadWave will destroy and respawn units.
        // This coroutine will continue and wait for IsWaveReady at the top of loop.
        battleManager.LoadWave(currentWave);
        turnText.text = $"1/20";
        clearImage.SetActive(false);
    }

    private bool AllAliveHeroesClearFinished()
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var hero = GetUnitAtSlot(TeamHero, slot);
            if (hero == null) continue;
            if (IsDead(hero)) continue;

            // if any alive hero still clearing -> not finished
            if (hero.IsClear)
                return false;
        }

        return true;
    }

    public void SetCanSkill()
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.CanSkill = false;
        }

        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamEnemy, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.CanSkill = false;
        }

        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;

            if (ShouldUseSkill(unit))
                unit.CanSkill = true;
        }

        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamEnemy, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;

            if (ShouldUseSkill(unit))
                unit.CanSkill = true;
        }
    }

    private bool DecideHeroTeamStarts()
    {
        float heroTotal = SumTeamSpeed(TeamHero);
        float enemyTotal = SumTeamSpeed(TeamEnemy);

        bool heroStarts = heroTotal >= enemyTotal;
        Debug.Log($"[BattleTurnManager] TeamSpeed: Hero={heroTotal:0.##} Enemy={enemyTotal:0.##}. HeroStarts={heroStarts}");
        return heroStarts;
    }

    private float SumTeamSpeed(string teamTag)
    {
        if (BattlefieldRegistry.Instance == null)
            return 0f;

        float sum = 0f;
        var teamRoots = BattlefieldRegistry.Instance.GetUnitsByTeam(teamTag);
        for (int i = 0; i < teamRoots.Count; i++)
        {
            var root = teamRoots[i];
            if (root == null) continue;

            var hc = root.GetComponent<HeroControl>();
            if (hc == null) continue;

            if (IsDead(hc)) continue;

            sum += BattlefieldRegistry.Instance.GetUnitSpeed(root);
        }

        return sum;
    }
    private IEnumerator CoSoulBattlePhase()
    {
        if (heroTeamStarts)
        {
            yield return CoTeamSoulBattle(TeamHero);
            yield return CoTeamSoulBattle(TeamEnemy);
        }
        else
        {
            yield return CoTeamSoulBattle(TeamEnemy);
            yield return CoTeamSoulBattle(TeamHero);
        }
    }
    private IEnumerator CoPassiveBattlePhase()
    {
        if (heroTeamStarts)
        {
            yield return CoTeamPassiveBattle(TeamHero);
            yield return CoTeamPassiveBattle(TeamEnemy);
        }
        else
        {
            yield return CoTeamPassiveBattle(TeamEnemy);
            yield return CoTeamPassiveBattle(TeamHero);
        }
    }
    private IEnumerator CoUltimatePhase()
    {
        if (heroTeamStarts)
        {
            yield return CoTeamUltimate(TeamHero);
            yield return new WaitForSeconds(1f);
            yield return CoTeamUltimate(TeamEnemy);
        }
        else
        {
            yield return CoTeamUltimate(TeamEnemy);
            yield return new WaitForSeconds(1f);
            yield return CoTeamUltimate(TeamHero);
        }
    }

    private IEnumerator CoNormalSkillPhase()
    {
        if (heroTeamStarts)
        {
            yield return CoTeamNormalSkill(TeamHero);
            yield return new WaitForSeconds(1f);
            yield return CoTeamNormalSkill(TeamEnemy);
        }
        else
        {
            yield return CoTeamNormalSkill(TeamEnemy);
            yield return new WaitForSeconds(1f);
            yield return CoTeamNormalSkill(TeamHero);
        }
    }

    private IEnumerator CoTeamUltimate(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;

            unit.IsFinished = false;

            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;

            if (reC.HeroStatRuntime.CurrentMana < reC.HeroStatRuntime.MaxMana) continue;

           
            List<AbilityEffect> effectOnAttack = reC.HeroInfo.ultimate.GetEffectsOnAttack();
                for (int i = 0; i < effectOnAttack.Count; i++)
                {
                    var effect = effectOnAttack[i];
                    if (effect.type == AbilityEffectType.ModifyStat)
                    {
                        if(effect.target == AbilityTarget.HeroAll) 
                            ApplyStatAllStartBattle(effect.statType, effect.modifyValue);
                        else if(effect.target == AbilityTarget.Self)
                            unit.HeroStatRuntime.ApplyStats(effect.statType, effect.modifyValue,false);
                }
                 
                
            }
            unit.SetUltimate();
            yield return new WaitUntil(() => unit.IsFinished);

            if (delayBetweenUltimates > 0f)
                yield return new WaitForSeconds(delayBetweenUltimates);

            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }

        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }
    private IEnumerator CoTeamSoulBattle(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.IsFinished = false;
            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;
            if (reC.HeroInfo.soulID == null) continue;
            HeroInstance instance = PlayerInventory.Instance.GetHeroInstance(reC.HeroInfo.ID);
            if (reC.HeroInfo.soulID == null || reC.HeroInfo.soulID.Count == 0)
                continue;

            int count = Mathf.Min(1, reC.HeroInfo.soulID.Count); // hiện tại vẫn giới hạn 1
            for (int i = 0; i < count; i++)
            {
                int soulId = reC.HeroInfo.soulID[i];
                FightSoulInfo soul = DatabaseManager.Instance.FightSoulDatabase.GetSoulInfo(soulId);
                if (soul != null)
                    reC.HeroStatRuntime.GainValueBySoul(instance, soul);
            }

        }
        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }
    private IEnumerator CoTeamPassiveBattle(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;

            unit.IsFinished = false;

            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;

            if (reC.HeroInfo.passive == null) continue;
            List<AbilityEffect> effectBattle = reC.HeroInfo.passive.GetEffectsStartBattle();
            if (effectBattle.Count == 0) continue;
            for (int i = 0; i < effectBattle.Count; i++)
            {
                var effect = effectBattle[i];
                if (effect.type == AbilityEffectType.ModifyStat)
                {
                    if(effect.target == AbilityTarget.HeroAll) 
                        ApplyStatAllStartBattle(effect.statType, effect.modifyValue);
                }
            }

            


            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }

        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }
    void ApplyStatAllStartBattle(ModifyStatType type, float  value)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.HeroStatRuntime.ApplyStats(type, value, true);

        }
    }
    private IEnumerator CoTeamNormalSkill(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;

            unit.IsFinished = false;

            if (unit.CanSkill)
                unit.SetSkill();
            else
                unit.SetAttack();

            yield return new WaitUntil(() => unit.IsFinished);

            if (delayBetweenActions > 0f)
                yield return new WaitForSeconds(delayBetweenActions);

            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }
    }

    private static bool ShouldUseSkill(HeroControl unit)
    {
        if (unit == null || unit.HeroInfo == null || unit.HeroInfo.skill == null)
            return false;

        float chance = unit.HeroInfo.skillChance;
        return chance > 0f && Random.value < chance;
    }

    private static bool IsDead(HeroControl unit)
    {
        var recv = unit != null ? unit.GetComponentInChildren<HeroReceiveDamagee>() : null;
        return recv != null && recv.IsDead;
    }

    private bool AreAllTeamDead(string teamTag)
    {
        bool anyUnit = false;

        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;

            anyUnit = true;

            if (!IsDead(unit))
                return false;
        }

        return anyUnit;
    }

    private HeroControl GetUnitAtSlot(string teamTag, int slotIndex1To6)
    {
        if (BattlefieldRegistry.Instance == null)
            return null;

        var all = BattlefieldRegistry.Instance.GetUnitsByTeam(teamTag);
        for (int i = 0; i < all.Count; i++)
        {
            var root = all[i];
            if (root == null) continue;

            if (!BattlefieldRegistry.Instance.TryGetSlotIndex(root, out int slot)) continue;
            if (slot != slotIndex1To6) continue;

            return root.GetComponent<HeroControl>();
        }

        return null;
    }
}