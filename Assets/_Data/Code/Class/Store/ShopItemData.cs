[System.Serializable]
public enum CurrencyType
{
    Gold,
    Diamond
}

[System.Serializable]
public class ShopItemData
{
    public int itemId;
    public int amount;
    public CurrencyType currencyType;
    public int price;
}