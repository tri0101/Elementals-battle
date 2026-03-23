using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class HeroRun : MonoBehaviour
{
    HeroControl heroControl;
    float CurrentSpeed = 100f;

    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }

    public void MoveTo(Vector3 targetPos, float speed)
    {
        HandleFlipWhileMoving(targetPos);

        Vector3 cur = transform.position;
        Vector3 next = cur;
        CurrentSpeed = speed;
        float step = CurrentSpeed * Time.deltaTime;

        float yDiff = targetPos.y - cur.y;

        // ---- PHASE 1: y chưa bằng, đi xéo nhưng X ngắn ----
        if (Mathf.Abs(yDiff) > 0.01f)
        {
            // Y đi nhanh
            next.y = Mathf.MoveTowards(
                cur.y,
                targetPos.y,
                step
            );

            // X đi chậm (ví dụ 30% speed)
            next.x = Mathf.MoveTowards(
                cur.x,
                targetPos.x,
                step * 1f
            );
        }
        // ---- PHASE 2: y đã bằng, X đi full ----
        else
        {
            next.y = targetPos.y; // khóa lane

            next.x = Mathf.MoveTowards(
                cur.x,
                targetPos.x,
                step
            );
        }
        next.z = next.y;
        transform.position = next;
    }

    void HandleFlipWhileMoving(Vector3 targetPos)
    {
        float dirX = targetPos.x - transform.position.x;
        if (Mathf.Abs(dirX) < 0.001f) return;
        Vector3 scale = transform.localScale;
        if (dirX > 0 && scale.x < 0)
            scale.x *= -1;
        else if (dirX < 0 && scale.x > 0)
            scale.x *= -1;

        transform.localScale = scale;
    }

  
    public void FaceDefaultDirection()
    {
      
        Vector3 scale = transform.localScale;

        if (heroControl.CompareTag("Hero"))
        {
            
            scale.x = Mathf.Abs(scale.x);
        }
        else if (heroControl.CompareTag("Enemy"))
        {
           
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    public void SetZ() // dùng để hiển thị nhân vật tấn cong phía trước nhân vật khác, tránh bị che khuất
    {
        Vector3 position = transform.position;
        position.z -= 0.01f;
        transform.position = position;
    }
}
