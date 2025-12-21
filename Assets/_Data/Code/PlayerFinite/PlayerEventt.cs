using UnityEngine;
using System.Collections;

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

        if (transform.parent.localScale.x > 0)
        {
            return startPos + new Vector3(targetMove.x, 0, 0);
        }
        else
        {
            return startPos + new Vector3(-targetMove.x, 0, 0);
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
    
}
