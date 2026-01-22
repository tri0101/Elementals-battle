using System.Collections.Generic;
using UnityEngine;
public enum GachaResultType
{
    Hero,
    Shard
}

public struct GachaResult
{
    public int heroId;
    public GachaResultType type;
}