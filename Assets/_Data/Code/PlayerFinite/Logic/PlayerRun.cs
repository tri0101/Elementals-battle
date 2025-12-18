using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;
    [SerializeField] float CurrentSpeed;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }
    private void Start()
    {
        CurrentSpeed = playerControl.PlayerInfo.moveSpeed;
    }
    public void Move()
    {
        playerControl.Rb.linearVelocity = new Vector2(playerControl.MoveX * CurrentSpeed, playerControl.Rb.linearVelocity.y);

   

    }
    public void Flip()
    {
        float moveX = playerControl.MoveX;

        float baseScaleX = Mathf.Abs(transform.localScale.x);
        if (moveX > 0)
        {

            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }

        else if (moveX < 0)
        {

            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }



    }
}
