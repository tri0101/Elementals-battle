using UnityEngine;

public class ObjectSpawnPointEvent : MonoBehaviour
{
    ObjectSpawnPointController ospc;
    private void Awake()
    {
        ospc = transform.parent.GetComponent<ObjectSpawnPointController>();
    }

    public void DespawnObject()
    {
        Destroy(transform.parent.gameObject);
    }

    public void DespawnObjectToPool()
    {
        ObjectSpawnPoint.instance.AddToPool(transform.parent.gameObject);
    }
}
