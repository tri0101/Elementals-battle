using UnityEngine;

public class PlayerSkillOne : MonoBehaviour
{
    PlayerControl playerControl;
    public bool canMove = true;
    public PlayerControl PlayerControlPlayer => playerControl;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }


    public void ResetMana()
    {
        playerControl.PlayerReceiveDamagee.Mana -= 500f;
        playerControl.RefreshObservers();
    }
    public void Move()
    {
       
        playerControl.Rb.linearVelocity = new Vector2(playerControl.MoveX * 10f, playerControl.Rb.linearVelocity.y);



    }
}
