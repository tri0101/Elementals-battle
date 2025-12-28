using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
public class PlayerJump : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;
    [SerializeField] float jumpForce;

    [SerializeField] bool canMoveWhenJump = true;
    
    [SerializeField] float airWalkSpeed
    {
        get
        {
            if (canMoveWhenJump)
            {
                if (!playerControl.PlayerCheckingGround.IsOnWall )
                {
                    return playerControl.PlayerInfo.airWalkSpeed;
                }
                else
                {
                    return 0;
                }
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
    //public void MyFixedUpdate(float moveX)
    //{
    //    playerControl.Rb.linearVelocity = new Vector2(moveX * airWalkSpeed, playerControl.Rb.linearVelocity.y);
    //}

    private void Update()
    {
        if (playerControl.PlayerCheckingGround.IsXJumpPlayer)
        {
            canMoveWhenJump = false;
            StartCoroutine(ResetCanMove());
        }
    }
    public void MyFixedUpdate(float moveX)
    {
        
        Vector2 currentVel = playerControl.Rb.linearVelocity;
        currentVel.x = moveX * airWalkSpeed;
        playerControl.Rb.linearVelocity = currentVel;
    }
    public void CheckPlayer()
    {
        if (playerControl.PlayerCheckingGround.IsOnPlayer && playerControl.Rb.linearVelocity.y < 0)
        {
            // Tạo hiệu ứng trượt xuống người khác
            Vector2 vel = playerControl.Rb.linearVelocity;
            vel.y = -50f; // tốc độ rơi trượt
            
            playerControl.Rb.linearVelocity = vel;
        }
        
    }
    private IEnumerator ResetCanMove()
    {
        
        yield return new WaitForSeconds(0.2f);
        canMoveWhenJump = true;
    }
}
