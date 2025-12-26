using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq.Expressions;
public class ObjectFlyingAttack : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    [SerializeField] private bool hasAttacked = false;

    
    private void Start()
    {
        objController = GetComponent<ObjectFlyingController>();
    }
    private void Update()
    {
        if(transform.name == "Air_Arrow")
        {
            if (objController.CheckingObjectFlying.IsTouchGround && !hasAttacked)
            {
                hasAttacked = true;
                StartCoroutine(DelayAttack(objController.ObjectFlyingSO.delayAttack));
            }
        }
        else
        {
            if (objController.CheckingObjectFlying.IsTouchPlayer && !hasAttacked)
            {
                hasAttacked = true;
                StartCoroutine(DelayAttack(objController.ObjectFlyingSO.delayAttack));
            }
        }
        
    }

    IEnumerator DelayAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        Attack();
    }

    void Attack()
    {
        if (transform.name != "Arrow")
        {
            objController.Animator.SetTrigger("attack");
        }
   
        objController.ObjectFlying.CanFly = false;
        if (objController.ObjectFlying.HasBeenToPool) return;
        ObjectFlyingSpawnPoint.instance.AddToPoolTimer(gameObject, ObjController.ObjectFlyingSO.timeToDespawnAttack);
        objController.ObjectFlying.HasBeenToPool = true;
        
    }


    private void OnDisable()
    {
        hasAttacked = false;
    }

}
