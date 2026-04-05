using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
[CreateAssetMenu(menuName = "Object/ObjectSpawnPointSO")]
public class ObjectSpawnPointSO : ScriptableObject
{
    public bool canFly;
    public float meterFly; // bay bao xa
    public Vector3 positionSpawn; // vị trí spawn( tương đối với unit spawn ra nó )
}
