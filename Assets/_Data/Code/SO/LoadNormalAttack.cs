using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "hero/Normal Attack")]
[System.Serializable]   
public class TypeAndVector
{
    public int indexSpawn;
    public AbilityEffectType type;
    public Vector3 positionSpawn; // vị trí tương đối với hero spawn ra nó
}
public class LoadNormalAttack : ScriptableObject
{
    public int numberOfAttack;
    public List<TypeAndVector> dicSpawn;
   

    
   
}
