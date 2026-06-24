using System;



[Serializable]
public class Card 
{
    public Item bindItem;
    public void SetItem(Item item)
    {
        bindItem = item;
    }

    public bool HasItem()
    {
        if (bindItem == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
