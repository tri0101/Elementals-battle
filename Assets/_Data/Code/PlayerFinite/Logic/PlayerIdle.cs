using UnityEngine;

public class PlayerIdle : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }


    public void AutoFlip()
    {
        
        if (playerControl.MoveX != 0) return;
        float baseScaleX = Mathf.Abs(transform.localScale.x);
        if (transform.localPosition.x > playerControl.Enemy.transform.localPosition.x)
        {
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }
        else
        {

            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}
