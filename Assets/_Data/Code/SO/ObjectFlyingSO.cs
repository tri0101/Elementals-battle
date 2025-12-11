using UnityEngine;

[CreateAssetMenu(menuName = "Object/ObjectFlyingInfo")]
public class ObjectFlyingSO : ScriptableObject
{
    public float flySpeed;
    public float delayAttack;
    public float timeToDespawn;
    public float timeToDespawnAttack;
    public Vector3 spawnPosition;
}
