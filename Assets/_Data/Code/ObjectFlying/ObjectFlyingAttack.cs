using UnityEngine;
using System.Collections;
using TMPro;
public class ObjectFlyingAttack : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    private bool hasAttacked = false;

    
    private void Start()
    {
        objController = GetComponent<ObjectFlyingController>();
    }
    private void Update()
    {
        if (objController.CheckingObjectFlying.IsTouchPlayer && !hasAttacked)
        {
            hasAttacked = true;
            StartCoroutine(DelayAttack(objController.ObjectFlyingSO.delayAttack));
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
        
        ObjectFlyingSpawnPoint.instance.AddToPoolTimer(gameObject, ObjController.ObjectFlyingSO.timeToDespawnAttack);
        
    }


    private void OnDisable()
    {
        hasAttacked = false;
    }

}
