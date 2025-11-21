using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player/Normal Attack")]
public class LoadNormalAttack : ScriptableObject
{
    public int numberOfAttack;
    public List<Vector3> Attacks = new List<Vector3>();
    
    public void AddAttack(Vector3 attack)
    {
        Attacks.Add(attack);
    }
}
