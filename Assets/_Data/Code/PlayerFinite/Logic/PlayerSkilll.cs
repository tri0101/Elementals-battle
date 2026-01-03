using UnityEngine;

public class PlayerSkilll : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;
    
    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }


    public void ResetMana()
    {
        playerControl.PlayerReceiveDamagee.Mana -= 500f ;
        playerControl.RefreshObservers();
    }
}
