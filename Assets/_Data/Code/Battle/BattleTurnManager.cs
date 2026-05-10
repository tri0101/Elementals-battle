using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BattleTurnManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject clearImage;
    [SerializeField] private GameObject winExpPlusImage;
    [Header("Defeat UI")]
    [SerializeField] private GameObject panelDefeat;


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

    //// Skip turn effects: unit nào bị effect cấm đánh trong turn này sẽ skip cả turn (ultimate + normal)
    //private readonly HashSet<HeroControl> skipThisTurn = new HashSet<HeroControl>();

    //// Skip turn effects: cuối turn mới trừ duration (để UI chỉ tắt/bật ở cuối turn)
    //private readonly HashSet<HeroControl> consumeSkipAtEndOfTurn = new HashSet<HeroControl>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        isEnd = false;
        BattleManager.Instance.SetActiveForUIBatle(false);
        StartCoroutine(CoBattleLoop());
    }

    private IEnumerator CoBattleLoop()
    {
        while (!BattleManager.Instance.IsWaveReady)
            yield return null;

        currentWave = 1;

        yield return CoPassiveBattlePhase();

        while (!isEnd)
        {
            heroTeamStarts = DecideHeroTeamStarts();
            SetStartsForTeam();
            for (int turn = 1; turn <= maxTurns; turn++)
            {
                //skipThisTurn.Clear();
                //consumeSkipAtEndOfTurn.Clear();

                if (turnText != null)
                    turnText.text = $"{turn}/20";
                
               
                SetCanSkill();

                BattleManager.Instance.SetActiveForUIBatle(true);
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
                    BattleManager.Instance.SetActiveForUIBatle(false);
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
                    BattleManager.Instance.SetActiveForUIBatle(false);
                    yield return new WaitForSeconds(0.5f);
                    yield return CoHandleWaveCleared();
                    break;
                }
                ConsumeModifyStatForTeam(TeamHero);
                ConsumeModifyStatForTeam(TeamEnemy);
                yield return new WaitForSeconds(0.5f);
                // DOT tick + giảm duration DOT ở cuối full round (2 team cùng lúc)
                yield return CoApplyEffect(false);

                // NEW: cuối turn mới trừ skip effects để UI chỉ cập nhật/tắt ở cuối turn
                ConsumeSkipEffectsAtEndOfTurn();

            }

           

            while (!BattleManager.Instance.IsWaveReady)
                yield return null;
        }
    }

    //private bool TrySkipActionIfDisabled(HeroControl unit)
    //{
    //    if (unit == null || unit.HeroStatRuntime == null)
    //        return false;

    //    // đã đánh dấu skip trong turn => mọi phase đều skip
    //    if (skipThisTurn.Contains(unit))
    //        return true;

    //    // Kiểm tra tất cả effect cấm đánh
    //    if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Rooted) ||
    //        unit.HeroStatRuntime.HasAES(AbilityEffectType.Stun) ||
    //        unit.HeroStatRuntime.HasAES(AbilityEffectType.Sleep) ||
    //        unit.HeroStatRuntime.HasAES(AbilityEffectType.Freeze)||
    //        unit.HeroStatRuntime.HasAES(AbilityEffectType.Paralysis))
    //    {
    //        skipThisTurn.Add(unit);
    //        consumeSkipAtEndOfTurn.Add(unit);
    //        return true;
    //    }

    //    return false;
    //}
    private bool TrySkipActionIfDisabled(HeroControl unit)
    {
        if (unit == null || unit.HeroStatRuntime == null)
            return false;

        

        // Kiểm tra tất cả effect cấm đánh
        if (!unit.CanAttackInBattle)
        {
            unit.IsFinished = true;
            return true;
        }

        return false;
    }

    //private void ConsumeSkipEffectsAtEndOfTurn()
    //{
    //    if (consumeSkipAtEndOfTurn.Count == 0)
    //        return;

    //    foreach (var unit in consumeSkipAtEndOfTurn)
    //    {
    //        if (unit == null || unit.HeroStatRuntime == null || IsDead(unit))
    //            continue;

    //        // Trừ tất cả effect cấm đánh
    //        if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Rooted))
    //            unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Rooted);

    //        if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Stun))
    //            unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Stun);

    //        if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Sleep))
    //            unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Sleep);

    //        if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Freeze))
    //            unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Freeze);
    //        if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Paralysis))
    //            unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Paralysis);
    //    }

    //    consumeSkipAtEndOfTurn.Clear();
    //}
    private void ConsumeSkipEffectsAtEndOfTurn()
    {
        ConsumeSkipEffectsAtEndOfTurnForTeam(TeamHero);
        ConsumeSkipEffectsAtEndOfTurnForTeam(TeamEnemy);
    }
    private void ConsumeSkipEffectsAtEndOfTurnForTeam(string teamTag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
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

            if (unit.HeroStatRuntime.HasAES(AbilityEffectType.Paralysis))
                unit.HeroStatRuntime.MinusRemainTurn(AbilityEffectType.Paralysis);
        }
    }
    bool WaitForAllFinished()
    {
        //if (AreAllTeamDead(TeamHero) || AreAllTeamDead(TeamEnemy)) return false;
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamHero, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if (IsLeftBattle(unit)) continue;
            if (!unit.CanAttackInBattle) continue;

            if (!unit.IsFinished)
                return false;
        }
  
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(TeamEnemy, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if (IsLeftBattle(unit)) continue;
            if (!unit.CanAttackInBattle) continue;

            if (!unit.IsFinished)
                return false;
        }

        return true;
    }
    private IEnumerator CoTeamUltimate(string teamTag)
    {
        if(teamTag == TeamHero && BattleManager.Instance.StageConfig.stageID >= 31 && BattleManager.Instance.StageConfig.stageID <= 40)
        {
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamTag, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if(IsLeftBattle(unit)) continue;
 

            if (TrySkipActionIfDisabled(unit))
                continue;

            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;
            if (reC.HeroInfo.ultimate == null) continue;
            if (!reC.CanAttackInBattle) continue;
            if (reC.HeroStatRuntime.CurrentMana < reC.HeroStatRuntime.MaxMana)
            {
                unit.IsFinished = true;
                continue;
            }
            unit.CheckCanSpecial();
            if (unit.CanUltimateSpecial)
            {
                unit.SetTarget(unit.HeroInfo.ultimateSpecial);
            }
            else
            {
                unit.SetTarget(unit.HeroInfo.ultimate);
            }

            unit.IsFinished = false;
            string skillName = reC.HeroInfo.ultimate.abilityName;
            List<AbilityEffect> effectOnUse = reC.HeroInfo.ultimate.GetEffectsOnUse();
            List<AbilityEffect> effectOnAttack = reC.HeroInfo.ultimate.GetEffectsOnAttack();
            ApplyEffectOnUse(effectOnUse, effectOnAttack, teamTag, skillName, reC);
            
            if(unit.CanUltimateSpecial)
                unit.SetUltimateSpecial();
            else
                unit.SetUltimate();
            //yield return new WaitUntil(() => unit.IsFinished);
            yield return new WaitUntil(() => WaitForAllFinished());

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


            if (TrySkipActionIfDisabled(unit))
                continue;

            if (!unit.CanAttackInBattle)
            {
                unit.IsFinished = true;
                continue;
            }
            if (unit.CanSkill)
            {
                unit.SetTarget(unit.HeroInfo.skill);
                //if (unit.IsFinished) continue;
                string skillName = unit.HeroInfo.skill.abilityName;
                List<AbilityEffect> effectOnUse = unit.HeroInfo.skill.GetEffectsOnUse();
                List<AbilityEffect> effectOnAttack = unit.HeroInfo.skill.GetEffectsOnAttack();
                ApplyEffectOnUse(effectOnUse, effectOnAttack, teamTag, skillName, unit);
            }
            else
            {
                unit.SetTarget(unit.HeroInfo.normalAttack);
                //if (unit.IsFinished) continue;
                string skillName = unit.HeroInfo.normalAttack.abilityName;
                List<AbilityEffect> effectOnUse = unit.HeroInfo.normalAttack.GetEffectsOnUse();
                List<AbilityEffect> effectOnAttack = unit.HeroInfo.normalAttack.GetEffectsOnAttack();
                ApplyEffectOnUse(effectOnUse, effectOnAttack, teamTag, skillName, unit);
            }
            unit.IsFinished = false;
            if (unit.CanSkill)
                unit.SetSkill();
            else
                unit.SetAttack();

            //yield return new WaitUntil(() => unit.IsFinished);
            yield return new WaitUntil(() => WaitForAllFinished());

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
            AbilityTarget target = effect.target;
            if (effect.statType == ModifyStatType.HealingRate)
            {
                unit.SetShouldPlus(false);
            }
            else
            {
                bool value = shouldPlus && effect.shouldPlus(target);
                unit.SetShouldPlus(value);
            }
        }
        for (int i = 0; i < effectOnUse.Count; i++)
        {
            var effect = effectOnUse[i];
            AbilityTarget target = effect.target;
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
                    duration = shouldPlus && effect.shouldPlus(target) ?
                    effect.durationTurn + 1 : effect.durationTurn;
                }
                if (effect.target == AbilityTarget.HeroAll)
                {
                    ApplyModifyStatAll(skillName, effect.statType, duration, effect.modifyValue, effect.stackCount);
                }
                else if (effect.target == AbilityTarget.Self)
                {
                    unit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, duration, effect.modifyValue, effect.stackCount, unit);
                }
                else if (effect.target == AbilityTarget.CurrentTarget)
                {
                    foreach (Transform enemy in unit.enemyTarget)
                    {
                        var targetUnit = enemy.GetComponent<HeroControl>();
                        if (targetUnit == null) continue;
                        targetUnit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, duration, effect.modifyValue, effect.stackCount, unit);
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
                case AbilityEffectType.Burn or AbilityEffectType.Poison or AbilityEffectType.Bleeding:
                    bool shouldTakeHit = i == 0;// burn index luôn = 0
                   
                    unit.HeroReceiveDamagee.ReceiveDamage(aes.damagePerTurn, DamageType.turnDamage, shouldTakeHit, true, aes.attacker);
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
        if (currentWave >= BattleManager.Instance.StageConfig.waveStage)
        {
            isEnd = true;
            Time.timeScale = 1f;

            for (int slot = 1; slot <= 6; slot++)
            {
                var hero = GetUnitAtSlot(TeamHero, slot);
                if (hero == null) continue;

                if (IsDead(hero))
                {
                    BattleManager.Instance.BattleResult.SetList(slot, false);
                    BattleManager.Instance.BattleResult.SetListByID(hero.HeroInfo.ID, false);
                }
                else
                {
                    BattleManager.Instance.BattleResult.SetListByID(hero.HeroInfo.ID, true);
                    BattleManager.Instance.BattleResult.SetList(slot, true);
                }
            }

            ProgressManager.Instance.UpdateStage(BattleManager.Instance.StageConfig.stageID);

            BattleManager.Instance.BattleResult.SetUIExpPlus();
            BattleManager.Instance.BattleResult.SetExpPlus();
            BattleManager.Instance.BattleResult.SetExpForPlayer();
            winExpPlusImage.SetActive(true);
            BattleManager.Instance.BattleResult.CheckHeroesLost();
            BattleManager.Instance.BattleResult.SetUpRollItems();
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

        if ( BattleManager.Instance.StageConfig == null)
            yield break;

        int maxWave = Mathf.Max(1, BattleManager.Instance.StageConfig.waveStage);
        if (currentWave >= maxWave)
            yield break;

        currentWave++;

        BattleManager.Instance.LoadWave(currentWave);
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
    private void SetStartsForTeam()
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unitHero = GetUnitAtSlot(TeamHero, slot);
            var unitEnemy = GetUnitAtSlot(TeamEnemy, slot);

            if (unitHero != null)
                unitHero.IsStart = heroTeamStarts;

            if (unitEnemy != null)
                unitEnemy.IsStart = !heroTeamStarts;
        }
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
            //unit.IsFinished = false;

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
                                effect.durationTurn, effect.modifyValue, effect.stackCount, reC);
                        }
                    }


                }
            }
            else if (reC.HeroInfo.ID == 53)
            {
                // Duyệt các hero nữ trong team của hero 53 và giải khống chế ngẫu nhiên 1 hero
                var femaleHeroes = new List<HeroControl>(6);

                for (int s = 1; s <= 6; s++)
                {
                    var h = GetUnitAtSlot(teamTag, s);
                    if (h == null) continue;
                    if (IsDead(h)) continue;
                    if (IsLeftBattle(h)) continue;
                    if (h.HeroStatRuntime == null) continue;
                    if (h.HeroInfo == null) continue;

                    if (h.HeroInfo.GetTag(Tag.Female) && h.HeroStatRuntime.HasAnyAES())
                        femaleHeroes.Add(h);
                }

                if (femaleHeroes.Count > 0)
                {
                    var target = femaleHeroes[Random.Range(0, femaleHeroes.Count)];
                    if (target != null && target.HeroStatRuntime != null)
                    {
                        target.HeroReceiveDamagee.ClearEffectByName("Cleanse");
                        target.HeroReceiveDamagee.CallSpawnEffectHero("Cleanse", new Vector3(0, 0.5f, -0.1f));
                        target.HeroStatRuntime.ClearAllAES();
                        target.CanAttackInBattle = true;
                     
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
            var reC = unit.GetComponent<HeroControl>();
            if (reC == null) continue;

            if (reC.HeroInfo.passive == null) continue;
            List<AbilityEffect> effectBattle = reC.HeroInfo.passive.GetEffectsStartBattle();
            if (effectBattle.Count == 0) continue;
            HeroInstance heroInstance = PlayerInventory.Instance.GetHeroInstance(unit.HeroInfo.ID);
            int passiveLevel = heroInstance.GetAbilityLevel(AbilityType.Passive);
            for (int i = 0; i < effectBattle.Count; i++)
            {
                var effect = effectBattle[i];
                string tagCurrentUnit = unit.transform.tag;
                if (effect.type == AbilityEffectType.ModifyStat)
                {
                    float finalStat = effect.modifyValue;
                    if (effect.canUpgrade)
                    {
                        finalStat +=  passiveLevel * effect.valueUpPerLevel;
                    }
                    

                    if (effect.target == AbilityTarget.HeroAll)
                        ApplyStatAll(effect.statType, effect.modifyValue);
                    else if (effect.target == AbilityTarget.DPSHeroAll)
                        ApplyStatCertainRoleBattle(tagCurrentUnit, effect.statType, finalStat, RoleHero.DPS);
                    else if (effect.target == AbilityTarget.TankHeroAll)
                        ApplyStatCertainRoleBattle(tagCurrentUnit, effect.statType, finalStat, RoleHero.Tank);
                    else if (effect.target == AbilityTarget.SupportHeroAll)
                        ApplyStatCertainRoleBattle(tagCurrentUnit, effect.statType, finalStat, RoleHero.Support);
                    else if (effect.target == AbilityTarget.HeroTagMale)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Male);
                    else if (effect.target == AbilityTarget.HeroTagFemale)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Female);
                    else if (effect.target == AbilityTarget.HeroTagAssassin)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Assassin);
                    else if (effect.target == AbilityTarget.HeroTagAwakened)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Awakened);
                    else if (effect.target == AbilityTarget.HeroTagWarrior)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Warrior);
                    else if (effect.target == AbilityTarget.HeroTagDemon)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Demon);
                    else if (effect.target == AbilityTarget.HeroTagFighter)
                        ApplyStatCertainTagBattle(tagCurrentUnit, effect.statType, finalStat, Tag.Fighter);
                }       
            }
            // riêng hero id = 59
            if (reC.HeroInfo.ID == 59 && reC.HeroStatRuntime != null)
            {
                int nextSlot = slot + 3;
                if (nextSlot >= 1 && nextSlot <= 6)
                {
                    var nextHero = GetUnitAtSlot(teamTag, nextSlot);
                    if (nextHero != null && !IsDead(nextHero) && nextHero.HeroStatRuntime != null && nextHero.HeroInfo.ID != 51)
                    {
                        
                        float shieldValue = 0.25f * reC.HeroStatRuntime.MaxHealth;
                        if (shieldValue > 0f)
                            nextHero.HeroStatRuntime.GainMaxShield(shieldValue);
                    }
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
            unit.HeroStatRuntime.ApplyModifyStat(nameAbility, type, turns, value, maxStack, unit);
        }
    }

    void ApplyStatCertainRoleBattle(string teamApply, ModifyStatType type, float value, RoleHero role)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamApply, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if (unit.HeroInfo.role != role) continue;
            unit.HeroStatRuntime.ApplyStatOnStartBattle(type, (int)value);
        }
    }
    void ApplyStatCertainTagBattle(string teamApply ,ModifyStatType type, float value, Tag tag)
    {
        for (int slot = 1; slot <= 6; slot++)
        {
            var unit = GetUnitAtSlot(teamApply, slot);
            if (unit == null) continue;
            if (IsDead(unit)) continue;
            if(unit.HeroInfo.GetTag(tag) == false) continue;
            unit.HeroStatRuntime.ApplyStatOnStartBattle(type, (int)value);
        }
    }
    
    bool HasTagInTeam(HeroControl heroControl, Tag tag)
    {
        foreach(Tag tag1 in heroControl.HeroInfo.tags)
        {
            if (tag1 == tag)
                return true;
        }
        return false;
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

        float chance = unit.HeroStatRuntime.skillChanceFinal;
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