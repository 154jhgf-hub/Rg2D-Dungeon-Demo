using System;
using System.Collections.Generic;

[Serializable]
public class InventoryData 
{
    public List<Card> cards = new List<Card>();
    public int capacity = 20;
    public InventoryData(int amount)
    {
        capacity = amount;
        Init();
    }

    private void Init()
    {
        cards.Clear();
        for(int i = 0; i < capacity; i++)
        {
            cards.Add(new Card());
            
        }
    }
    public bool AddItem(Item item)
    {
        Normalize(capacity);
        if (item == null || item.currentAmount <= 0)
        {
            return false;
        }

        foreach (Card card in cards)
        {
            if(card.bindItem!=null&&card.bindItem.id==item.id&&
                card.bindItem.currentAmount < card.bindItem.maxStack)
            {
                int canAdd = card.bindItem.maxStack - card.bindItem.currentAmount;
                if (item.currentAmount <= canAdd)
                {
                    card.bindItem.currentAmount += item.currentAmount;
                    return true;
                }
                else
                {
                    card.bindItem.currentAmount = card.bindItem.maxStack;
                    item.currentAmount -= canAdd;
                }
            }
        }

        foreach(Card card1 in cards)
        {
            if (card1.bindItem == null)
            {
                card1.bindItem = item;
                return true;
            }
        }

        return false;
    }

    public void Normalize(int targetCapacity)
    {
        capacity = targetCapacity;
        if (cards == null)
        {
            cards = new List<Card>();
        }

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i] == null)
            {
                cards[i] = new Card();
                continue;
            }

            Item item = cards[i].bindItem;
            if (item == null)
            {
                continue;
            }

            if (item.currentAmount <= 0)
            {
                cards[i].bindItem = null;
                continue;
            }

            if (item.maxStack <= 0)
            {
                item.maxStack = 1;
            }
        }

        while (cards.Count < capacity)
        {
            cards.Add(new Card());
        }

        if (cards.Count > capacity)
        {
            cards.RemoveRange(capacity, cards.Count - capacity);
        }
    }

    public void RemoveItem(int index,int amount=1)
    {
        if (index < 0 || index >= cards.Count)
        {
            return;
        }
        Card card = cards[index];
        if (card.bindItem != null)
        {
            if (amount <= card.bindItem.currentAmount)
            {
                card.bindItem.currentAmount -= amount;
                if (card.bindItem.currentAmount <= 0)
                {
                    card.bindItem = null;
                }
            }
        }
    }

    public void SwapItem(int index1,int index2)
    {
        if (index1 < 0 || index1 >= cards.Count)
        {
            return;
        }
        if (index2 < 0 || index2 >= cards.Count)
        {
            return;
        }

        Item temp = cards[index1].bindItem;
        if (temp != null)
        {
            cards[index1].bindItem = cards[index2].bindItem;
            cards[index2].bindItem = temp;
        }
        else
        {
            return;
        }
    }
}
