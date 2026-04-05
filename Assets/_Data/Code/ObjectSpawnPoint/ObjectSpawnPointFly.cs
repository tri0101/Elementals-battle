using System.Collections;
using UnityEngine;

public class ObjectSpawnPointFly : MonoBehaviour
{
    [SerializeField] ObjectSpawnPointController objectSpawnPointController;

    public ObjectSpawnPointController ObjectSpawnPointController => objectSpawnPointController;

    private int _dir = 1;

    [SerializeField] private float flySpeed = 5f;

    private Coroutine _flyRoutine;

    public void Awake()
    {
        objectSpawnPointController = GetComponent<ObjectSpawnPointController>();
    }

    public void SetDirectionFromUnitScale(float unitScaleX)
    {
        
        _dir = unitScaleX > 0f ? 1 : -1;
    }

    public void Fly()
    {
        if (objectSpawnPointController == null || objectSpawnPointController.ObjectSpawnPointSO == null)
            return;

        if (_flyRoutine != null)
            StopCoroutine(_flyRoutine);

        _flyRoutine = StartCoroutine(FlyRoutine(objectSpawnPointController.ObjectSpawnPointSO.meterFly));
    }

    private IEnumerator FlyRoutine(float meterFly)
    {
        Vector3 startPos = transform.position;
        float meterFlyAbs = Mathf.Abs(meterFly);

        while (true)
        {
            transform.position += Vector3.right * (_dir * flySpeed * Time.deltaTime);

            float traveled = Vector3.Distance(startPos, transform.position);
            if (traveled >= meterFlyAbs)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }
}