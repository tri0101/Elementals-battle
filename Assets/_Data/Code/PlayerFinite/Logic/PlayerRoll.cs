using UnityEngine;
using System.Collections;
public class PlayerRoll : MonoBehaviour
{
    private float rollPower;
    private float rollDuration;
    private float rollCooldown = 0.5f;
    private Vector2 rollDirection;
   
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        rollPower = playerControl.PlayerInfo.rollPower;
        rollDuration = playerControl.PlayerInfo.rollDuration;
    }
    public void StartRoll()
    {
        
        StartCoroutine(Roll());
    }

    private IEnumerator Roll()
    {
     
        float x = 0f;
        float y = 0f;
        if (transform.tag == "Player1")
        {
            if (Input.GetKey(KeyCode.A)) x = -1;
            if (Input.GetKey(KeyCode.D)) x = 1;
        }

        else
        {
            if (Input.GetKey(KeyCode.LeftArrow)) x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) x = 1;
        }


        if (x == 0 && y == 0)
            x = transform.localScale.x > 0 ? 1 : -1;

        rollDirection = new Vector2(x, y).normalized;

        float start = Time.time;


        while (Time.time < start + rollDuration)
        {
            playerControl.Rb.MovePosition(playerControl.Rb.position + rollDirection * rollPower * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }


        playerControl.Rb.linearVelocity = new Vector2(playerControl.Rb.linearVelocity.x, 0f);

        
        yield return new WaitForSeconds(0);
    }


}
