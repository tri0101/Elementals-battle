using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpeedAndVector
{
    public int index;
    public float speed; // tốc độ lướt
    public Vector3 positionSpawn; // vị trí tương đối với hero spawn ra nó
}

[CreateAssetMenu(menuName = "hero/SlideToPosiion")]
public class LoadSlideToPosition : ScriptableObject
{

    public List<SpeedAndVector> listPosition; // lướt bao xa



}
