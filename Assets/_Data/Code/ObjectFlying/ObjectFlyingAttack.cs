using UnityEngine;
using System.Collections;
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
        if (objController.CheckingObjectFlying.IsOnWall && !hasAttacked)
        {
            hasAttacked = true;
            StartCoroutine(DelayAttack(0.05f));
        }
    }

    IEnumerator DelayAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        Attack();
    }

    void Attack()
    {
        objController.Animator.SetTrigger("attack");
        objController.ObjectFlying.CanFly = false;
    }

}
