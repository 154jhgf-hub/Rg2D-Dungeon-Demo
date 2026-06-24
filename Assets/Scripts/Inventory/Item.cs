using System;



public enum ItemType
{
    Weapon,
    Consumable,
}

public enum ConsumableType
{
    Health,         // ÉúÃüÒ©Ë®
    Mana,
    Coin,
    None
}

[Serializable]
public class Item 
{
    public int id;
    public int maxStack = 10;
    public int currentAmount=1;
    public string itemName;
    public string iconpath;
    public ItemType itemType;
    public ConsumableType consumableType;
    public Item(int id, int maxStack,string name,string iconpath)
    {
        this.id = id;
        this.maxStack = maxStack;
        itemName = name;
        this.iconpath = iconpath;
        currentAmount = 1;
    }
}
