using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BattleManager battleManager;

    [Header("Turn Config")]
    [Min(1)] public int maxTurns = 20;

    [Tooltip("Delay between each hero action (seconds).")]
    [Min(0f)] public float delayBetweenActions = 0.15f;

    [Header("Ultimate Phase")]
    [Tooltip("Delay between each ultimate cast and the next unit (seconds).")]
    [Min(0f)] public float delayBetweenUltimates = 0.05f;

    private const string TeamHero = "Hero";
    private const string TeamEnemy = "Enemy";

    private bool heroTeamStarts;

    private void Awake()
    {
        if (battleManager == null)
            battleManager = FindFirstObjectByType<BattleManager>();
    }

    private void Start()
    {
        StartCoroutine(CoBattleLoop());
    }

    private IEnumerator CoBattleLoop()
    {
        while (battleManager == null || !battleManager.IsWaveReady)
            yield return null;

        heroTeamStarts = DecideHeroTeamStarts();

        for (int turn = 1; turn <= maxTurns; turn++)
        {
           yield return new WaitForSeconds(2f);
            yield return CoNormalSkillPhase();
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

    private IEnumerator CoUltimatePhase()
    {
        if (heroTeamStarts)
        {
            yield return CoTeamUltimate(TeamHero);
            yield return CoTeamUltimate(TeamEnemy);
        }
        else
        {
            yield return CoTeamUltimate(TeamEnemy);
            yield return CoTeamUltimate(TeamHero);
        }
    }

    private IEnumerator CoNormalSkillPhase()
    {
        if (heroTeamStarts)
        {
            yield return CoTeamNormalSkill(TeamHero);
            yield return new WaitForSeconds(2f);
            yield return CoTeamNormalSkill(TeamEnemy);
        }
        else
        {
            yield return CoTeamNormalSkill(TeamEnemy);
            yield return new WaitForSeconds(2f);
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

            var recv = unit.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv == null) continue;

            if (recv.Mana < recv.MaxMana) continue;

            // Consume mana immediately to lock-out more ult in this phase.
            recv.Mana = 0f;
            unit.RefreshObservers();

            unit.SetUltimate();

            yield return WaitForActionFinished(unit, 5.0f);

            if (delayBetweenUltimates > 0f)
                yield return new WaitForSeconds(delayBetweenUltimates);
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

            unit.IsFinished = false;
            if (ShouldUseSkill(unit))
                unit.SetSkill();
            else
                unit.SetAttack();

            //yield return WaitForActionFinished(unit, 1f);
            yield return new WaitUntil(() => unit.IsFinished);

            if (delayBetweenActions > 0f)
                yield return new WaitForSeconds(delayBetweenActions);
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

    private static IEnumerator WaitForActionFinished(HeroControl unit, float timeoutSeconds)
    {
        if (unit == null)
            yield break;

        float t = 0f;
        while (t < timeoutSeconds)
        {
            if (!unit.ActionInProgress)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }

        unit.NotifyActionFinished();
    }
}