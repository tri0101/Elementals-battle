using UnityEngine;
using System.Collections;
public class PlayerEvent : MonoBehaviour
{
    public LoadNormalAttack lnA;
    PlayerController pc;
    EnemyController ec;
    private void Awake()
    {
        if(transform.parent.tag == "Player")
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
    private IEnumerator SlideToPosition(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.parent.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.parent.localPosition = endPos;
    }
    public void SetBoolTransform()
    {

        if(transform.parent.tag == "Player")
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
        if (transform.parent.tag == "Player")
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
        if(transform.parent.tag == "Player")
        {
            pc.NormalT.gameObject.SetActive(false);
        }
        else
        {
            ec.NormalT.gameObject.SetActive(false);
        }
    }
    //public void EnableTransform()
    //{
    //    pc.TransformT.gameObject.SetActive(true);
    //}
    public void EnableNormal()
    {
        if(transform.parent.tag == "Player")
        {
            pc.NormalT.gameObject.SetActive(true);
        }
        else
        {
            ec.NormalT.gameObject.SetActive(true);
        }
    }
    //public void DisableTransform()
    //{
    //    pc.TransformT.gameObject.SetActive(false);
    //}
    public void SetFalseCurrentlyTransform()
    {
        if(transform.parent.tag == "Player")
        {
            pc.IsCurrentlyTransforming = false;
        }
        else
        {
            ec.IsCurrentlyTransforming = false;
        }

    }
}
