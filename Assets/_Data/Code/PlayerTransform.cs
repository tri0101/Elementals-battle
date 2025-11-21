using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] private float timer = 0f;
    private float timerTarget = 5f;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    private void Update()
    {

        TranformTo();
        if (pc.HasBeenTransformed)
        {
            if(timer < timerTarget)
            {
                timer += Time.deltaTime;
                if(timer >= timerTarget )
                {
                    BackToHuman();
                    
                }
            }
            
        }
    }
    void TranformTo()
    {

        if (pc.HasBeenTransformed || pc.IsCurrentlyTransforming)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {

            pc.IsCurrentlyTransforming = true;
            timer = 0f;
            pc.Animator.SetTrigger("transform");


            pc.HasBeenTransformed = true;
        }
    }
    void BackToHuman()
    {
        if (pc.IsCurrentlyTransforming) return;
        pc.IsCurrentlyTransforming = true;
        pc.Animator.SetTrigger("transformToHuman");
   
    }
}
