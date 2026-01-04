using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;

public class PlayerEventt : MonoBehaviour
{
    public LoadNormalAttack lnA;

    PlayerControl playerControl;

    private void Awake()
    {
        playerControl = transform.parent.GetComponent<PlayerControl>();
    }
    public void SetPositionAttack(int attackCount)
    {
        float duration = 0.2f;
        Vector3 targetMove = lnA.Attacks[attackCount];
        Vector3 startPos = transform.parent.localPosition;
        Vector3 endPos;

        if (transform.parent.localScale.x > 0)
        {
            endPos = startPos + new Vector3(targetMove.x, 0, 0);
        }
        else
        {
            endPos = startPos + new Vector3(-targetMove.x, 0, 0);
        }

        StartCoroutine(SlideToPosition(startPos, endPos, duration));

    }
    //==================================
    //Tính giá trị endpos dựa trên scale
    //==================================
    public Vector3 ReturnEndPos(Vector3 startPos, Vector3 targetMove)
    {
        float finalX = targetMove.x;
        if (playerControl.CurrentStringState == "Block" || playerControl.CurrentStringState == "T_Block")
        {
            finalX *= 0.5f;
        }
            if (transform.parent.localScale.x > 0)
        {
            
            return startPos + new Vector3(finalX, 0, 0);
        }
        else
        {
            return startPos + new Vector3(-finalX, 0, 0);
        }
    }



    //dùng để gọi ở script khác
    public void CallSlideToPosition(Vector3 startPos, Vector3 targetMove, float duration)
    {
        Vector3 endPos;
        endPos = ReturnEndPos(startPos, targetMove);

        StartCoroutine(SlideToPosition(startPos, endPos, duration));
    }
    private IEnumerator SlideToPosition(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector2 newPos = Vector2.Lerp(startPos, endPos, t);

            playerControl.Rb.MovePosition(newPos);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        playerControl.Rb.MovePosition(endPos);
    }





    //========Spawn ra ranged attack Object ===================
    public void SpawnRangedAttack(string nameObject)
    {
        string tag;
        if (transform.parent.tag == "Player1")
        {
            tag = "RangedAttackPlayer1";
        }
        else
        {
            tag = "RangedAttackPlayer2";
        }

        ObjectFlyingSpawnPoint.instance.SpawnObjectAtPosition(nameObject, transform, tag);
    }
    public void SpawnObject(string nameObject)
    {
        string tag;
        if (transform.parent.tag == "Player1")
        {
            tag = "RangedAttackPlayer1";
        }
        else
        {
            tag = "RangedAttackPlayer2";
        }

        ObjectSpawnPoint.instance.SpawnObjectAtPosition(nameObject, transform, tag);
    }


    //======Stop Anim khi bi tan cong=============
    public void IsStopAnim()
    {


         StartCoroutine(PauseAnimCoroutine(playerControl.PlayerReceiveDamagee.DurationFinalAttack));
        
    }
    public void CallStopAnimFinalAttack()
    {
        StartCoroutine(PauseAnimCoroutine(0.3f));
    }
    private IEnumerator PauseAnimCoroutine(float duration)
    {
        playerControl.Animator.speed = 0f;
        yield return new WaitForSeconds(duration);
        
        playerControl.Animator.speed = 1f;
       

    }
}
