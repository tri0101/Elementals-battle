//                         using UnityEngine;

//public class heroInput : MonoBehaviour
//{

//    heroControl heroControl;

//    private void Awake()
//    {
//        heroControl = GetComponent<heroControl>();
//    }
//    public bool isLeftMove;
//    public bool isRightMove;
//    public bool isLeftMovePC;
//    public bool isRightMovePC;
//    public bool isAttackInput;
//    public bool isRollInput;
//    public bool isJumpInput;
//    public bool isSkillInput;
//    public bool isSkillOneInput;
//    public bool isTransformInput;
//    public bool isRangedAttackInput;
//    public bool isBlockInputDown;
//    public bool isBlockInputUp;
//    public bool isBlockInputDownPC;
//    public bool isBlockInputUpPC;



//    private void Update()
//    {
//        isLeftMovePC = Input.GetKey(heroControl.KeyBiding.leftMove);
//        isRightMovePC  = Input.GetKey(heroControl.KeyBiding.rightMove);
        
//        if (Input.GetKeyDown(heroControl.KeyBiding.attackKey)) isAttackInput = true;
//        if (Input.GetKeyDown(heroControl.KeyBiding.jumpKey)) isJumpInput = true;
//        if (Input.GetKeyDown(heroControl.KeyBiding.blockKey)) isBlockInputDownPC = true;
//        if (Input.GetKeyUp(heroControl.KeyBiding.blockKey)) isBlockInputUpPC = true;
        
//        if ((Input.GetKeyDown(heroControl.KeyBiding.transformKey))) isTransformInput = true;
//        if (Input.GetKeyDown(heroControl.KeyBiding.rangedAttackKey)) isRangedAttackInput = true;
//        if (Input.GetKeyDown(heroControl.KeyBiding.skillKey)) isSkillInput = true;
//        if (Input.GetKeyDown(heroControl.KeyBiding.rangedAttackKey)) isRangedAttackInput = true;
//    }

//    public void OnAttackButtonDown()
//    {
//        isAttackInput = true;
//    }
//    public void OnJumpButtonDown()
//    {
//        isJumpInput = true;
//    }
//    public void OnRollButtonDown()
//    {
//        isRollInput = true;
//    }
//    public void OnSKillButtonDown()
//    {
//        isSkillInput = true;
//    }
//    public void OnSKillOneButtonDown()
//    {
//        isSkillOneInput = true;
//    }
//    public void OnTransformButtonDown()
//    {
//        isTransformInput = true;
//    }
//    public void OnMoveLeftDown()
//    {
//        isLeftMove = true;
//    }
//    public void OnMoveLeftUp()
//    {
//        isLeftMove = false;
//    }
//    public void OnMoveRightDown()
//    {
//        isRightMove = true;
//    }
//    public void OnMoveRightUp()
//    {
//        isRightMove = false;
//    }
//    public void OnRangedAttackButtonDown()
//    {
//        isRangedAttackInput = true;
//    }
//    public void OnBlockUp()
//    {
//        isBlockInputUp = true;
//    }
//    public void OnBlockDown()
//    {
//        isBlockInputDown = true;
//    }
//    void LateUpdate()
//    {
//        isAttackInput = false;
//        isRollInput = false;
//        isJumpInput = false;
//        isSkillInput = false;
//        isSkillOneInput = false;
//        isTransformInput = false;
//        isRangedAttackInput = false;
//        isBlockInputDown = false;
//        isBlockInputUp = false;
//        isBlockInputDownPC = false;
//        isBlockInputUpPC = false;
       
//    }
//}
