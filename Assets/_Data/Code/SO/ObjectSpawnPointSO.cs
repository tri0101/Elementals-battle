using UnityEngine;

[CreateAssetMenu(menuName = "Object/ObjectSpawnPointInfo")]
public class ObjectSpawnPointSO : ScriptableObject
{
    public float timeToDespawn;
    public Vector3 spawnPosition;
}
