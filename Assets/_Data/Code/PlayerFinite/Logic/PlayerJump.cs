using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;
    [SerializeField] float jumpForce;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        jumpForce = playerControl.PlayerInfo.jumpForce;
    }
    public void Jump()
    {
        playerControl.Rb.linearVelocity = new Vector2(playerControl.Rb.linearVelocity.x, jumpForce);
        playerControl.Animator.SetFloat("yVelocity", playerControl.Rb.linearVelocity.y);
    }

    public void UpdateAnimatorY()
    {
        playerControl.Animator.SetFloat(
            "yVelocity",
            playerControl.Rb.linearVelocity.y
        );
    }
}
