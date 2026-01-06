                         using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    PlayerControl playerControl;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }
    public bool isLeftMove;
    public bool isRightMove;
    public bool isLeftMovePC;
    public bool isRightMovePC;
    public bool isAttackInput;
    public bool isRollInput;
    public bool isJumpInput;
    public bool isSkillInput;
    public bool isSkillOneInput;
    public bool isTransformInput;
    public bool isRangedAttackInput;
    public bool isBlockInputDown;
    public bool isBlockInputUp;
    public bool isBlockInputDownPC;
    public bool isBlockInputUpPC;



    private void Update()
    {
        isLeftMovePC = Input.GetKey(playerControl.KeyBiding.leftMove);
        isRightMovePC  = Input.GetKey(playerControl.KeyBiding.rightMove);
        
        if (Input.GetKeyDown(playerControl.KeyBiding.attackKey)) isAttackInput = true;
        if (Input.GetKeyDown(playerControl.KeyBiding.jumpKey)) isJumpInput = true;
        if (Input.GetKeyDown(playerControl.KeyBiding.blockKey)) isBlockInputDownPC = true;
        if (Input.GetKeyUp(playerControl.KeyBiding.blockKey)) isBlockInputUpPC = true;
        
        if ((Input.GetKeyDown(playerControl.KeyBiding.transformKey))) isTransformInput = true;
        if (Input.GetKeyDown(playerControl.KeyBiding.rangedAttackKey)) isRangedAttackInput = true;
        if (Input.GetKeyDown(playerControl.KeyBiding.skillKey)) isSkillInput = true;
        if (Input.GetKeyDown(playerControl.KeyBiding.rangedAttackKey)) isRangedAttackInput = true;
    }

    public void OnAttackButtonDown()
    {
        isAttackInput = true;
    }
    public void OnJumpButtonDown()
    {
        isJumpInput = true;
    }
    public void OnRollButtonDown()
    {
        isRollInput = true;
    }
    public void OnSKillButtonDown()
    {
        isSkillInput = true;
    }
    public void OnSKillOneButtonDown()
    {
        isSkillOneInput = true;
    }
    public void OnTransformButtonDown()
    {
        isTransformInput = true;
    }
    public void OnMoveLeftDown()
    {
        isLeftMove = true;
    }
    public void OnMoveLeftUp()
    {
        isLeftMove = false;
    }
    public void OnMoveRightDown()
    {
        isRightMove = true;
    }
    public void OnMoveRightUp()
    {
        isRightMove = false;
    }
    public void OnRangedAttackButtonDown()
    {
        isRangedAttackInput = true;
    }
    public void OnBlockUp()
    {
        isBlockInputUp = true;
    }
    public void OnBlockDown()
    {
        isBlockInputDown = true;
    }
    void LateUpdate()
    {
        isAttackInput = false;
        isRollInput = false;
        isJumpInput = false;
        isSkillInput = false;
        isSkillOneInput = false;
        isTransformInput = false;
        isRangedAttackInput = false;
        isBlockInputDown = false;
        isBlockInputUp = false;
        isBlockInputDownPC = false;
        isBlockInputUpPC = false;
       
    }
}
