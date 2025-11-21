using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target; 

    [Header("Follow settings")]
    public float smoothSpeed = 0.125f; 
    public Vector3 offset;             

    [Header("Optional limits")]
    public Vector2 minXMaxX = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
    public Vector2 minYMaxY = new Vector2(float.NegativeInfinity, float.PositiveInfinity);

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy object có tag 'Player'");
        }
    }
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

      
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minXMaxX.x, minXMaxX.y);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minYMaxY.x, minYMaxY.y);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

      
        transform.position = smoothedPosition;

    
         transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, -10f);
    }
}
