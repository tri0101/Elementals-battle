using UnityEngine;

public class PlayerTransformm : MonoBehaviour
{
    PlayerControl playerControl;
    public PlayerControl PlayerControlPlayer => playerControl;

    float timer;
    float timerTarget = 7f;


    private bool transformToHuman;
    public bool TransformToHuman
    {
        get => transformToHuman;
        set => transformToHuman = value;
    }
    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        

    }
    private void Update()
    {
        if (!playerControl.HasBeenTransform) return;
        if (timer < timerTarget)
        {
            timer += Time.deltaTime;
            if (timer >= timerTarget)
            {
                timer = 0f;
                TransformToHuman = true;

            }
        }
    }

}
