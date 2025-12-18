using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;
    [SerializeField] float jumpForce;
    [SerializeField] float airWalkSpeed
    {
        get
        {
            if (!playerControl.PlayerCheckingGround.IsOnWall)
            {
                return playerControl.PlayerInfo.airWalkSpeed;
            }
            else
            {
                return 0;
            }
        }
    }
    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        jumpForce = playerControl.PlayerInfo.jumpForce;
      
    }
    public void Jump()
    {
        playerControl.Rb.linearVelocity = new Vector2(playerControl.Rb.linearVelocity.x, jumpForce);

    }

    public bool CheckVelociyY()
    {
        return (playerControl.Rb.linearVelocityY < 0);
 
        
    }

    public void SetGravity(float amount )
    {
        playerControl.Rb.gravityScale = amount;
    }
    public void MyFixedUpdate()
    {
        playerControl.Rb.linearVelocity = new Vector2(playerControl.MoveX * airWalkSpeed, playerControl.Rb.linearVelocity.y);
    }
}
