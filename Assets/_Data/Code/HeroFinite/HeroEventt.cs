using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroEventt : MonoBehaviour
{

    public LoadNormalAttack loadNormalAttack;
    public LoadSlideToPosition loadSlideToPosition;
    HeroControl heroControl;
    private Coroutine hideTotalDmgRoutine;
    private static readonly int[] RandomSummonHeroIds = { 500, 501, 502 };
    private void Awake()
    {
        heroControl = transform.parent.GetComponent<HeroControl>();
    }

    public void SetFinished()
    {
        heroControl.IsFinished = true;
        //heroControl.NotifyActionFinished();
       

    }
    public void SetCoroutineFinished(float value)
    {
        StartCoroutine(CoSetFinishedAfterDelay(value));
    }
    IEnumerator CoSetFinishedAfterDelay(float value)
    {
        yield return new WaitForSeconds(value);
        heroControl.IsFinished = true;
    }
    public void ChangeBackGround(AbilityType type)
    {
        switch(type)
        {
            case AbilityType.Ultimate:
                if (heroControl.HeroInfo.ultimate.isChangeBackGround)
                {
                    BattleManager.Instance.PutArenaOnStack(heroControl.HeroInfo.ultimate.abilityName,
                        heroControl.HeroInfo.name, heroControl.HeroInfo.ID, heroControl.HeroInfo.ultimate.order,
                        heroControl.HeroInfo.ultimate.backgroundChange);
                    SetEffectToEnemy(TimesToCall.OnUse);
                }
                break;
            case AbilityType.Passive:
                if (heroControl.HeroInfo.passive.isChangeBackGround)
                {
                    BattleManager.Instance.PutArenaOnStack(heroControl.HeroInfo.passive.abilityName,
                        heroControl.HeroInfo.name, heroControl.HeroInfo.ID, heroControl.HeroInfo.passive.order,
                        heroControl.HeroInfo.passive.backgroundChange);
                    //SetEffectToEnemy(TimesToCall.OnUse);
                }
                break;
        }
      
    }
    //public void ChangeBackGround()
    //{
    //    if(BattleManager.Instance.HasArenaWithSkill(heroControl.HeroInfo.ultimate.abilityName))
    //        return;
    //    if (heroControl.HeroInfo.ultimate.isChangeBackGround)
    //    {
    //        BattleManager.Instance.PutArenaOnStack(heroControl.HeroInfo.ultimate.abilityName,
    //            heroControl.HeroInfo.name, heroControl.HeroInfo.ID, heroControl.HeroInfo.ultimate.order,
    //            heroControl.HeroInfo.ultimate.backgroundChange);
    //        SetEffectToEnemy(TimesToCall.OnUse);
    //    }
    //}
    public void RemoveArena() // sử dụng khi hero có sàn
    {
        BattleManager.Instance.RemoveArenaByHeroId(heroControl.HeroInfo.ID);
        RemoveEffectFromArena();
        
    }
    void RemoveEffectFromArena()
    {
        string skillName = heroControl.HeroInfo.ultimate.abilityName;
        ModifyStatType statType = heroControl.HeroInfo.ultimate.effects[0].statType;

        switch (heroControl.HeroInfo.ultimate.effects[0].target)
        {
            case AbilityTarget.EnemyAll:
                if (BattlefieldRegistry.Instance == null)
                    return;


                string enemyTeam = heroControl.CompareTag("Hero") ? "Enemy" : "Hero";

                var enemies = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);
                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemyRoot = enemies[i];
                    if (enemyRoot == null) continue;

                    HeroControl enemyControl = enemyRoot.GetComponent<HeroControl>();
                    if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;

                    var recv = enemyRoot.GetComponentInChildren<HeroReceiveDamagee>();
                    if (recv != null && recv.IsDead) continue;

                    enemyControl.HeroStatRuntime.RemoveModifyStatBySkill(skillName, statType);
                }
                break;
        }
    }

    public void SetGainManaNormal()
    {
        int manaGain = heroControl.HeroInfo.normalAttack.manaGain;
        foreach (var soul in heroControl.HeroInfo.soulID)
        {
            if (soul == 4)
            {
                FightSoulInfo soulInfo = DatabaseManager.Instance.FightSoulDatabase.GetSoulInfo(soul);
                if (soulInfo != null)
                {
                    HeroInstance heroInstance = PlayerInventory.Instance.GetHeroInstance(heroControl.HeroInfo.ID);
                    int manaAdd = soulInfo.soulValueConfigs[heroInstance.GetLevelSoul(0) - 1].value;
                    manaGain += manaAdd;
                }
            }
           
        }
        heroControl.HeroStatRuntime.GainMana(manaGain,true);
    }
 
    public void SetGainManaSkill()
    {
        heroControl.HeroStatRuntime.GainMana(heroControl.HeroInfo.skill.manaGain, true);
    }
    public void SetEffect()
    {
        List<AbilityEffect> effectOnAttack = heroControl.HeroInfo.ultimate.GetEffectsOnAttack();
        for (int i = 0; i < effectOnAttack.Count; i++)
        {
            var effect = effectOnAttack[i];
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                if (effect.target == AbilityTarget.Self)
                    ShowTextEffect(effect.statType,(int)effect.modifyValue);
               
            }
        }
    }

    //==chỉ dành cho UI onuse
    public void SetEffectToHero(TimesToCall timeTocall)
    {
        List<AbilityEffect> effects = null;
        if (timeTocall == TimesToCall.onAttack)
        {
            effects = heroControl.HeroInfo.ultimate.GetEffectsOnAttack();
        }
        else if (timeTocall == TimesToCall.OnUse)
        {
            effects = heroControl.HeroInfo.ultimate.GetEffectsOnUse();

        }
        for (int i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                if (effect.target == AbilityTarget.Self)
                {
                    ShowTextEffect(effect.statType, (int)effect.modifyValue);
                }
            }
        }
    }
    public void SetEffectToEnemy(TimesToCall timeTocall)
    {
        List<AbilityEffect> effects = null;
        if (timeTocall == TimesToCall.onAttack)
        {
            effects = heroControl.HeroInfo.ultimate.GetEffectsOnAttack();
        }
        else if(timeTocall == TimesToCall.OnUse)
        {
            effects = heroControl.HeroInfo.ultimate.GetEffectsOnUse();

        }
            
        for (int i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                if (effect.target == AbilityTarget.CurrentTarget)
                {
                    foreach (var target in heroControl.enemyTarget)
                    {
                        HeroControl enemyControl = target.GetComponent<HeroControl>();
                        if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                        enemyControl.HeroEventt.ShowTextEffect(effect.statType, (int)effect.modifyValue);
                    }
                }
                else if (effect.target == AbilityTarget.EnemyAll)
                {
                    ModifyStatType statType = heroControl.HeroInfo.ultimate.effects[0].statType;
                    int value = (int)heroControl.HeroInfo.ultimate.effects[0].modifyValue;
                    string enemyTeam = heroControl.CompareTag("Hero") ? "Enemy" : "Hero";

                    var enemies = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        var enemyRoot = enemies[j];
                        if (enemyRoot == null) continue;

                        HeroControl enemyControl = enemyRoot.GetComponent<HeroControl>();
                        if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;

                        var recv = enemyRoot.GetComponentInChildren<HeroReceiveDamagee>();
                        if (recv != null && recv.IsDead) continue;

                        enemyControl.HeroEventt.ShowTextEffect(statType, value);
                    }
                }
            }
        }
    }
    public void ApplyMinusManaToEnemy()// hiện tại dành cho hero có id = 9 và 56
    {
        foreach (var target in heroControl.enemyTarget)
        {
            HeroControl enemyControl = target.GetComponent<HeroControl>();
            if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
            if (enemyControl.HeroInfo.ultimate == null) continue;
            enemyControl.HeroStatRuntime.MinusMana(200);
        }
    }
    public void ShowTextEffect(ModifyStatType type, int value)
    {
        switch(type)
        {
            case ModifyStatType.CritRate:
                heroControl.RefreshObservers(ModifyStatType.CritRate, value);
                break;
            case ModifyStatType.Armor:
                heroControl.RefreshObservers(ModifyStatType.Armor, value);
                break;
            case ModifyStatType.HealingRate:
                heroControl.RefreshObservers(ModifyStatType.HealingRate, value);
                break;
            case ModifyStatType.ManaRecovery:
                heroControl.RefreshObservers(ModifyStatType.ManaRecovery, value);
                break;
            case ModifyStatType.Mana:
                heroControl.RefreshObservers(ModifyStatType.Mana, value);
                break;
            case ModifyStatType.ControlFree:
                heroControl.RefreshObservers(ModifyStatType.ControlFree, value);
                break;

        }
       
    }
    public void ShowUIEffect(AbilityEffectType type)
    {
        switch(type)
        {
            case AbilityEffectType.Rooted:
                foreach (var target in heroControl.enemyTarget)
                {
                    HeroControl enemyControl = target.GetComponent<HeroControl>();
                    if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                    enemyControl.HeroStatRuntime.ApplyEartEffect();
                }
                break;
        }
    }
    public void NotifyCanDead()
    {
        foreach (Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;
            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;
            
            heroC.HeroReceiveDamagee.SetCanDead(heroControl);
        }
    }
    public void NotifyCanShowTotalDmg()
    {
        foreach (Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;
            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;
            heroC.HeroReceiveDamagee.SetCanShowTotalDmg();
        }
    }
    public void NotifyCanHideTotalDmg()
    {
        if (hideTotalDmgRoutine != null)
            StopCoroutine(hideTotalDmgRoutine);

        hideTotalDmgRoutine = StartCoroutine(CoHideTotalDmgAfterDelay(0.5f));
    }

    private IEnumerator CoHideTotalDmgAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;

            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;

            var canvas = heroC.CanvasTotalDamage;
            if (canvas == null) continue;

            canvas.Hide();
        }

        hideTotalDmgRoutine = null;
    }
    public void SpawnObjectByName(string nameObject)
    {
        NameAndVector nameAndVector = loadNormalAttack.dicSpawnName.Find(dic => dic.nameObject == nameObject);
        if (nameAndVector == null) return;
        Vector3 newPos = heroControl.transform.position;
        newPos.x += nameAndVector.positionSpawn.x;
        newPos.y += nameAndVector.positionSpawn.y;  
        ObjectSpawnPoint.instance.SpawnObjectAtPosition(newPos, nameObject, heroControl.transform.localScale.x, heroControl);
    }
    public void SpawnObjectByType(int index)
    {
        TypeAndVector typeAndvector = loadNormalAttack.dicSpawnType.Find(dic => dic.indexSpawn == index);
        if(typeAndvector == null) return;
        Vector3 newPos = heroControl.transform.position;
        newPos.x += typeAndvector.positionSpawn.x;
        newPos.y += typeAndvector.positionSpawn.y;
        List<Transform> listEnemy = heroControl.enemyTarget;
        EffectManager.Instance.Spawn(typeAndvector.type, EffectManager.Instance.ListEffect, newPos,listEnemy);
    }

    public void CallStopAnim()
    {
        
        heroControl.Animator.speed = 0f;
    }
    public void CallCancelStopAnim()
    {
        
        heroControl.Animator.speed = 1f;    
    }
    

    public void UnLeashCharge() //chỉ dành cho hero có id = 54
    {
        int damageUnleash = (int)( 0.5f * heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
            ModifyStatType.Damage, heroControl.HeroStatRuntime.Damage));
        foreach(Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;
            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;
            heroC.HeroStatRuntime.ApplyUnleashChargeEffect(damageUnleash);
        }
    }
    public void GainHPAllHero() // chỉ dành cho hero có id = 8
    {
        AbilityInfo abilityInfo = heroControl.HeroInfo.skill;
        AbilityEffect abilityeffect = abilityInfo.effects[0];

        float damage = heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
            ModifyStatType.Damage,
            heroControl.HeroStatRuntime.Damage
        );

        // modifyValue = 20 nghĩa là 20%
        int hpGain = Mathf.RoundToInt(damage * (abilityeffect.modifyValue / 100f));

        Debug.Log(hpGain);
        foreach (Transform child in heroControl.transform.parent)
        {
            HeroControl heroC = child.GetComponent<HeroControl>();
            if (heroC == null) continue;

            heroC.HeroStatRuntime.GainHP(hpGain, DamageType.normalDamage);
            
        }
    }
    public void SpawnHeroByID()// chỉ dùng cho 1 số con nhất định
    {
        if (heroControl == null)
            return;

        if (BattlefieldRegistry.Instance == null || BattleManager.Instance == null)
            return;

        if (BattleManager.Instance.Formation == null)
            return;

        if (DatabaseManager.Instance == null || DatabaseManager.Instance.HeroDatabase == null)
            return;

        string teamTag = heroControl.CompareTag("Hero") ? "Hero" : "Enemy";

        List<int> emptySlots = BattlefieldRegistry.Instance.FindListEmptySlots(teamTag);
        if (emptySlots == null || emptySlots.Count == 0)
        {
            float damage = heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
           ModifyStatType.Damage,
           heroControl.HeroStatRuntime.Damage
       );

            // modifyValue = 20 nghĩa là 20%
            int hpGain = Mathf.RoundToInt(damage * (20 / 100f));

            Debug.Log(hpGain);
            foreach (Transform child in heroControl.transform.parent)
            {
                HeroControl heroC = child.GetComponent<HeroControl>();
                if (heroC == null) continue;

                heroC.HeroStatRuntime.GainHP(hpGain, DamageType.normalDamage);

            }
            StartCoroutine(CoFinishAfterDelay(0.25f));
            return;
        }

        for (int i = 0; i < emptySlots.Count; i++)
        {
            int slot = emptySlots[i];

            int randomHeroId = RandomSummonHeroIds[UnityEngine.Random.Range(0, RandomSummonHeroIds.Length)];

            var heroInfo = DatabaseManager.Instance.HeroDatabase.GetHero(randomHeroId);
            if (heroInfo == null || heroInfo.HeroPrefab == null)
                continue;

            Transform battlePos = teamTag == "Enemy"
                ? BattleManager.Instance.Formation.GetEnemyBattle(slot)
                : BattleManager.Instance.Formation.GetBattle(slot);

            if (battlePos == null)
                continue;

            Transform parentRoot = teamTag == "Enemy"
                ? BattleManager.Instance.Formation.ListEnemyRoot
                : BattleManager.Instance.Formation.ListHeroRoot;

            if (parentRoot == null)
                parentRoot = heroControl.transform.parent;

            GameObject go = Instantiate(heroInfo.HeroPrefab, parentRoot);
            go.tag = teamTag;
            go.transform.position = battlePos.position;

            var spawnedHeroControl = go.GetComponent<HeroControl>();
            if (spawnedHeroControl != null)
            {
                spawnedHeroControl.IsSpawn = true;
                spawnedHeroControl.SetBattleTarget(battlePos.position);
                spawnedHeroControl.SetArrivedBattle();

                ApplySummonStats(spawnedHeroControl, randomHeroId);
            }

            BattlefieldRegistry.Instance.Register(go.transform, slot, teamTag);
        }
        StartCoroutine(CoFinishAfterDelay(2f));
    }

    private static void ApplySummonStats(HeroControl spawned, int heroId)
    {
        if (spawned == null)
            return;

        var runtime = spawned.GetComponent<HeroStatRuntime>();
        var recv = spawned.GetComponentInChildren<HeroReceiveDamagee>();

        int health;
        int damage;
        int armor;

        switch (heroId)
        {
            case 500:
                health = 500; damage = 50; armor = 50;
                break;
            case 501:
                health = 350; damage = 75; armor = 25;
                break;
            case 502:
                health = 800; damage = 35; armor = 70;
                break;
            default:
                return;
        }

        runtime.FinalStat.health = health;
        runtime.FinalStat.damage = damage;
        runtime.FinalStat.armor = armor;
        runtime.CurrentHealth = health;
    }
    private IEnumerator CoFinishAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetFinished();
    }

    public void CallSlideToPosition(int index)
    {
        if (heroControl == null)
            return;

        if (loadSlideToPosition == null || loadSlideToPosition.listPosition == null)
            return;

        var cfg = loadSlideToPosition.listPosition.Find(p => p != null && p.index == index);
        if (cfg == null)
            return;

        Vector3 from = heroControl.transform.position;

        // If facing left (scale.x < 0) => invert X offset
        Vector3 offset = cfg.positionSpawn;
        if (heroControl.transform.localScale.x < 0f)
            offset.x = -offset.x;

        Vector3 to = from + offset;

        // speed = units/second
        StartCoroutine(CoSlideToPosition(to, cfg.speed));
    }

    private IEnumerator CoSlideToPosition(Vector3 targetPos, float speed)
    {
        if (speed <= 0f)
        {
            heroControl.transform.position = targetPos;
            yield break;
        }

        while ((heroControl.transform.position - targetPos).sqrMagnitude > 0.0001f)
        {
            heroControl.transform.position = Vector3.MoveTowards(
                heroControl.transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            yield return null;
        }

        heroControl.transform.position = targetPos;
    }

    public void CallTeleportToPosition(int index)
    {
        if (heroControl == null)
            return;

        if (loadSlideToPosition == null || loadSlideToPosition.listPosition == null)
            return;

        var cfg = loadSlideToPosition.listPosition.Find(p => p != null && p.index == index);
        if (cfg == null)
            return;

        Vector3 offset = cfg.positionSpawn;
        if (heroControl.transform.localScale.x < 0f)
            offset.x = -offset.x;

        heroControl.transform.position += offset;
    }
    public void CallChangeScale(float coefficient) //truyền vào hệ số thay đổi scale
    {
        float newScaleX = heroControl.transform.localScale.x * coefficient;
        float newScaleY = heroControl.transform.localScale.y * coefficient;
        heroControl.transform.localScale = new Vector3(newScaleX, newScaleY, heroControl.transform.localScale.z);
    }
}
