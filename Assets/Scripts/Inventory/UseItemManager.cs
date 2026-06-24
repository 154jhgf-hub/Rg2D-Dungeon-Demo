using UnityEngine;

public class UseItemManager : MonoBehaviour
{
    public static UseItemManager Instance;
    public GameObject player;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    


    public void UseItem(Item item,int cardIndex)
    {
        if (player == null)
        {
            ToastUI.Show("当前无法使用");
            return;
        }
        if (item == null || item.currentAmount <= 0)
        {
            ToastUI.Show("当前无法使用");
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Consumable:
                if (!UseConsumable(item.consumableType))
                {
                    return;
                }
                break;
            case ItemType.Weapon:
                ToastUI.Show("武器暂未支持使用");
                return;
        }
        InventoryManager.Instance.RemoveItem(cardIndex);
        //InventoryUI.Instance.cardUIs[cardIndex].UpdateCard(item);
    }

    private bool UseConsumable(ConsumableType item)
    {
        switch(item)
        {
            case ConsumableType.Coin:
                CoinManager.Instance.AddCoin(1);
                ToastUI.Show("获得金币：+1");
                return true;
            case ConsumableType.Health:
                Health health = player.GetComponent<Health>();
                if (health.currentHp >= health.maxHp)
                {
                    ToastUI.Show("生命值已满");
                    return false;
                }
                health.AddHp(20f);
                ToastUI.Show("使用生命药水 HP +20");
                return true;
            case ConsumableType.Mana:
                LevelUp levelUp = player.GetComponent<LevelUp>();
                levelUp.AddExp(20);
                ToastUI.Show("获得经验 EXP +20");
                return true;
        }

        ToastUI.Show("当前无法使用");
        return false;
    }
}
