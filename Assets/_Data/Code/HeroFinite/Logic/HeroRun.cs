using UnityEngine;

public class HeroRun : MonoBehaviour
{
    HeroControl heroControl;
    public HeroControl heroControlhero => heroControl;
    [SerializeField] float CurrentSpeed = 5f;

    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }
 
    public void Move()
    {
        Vector3 currentPos = transform.position;

        Vector3 targetPos = currentPos + new Vector3(
            heroControl.MoveX,
            0f,
            0f
        );

        transform.position = Vector3.MoveTowards(
            currentPos,
            targetPos,
            CurrentSpeed * Time.deltaTime
        );
    }
    public void MoveTo(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            CurrentSpeed * Time.deltaTime
        );
    }

}
