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
    [Header("Defeat UI")]
    [SerializeField] private GameObject panelDefeat;
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

    [SerializeField] private bool heroTeamStarts;
    private bool isEnd;

    private int currentWave = 1;

    // Skip turn effects: unit nào bị effect cấm đánh trong turn này sẽ skip cả turn (ultimate + normal)
    private readonly HashSet<HeroControl> skipThisTurn = new HashSet<HeroControl>();

    // Skip turn effects: cuối turn mới trừ duration (để UI chỉ tắt/bật ở cuối turn)
    private readonly HashSet<HeroControl> consumeSkipAtEndOfTurn = new HashSet<HeroControl>();

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
        while (battleManager == null || !battleManager.IsWaveReady)
            yield return null;

        currentWave = 1;

        yield return CoPassiveBattlePhase();

        while (!isEnd)
        {
            heroTeamStarts = DecideHeroTeamStarts();

            for (int turn = 1; turn <= maxTurns; turn++)
            {
                skipThisTurn.Clear();
                consumeSkipAtEndOfTurn.Clear();

                if (turnText != null)
                    turnText.text = $"{turn}/20";
                
               
                SetCanSkill();

                battleManager.SetActiveForUIBatle(true);
                if (currentWave == 1 && turn == 1) yield return new WaitForSeconds(2f);
                else if (turn == 1) yield return new WaitForSeconds(2f);
                else yield return new WaitForSeconds(1f);
                
               
                if (AreAllTeamDead(TeamHero))
                {
                    HandleDefeat();
                    yield break;
                }
                else if (AreAllTeamDead(TeamEnemy))
                {
                    yield return CoHandleWaveCleared();
                    break;
                }

                string firstTeam = heroTeamStarts ? TeamHero : TeamEnemy;
                string secondTeam = heroTeamStarts ? TeamEnemy : TeamHero;



                yield return SetUpStartTurn(firstTeam);
                yield return CoTeamUltimate(firstTeam);
                if (AreAllTeamDead(TeamHero))
                {
                    HandleDefeat();
                    yield break;
                }
                else if (AreAllTeamDead(TeamEnemy))
                {
                    yield return new WaitForSeconds(1f);
                    yield return CoHandleWaveCleared();

                    break;
                }
                yield return new WaitForSeconds(0.5f);
                yield return CoTeamNormalSkill(firstTeam);
                if (AreAllTeamDead(TeamHero))
                {
                    HandleDefeat();
                    yield break;
                }
                else if (AreAllTeamDead(TeamEnemy))
                {
                    battleManager.SetActiveForUIBatle(false);
                    yield return new WaitForSeconds(0.5f);
                    yield return CoHandleWaveCleared();
                    break;
                }

                yield return new WaitForSeconds(0.5f);
                yield return SetUpStartTurn(secondTeam);
                yield return CoTeamUltimate(secondTeam);
                if (AreAllTeamDead(TeamEnemy))
                {
                    yield return new WaitForSeconds(1f);
                    yield return CoHandleWaveCleared();
                    break;
                }
                yield return new WaitForSeconds(0.5f);
                yield return CoTeamNormalSkill(secondTeam);
                if (AreAllTeamDead(TeamHero))
                {
                    HandleDefeat();
                    yield break;
                }
                else if (AreAllTeamDead(TeamEnemy))
                {
                    battleManager.SetActiveForUIBatle(false);
                    yield return new WaitForSeconds(0.5f);
                    yield return CoHandleWaveCleared();
                    break;
                }
                ConsumeModifyStatForTeam(TeamHero);
                ConsumeModifyStatForTeam(TeamEnemy);
                // DOT tick + giảm duration DOT ở cuối full round (2 team cùng lúc)
                yield return CoApplyEffect(false);

                // NEW: cuối turn mới trừ skip effects để UI chỉ cập nhật/tắt ở cuối turn
                ConsumeSkipEffectsAtEndOfTurn();

            }

            if (battleManager == null)
                yield break;

            while (!battleManager.IsWaveReady)
                yield return null;
        }
    }

    private bool TrySkipActionIfDisabled(HeroControl unit)
    {
        if (unit == null || unit.HeroStatRuntime == null)
            return false;

        // đã đánh dấu skip trong turn => mọi phase đều skip
        if (skipThisTurn.Contains(unit))
            return true;

        // Kiểm tra tất cả effect cấm đánh
        if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Rooted) ||
            unit.HeroStatRuntime.HasAES(AbilityEffectType.Stun) ||
            unit.HeroStatRuntime.HasAES(AbilityEffectType.Sleep) ||
            unit.HeroStatRuntime.HasAES(AbilityEffectType.Freeze))
        {
            skipThisTurn.Add(unit);
            consumeSkipAtEndOfTurn.Add(unit);
            return true;
        }

        return false;
    }

    private void ConsumeSkipEffectsAtEndOfTurn()
    {
        if (consumeSkipAtEndOfTurn.Count == 0)
            return;

        foreach (var unit in consumeSkipAtEndOfTurn)
        {
            if (unit == null || unit.HeroStatRuntime == null || IsDead(unit))
                continue;

            // Trừ tất cả effect cấm đánh
            if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Rooted))
                unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Rooted);

            if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Stun))
                unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Stun);

            if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Sleep))
                unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Sleep);

            if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Freeze))
                unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Freeze);
        }

        consumeSkipAtEndOfTurn.Clear();
    }

    private IEnumerator CoTeamUltimate(string teamTag)
    {
        
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if(IsLeftBattle(unit)) continue;
            unit.IsFinished = false;

            if (TrySkipActionIfDisabled(unit))
                continue;

            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;
            if (reC.HeroInfo.ultimate == null) continue;
            if (!reC.CanAttackInBattle) continue;
            if (reC.HeroStatRuntime.CurrentMana < reC.HeroStatRuntime.MaxMana) continue;
            unit.CheckCanSpecial();
            if (unit.CanUltimateSpecial)
            {
                unit.SetTarget(unit.HeroInfo.ultimateSpecial);
            }
            else
            {
                unit.SetTarget(unit.HeroInfo.ultimate);
            }

            if (unit.IsFinished) continue;
            string skillName = reC.HeroInfo.ultimate.abilityName;
            List<AbilityEffect> effectOnUse = reC.HeroInfo.ultimate.GetEffectsOnUse();
            List<AbilityEffect> effectOnAttack = reC.HeroInfo.ultimate.GetEffectsOnAttack();
            ApplyEffectOnUse(effectOnUse, effectOnAttack, teamTag, skillName, reC);
            
            if(unit.CanUltimateSpecial)
                unit.SetUltimateSpecial();
            else
                unit.SetUltimate();
            yield return new WaitUntil(() => unit.IsFinished);

            if (delayBetweenUltimates > 0f)
                yield return new WaitForSeconds(delayBetweenUltimates);
            if(AreAllTeamDead(TeamHero))
                yield break;
            
            else
            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }

        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }

    private IEnumerator CoTeamNormalSkill(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if(IsLeftBattle(unit)) continue;
            unit.IsFinished = false;

            if (TrySkipActionIfDisabled(unit))
                continue;

            if (!unit.CanAttackInBattle) continue;
            if (unit.CanSkill)
            {
                unit.SetTarget(unit.HeroInfo.skill);
                if (unit.IsFinished) continue;
                string skillName = unit.HeroInfo.skill.abilityName;
                List<AbilityEffect> effectOnUse = unit.HeroInfo.skill.GetEffectsOnUse();
                List<AbilityEffect> effectOnAttack = unit.HeroInfo.skill.GetEffectsOnAttack();
                ApplyEffectOnUse(effectOnUse, effectOnAttack, teamTag, skillName, unit);
            }
            else
            {
                unit.SetTarget(unit.HeroInfo.normalAttack);
                if (unit.IsFinished) continue;
                string skillName = unit.HeroInfo.normalAttack.abilityName;
                List<AbilityEffect> effectOnUse = unit.HeroInfo.normalAttack.GetEffectsOnUse();
                List<AbilityEffect> effectOnAttack = unit.HeroInfo.normalAttack.GetEffectsOnAttack();
                ApplyEffectOnUse(effectOnUse, effectOnAttack, teamTag, skillName, unit);
            }

            if (unit.CanSkill)
                unit.SetSkill();
            else
                unit.SetAttack();

            yield return new WaitUntil(() => unit.IsFinished);

            if (delayBetweenActions > 0f)
                yield return new WaitForSeconds(delayBetweenActions);

            if (AreAllTeamDead(TeamHero))
                yield break;

            else
            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }
    }

    bool checkShouldPlusTurn(string teamTag)
    {
        if (teamTag == TeamHero && !heroTeamStarts)
            return true;
        else if (teamTag == TeamEnemy && heroTeamStarts)
            return true;
        else
            return false;
    }

    void ApplyEffectOnUse(List<AbilityEffect> onUse, List<AbilityEffect> onAttack, string teamTag, string skillName, HeroControl unit)
    {
        List<AbilityEffect> effectOnUse = onUse;
        List<AbilityEffect> effectOnAttack = onAttack;
        for (int i = 0; i < effectOnAttack.Count; i++)
        {
            var effect = effectOnAttack[i];
            bool shouldPlus = checkShouldPlusTurn(teamTag);
            if (effect.statType == ModifyStatType.HealingRate)
            {
                unit.SetShouldPlus(false);
            }
            else
            {
                bool value = shouldPlus && effect.shouldPlus();
                unit.SetShouldPlus(value);
            }
        }
        for (int i = 0; i < effectOnUse.Count; i++)
        {
            var effect = effectOnUse[i];
            float chance = Mathf.Clamp01(effect.chance);
            if (chance <= 0f) continue;
            if (chance < 1f && Random.value > chance) continue;
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                bool shouldPlus = checkShouldPlusTurn(teamTag);
                int duration;
                if (effect.statType == ModifyStatType.HealingRate)
                {
                    duration = effect.durationTurn;
                }
                else
                {
                    duration = shouldPlus && effect.shouldPlus() ?
                    effect.durationTurn + 1 : effect.durationTurn;
                }
                if (effect.target == AbilityTarget.HeroAll)
                {
                    ApplyModifyStatAll(skillName, effect.statType, duration, effect.modifyValue, effect.stackCount);
                }
                else if (effect.target == AbilityTarget.Self)
                {
                    unit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, duration, effect.modifyValue, effect.stackCount);
                }
                else if (effect.target == AbilityTarget.CurrentTarget)
                {
                    foreach (Transform enemy in unit.enemyTarget)
                    {
                        var targetUnit = enemy.GetComponent<HeroControl>();
                        if (targetUnit == null) continue;
                        targetUnit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, duration, effect.modifyValue, effect.stackCount);
                    }
                }
            }
        }
    }

    private IEnumerator CoApplyEffect(bool checkAtStart)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var hero = GetUnitAtSlot(TeamHero, slot);
            var enemy = GetUnitAtSlot(TeamEnemy, slot);

            if (hero != null && !IsDead(hero))
                yield return CoApplyEffectForUnit(hero, checkAtStart);

            if (enemy != null && !IsDead(enemy))
                yield return CoApplyEffectForUnit(enemy, checkAtStart);

            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }

        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }

    private IEnumerator CoApplyEffectForUnit(HeroControl unit, bool checkAtStart)
    {
        if (unit == null) yield break;

        var aesList = unit.HeroStatRuntime.GetAESSnapshot();

        for (int i = 0; i < aesList.Count; i++)
        {
            var aes = aesList[i];
            if (aes.remainingTurn <= 0) continue;

            switch (aes.type)
            {
                case AbilityEffectType.Burn:
                    bool shouldTakeHit = i == 0;
                    unit.HeroReceiveDamagee.ReceiveDamage(aes.damagePerTurn, DamageType.normalDamage, shouldTakeHit, true);
                    if (delayBetweenActions > 0f)
                        yield return new WaitForSeconds(0.1f);
                    break;
            }
        }

        if (aesList.Count > 0)
            unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Burn);
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
    private IEnumerator CoSetUpStartTurn()
    {
       
        yield return SetUpStartTurn(TeamHero);
        yield return SetUpStartTurn(TeamEnemy);
        
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
        if (heroTeamStarts)
        {
            yield return CoApplyPassiveBattle(TeamHero);
            yield return CoApplyPassiveBattle(TeamEnemy);
        }
        else
        {
            yield return CoApplyPassiveBattle(TeamEnemy);
            yield return CoApplyPassiveBattle(TeamHero);
        }
    }
    

    private IEnumerator CoHandleWaveCleared()
    {
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

            ProgressManager.Instance.UpdateStage(battleManager.StageConfig.stageID);

            battleManager.BattleResult.SetUIExpPlus();
            battleManager.BattleResult.SetExpPlus();
            battleManager.BattleResult.SetExpForPlayer();
            winExpPlusImage.SetActive(true);
            battleManager.BattleResult.CheckHeroesLost();
            battleManager.BattleResult.SetUpRollItems();
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

        yield return new WaitUntil(AllAliveHeroesClearFinished);

        clearImage.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        if (battleManager == null || battleManager.StageConfig == null)
            yield break;

        int maxWave = Mathf.Max(1, battleManager.StageConfig.waveStage);
        if (currentWave >= maxWave)
            yield break;

        currentWave++;

        battleManager.LoadWave(currentWave);
        if (turnText != null)
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

            int count = Mathf.Min(1, reC.HeroInfo.soulID.Count);
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
    private IEnumerator SetUpStartTurn(string teamTag)
    {
       
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.IsFinished = false;
            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;
            if (reC.HeroInfo.ID == 57)
            {
                if (reC.LeftBattle)
                {

                    reC.HeroEventt.CallCancelStopAnim();
                    reC.LeftBattle = false;
                    reC.HeroStatRuntime.GainMana(1000);
                }
            }
            else if (reC.HeroInfo.ID == 58)
            {
                Hero58ReceiveDamage hero58 = reC.HeroReceiveDamagee as Hero58ReceiveDamage;
                if (hero58.IsDiabolicPact)
                {
                    hero58.HasBeenUsed = true;
                    hero58.IsDiabolicPact = false;
                    hero58.ClearEffect();
                    reC.HeroStatRuntime.GainHP((int)(0.25f * reC.HeroStatRuntime.MaxHealth), DamageType.normalDamage);
                    AbilityInfo passiveSkill  = reC.HeroInfo.passive;
                    List<AbilityEffect> effects = passiveSkill.GetEffectsOnSpecial();
                    string nameAbility = passiveSkill.abilityName;
                   foreach(var effect in effects)
                    {
                        if (effect.type == AbilityEffectType.ModifyStat)
                        {
                            reC.HeroStatRuntime.ApplyModifyStat(nameAbility, effect.statType,
                                effect.durationTurn, effect.modifyValue, effect.stackCount);
                        }
                    }


                }
            }


        }
        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }

    private IEnumerator CoApplyPassiveBattle(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.IsFinished = false;
            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;
            Dictionary<ModifyStatType, int> dict = reC.HeroStatRuntime.GetDicOnStartBattle();
            foreach (var key in dict.Keys)
            {
                int value = dict[key];
                if (value != 0)
                    reC.HeroStatRuntime.ApplyStats(key, value, true);
            }
            if (AreAllTeamDead(TeamEnemy))
                yield break;
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
                    if (effect.target == AbilityTarget.HeroAll)
                        ApplyStatAll(effect.statType, effect.modifyValue);
                    else if (effect.target == AbilityTarget.DPSHeroAll)
                        ApplyStatCertainRoleBattle(effect.statType, effect.modifyValue, RoleHero.DPS);
                    else if (effect.target == AbilityTarget.TankHeroAll)
                        ApplyStatCertainRoleBattle(effect.statType, effect.modifyValue, RoleHero.Tank);
                    else if (effect.target == AbilityTarget.SupportHeroAll)
                        ApplyStatCertainRoleBattle(effect.statType, effect.modifyValue, RoleHero.Support);
                }
            }

            if (AreAllTeamDead(TeamEnemy))
                yield break;
        }

        if (delayBetweenActions > 0f)
            yield return new WaitForSeconds(delayBetweenActions);
    }

    void ApplyStatAll(ModifyStatType type, float value)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.HeroStatRuntime.ApplyStatOnStartBattle(type, (int)value);
        }
    }

    void ApplyModifyStatAll(string nameAbility, ModifyStatType type, int turns, float value, int maxStack)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            unit.HeroStatRuntime.ApplyModifyStat(nameAbility, type, turns, value, maxStack);
        }
    }

    void ApplyStatCertainRoleBattle(ModifyStatType type, float value, RoleHero role)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if (unit.HeroInfo.role != role) continue;
            unit.HeroStatRuntime.ApplyStatOnStartBattle(type, (int)value);
        }
    }

    private void ConsumeModifyStatForTeam(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if (unit.HeroStatRuntime == null) continue;

            unit.HeroStatRuntime.MinusAllModifyStatRemainTurn();
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
    private static bool IsLeftBattle(HeroControl unit)
    {
        return unit != null && unit.LeftBattle;
    }
    public Transform GetHeroTransformWithLowestHpPercent()
    {
        if (BattlefieldRegistry.Instance == null)
            return null;

        Transform best = null;
        float bestHp01 = float.PositiveInfinity;

        for (int slot = 1; slot <= 6; slot++)
        {
            var hero = GetUnitAtSlot(TeamHero, slot);
            if (hero == null) continue;
            if (IsDead(hero)) continue;
            if (IsLeftBattle(hero)) continue;
            if (hero.HeroStatRuntime == null) continue;

            float maxHp = hero.HeroStatRuntime.MaxHealth;
            if (maxHp <= 0f) continue;

            float hp01 = hero.HeroStatRuntime.CurrentHealth / maxHp;

            if (hp01 < bestHp01)
            {
                bestHp01 = hp01;
                best = hero.transform;
            }
        }

        return best;
    }
    public bool HasAnyHeroHpPercentBelow(float value)
    {
        if (BattlefieldRegistry.Instance == null)
            return false;

        float threshold = Mathf.Clamp01(value);

        for (int slot = 1; slot <= 6; slot++)
        {
            var hero = GetUnitAtSlot(TeamHero, slot);
            if (hero == null) continue;
            if (IsDead(hero)) continue;
            if (IsLeftBattle(hero)) continue;
            if (hero.HeroStatRuntime == null) continue;

            float maxHp = hero.HeroStatRuntime.MaxHealth;
            if (maxHp <= 0f) continue;

            float hp01 = hero.HeroStatRuntime.CurrentHealth / maxHp;
            if (hp01 < threshold)
                return true;
        }

        return false;
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
            if(IsLeftBattle(unit))
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
    private void HandleDefeat()
    {
        isEnd = true;
        Time.timeScale = 1f;

        //if (battleManager != null)
        //    battleManager.SetActiveForUIBatle(false);

        //if (clearImage != null)
        //    clearImage.SetActive(false);

        //if (winExpPlusImage != null)
        //    winExpPlusImage.SetActive(false);

        if (panelDefeat != null)
            panelDefeat.SetActive(true);
    }
}