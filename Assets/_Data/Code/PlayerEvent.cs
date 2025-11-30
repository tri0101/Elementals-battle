using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerEvent : MonoBehaviour
{
    public LoadNormalAttack lnA;
    PlayerController pc;
    EnemyController ec;
  

    private void Awake()
    {
        if(transform.parent.tag == "Player1" || transform.parent.tag == "Player2")
        {
            pc = transform.parent.GetComponent<PlayerController>();
        }
        else
        {
            ec = transform.parent.GetComponent<EnemyController>();  
        }
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
    //dùng để gọi ở script khác
    public void CallSlideToPosition(Vector3 startPos, Vector3 targetMove, float duration)
    {
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
    private IEnumerator SlideToPosition(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector2 newPos = Vector2.Lerp(startPos, endPos, t);

            pc.Rb.MovePosition(newPos);  

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); 
        }

        pc.Rb.MovePosition(endPos); 
    }


    public void CallSlideToPositionBySpeed(Vector3 targetMove, float speed)
    {
        // Lấy startPos hiện tại
        Vector3 startPos = transform.parent.localPosition;

        // Tính endPos dựa vào hướng scale
        Vector3 endPos;
        if (transform.parent.localScale.x > 0)
            endPos = startPos + new Vector3(targetMove.x, 0, 0);
        else
            endPos = startPos + new Vector3(-targetMove.x, 0, 0);

        StartCoroutine(SlideToPositionFixedSpeed(endPos, speed));
    }
    private IEnumerator SlideToPositionFixedSpeed(Vector3 endPos, float speed)
    {
        while ((pc.Rb.transform.localPosition - endPos).sqrMagnitude > 0.001f)
        {
            Vector2 nextPos = Vector2.MoveTowards(
                pc.Rb.transform.localPosition,
                endPos,
                speed * Time.fixedDeltaTime // PHẢI DÙNG fixedDeltaTime
            );

            pc.Rb.MovePosition(nextPos);

            yield return new WaitForFixedUpdate(); // PHẢI CHỜ FRAME PHYSICS
        }

        pc.Rb.transform.localPosition = endPos;
    }

    public void SetBoolTransform()
    {

        if(transform.parent.tag == "Player1" || transform.parent.tag == "Player2")
        {
            pc.HasBeenTransformed = true;
        }
        else
        {
            ec.HasBeenTransformed = true;
        }
        
    }
    public void SetFalseTransform()
    {
        if (transform.parent.tag == "Player1" || transform.parent.tag == "Player2")
        {
            pc.HasBeenTransformed = false;
        }
        else
        {
            ec.HasBeenTransformed = false;
        }
    }
    public void DisableNormal()
    {
        if(transform.parent.tag == "Player1" || transform.parent.tag == "Player2")
        {
            pc.NormalT.gameObject.SetActive(false);
        }
        else
        {
            ec.NormalT.gameObject.SetActive(false);
        }
    }

    public void EnableNormal()
    {
        if(transform.parent.tag == "Player1" || transform.parent.tag == "Player2")
        {
            pc.NormalT.gameObject.SetActive(true);
        }
        else
        {
            ec.NormalT.gameObject.SetActive(true);
        }
    }
 
    public void SetFalseCurrentlyTransform()
    {
        if(transform.parent.tag == "Player1" || transform.parent.tag == "Player2")
        {
            pc.IsCurrentlyTransforming = false;
        }
        else
        {
            ec.IsCurrentlyTransforming = false;
        }

    }
   public void IsStopAnim()
    {
        if (pc.PlayerReceiveDamage.IsStopAnim)
        {
            StartCoroutine(PauseAnimCoroutine(pc.PlayerReceiveDamage.DurationFinalAttack));
        }
    }

    private IEnumerator PauseAnimCoroutine(float duration)
    {
        pc.Animator.speed = 0f;                 // Dừng animation
        yield return new WaitForSeconds(duration); // Đợi X giây
        pc.Animator.speed = 1f;                 // Chạy lại bình thường
    }
    public void ResetIsFinalAttack()
    {
        if (pc.PlayerReceiveDamage.IsFinalAttack)
        {
            pc.PlayerReceiveDamage.ResetDurationAttack();
        }
        
    }
}
