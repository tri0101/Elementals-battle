using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : Subject
{
    [SerializeField] private AttackInfo attackInfo;
    public AttackInfo AttackInfo => attackInfo;
    [SerializeField] private HeroControl heroControl;
    [SerializeField] private float attackDamage;
    public float AttackDamage => attackDamage;

    [SerializeField] string myTag;
    
    private void Awake()
    {

       
        heroControl = transform.parent.parent.parent.parent.GetComponent<HeroControl>();
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("ReceiveField"))
        {
            Debug.Log(other.tag);
            return;
        }
        attackDamage = attackInfo.mutiplerDamageSend * heroControl.HeroStatRuntime.Damage;
        Debug.Log("Attack Damage: " + attackDamage);
        var hero = other.GetComponent<HeroReceiveDamagee>();
        if (hero == null)
        {
            Debug.LogError("HeroReceiveDamagee component not found on the collided object.");
            return;
        }


        if (!heroControl.enemyTarget.Contains(hero.transform.parent.parent))
        {
            Debug.Log("Not enemy target");
            return;
        }
        hero.ReceiveDamage(attackDamage);
        NotifyObservers(this);
    }

}
