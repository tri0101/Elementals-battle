using System.Collections.Generic;
using UnityEngine;
public enum GachaResultType
{
    Hero,
    Shard,
    Item
}

public struct GachaResult
{
    public int heroId;    // dùng khi type == Hero
    public int itemId;    // dùng khi type == Shard hoặc Item
    public int amount;    // số lượng item/shard (Hero thường = 1)
    public GachaResultType type;
}