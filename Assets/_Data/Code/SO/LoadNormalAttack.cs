using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class TypeAndVector
{
    public int indexSpawn;
    public AbilityEffectType type;
    public Vector3 positionSpawn; // vị trí tương đối với hero spawn ra nó
}
[CreateAssetMenu(menuName = "hero/Normal Attack")]
public class LoadNormalAttack : ScriptableObject
{
   
    public List<TypeAndVector> dicSpawn;
   

    
   
}
