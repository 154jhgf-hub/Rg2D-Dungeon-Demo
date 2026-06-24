using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { private set; get; }
    public List<GameObject> itemList=new List<GameObject>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CreatItem(Vector2 pos)
    {
        int index = Random.Range(0, itemList.Count);
        float dropchance = Random.Range(0, 1f);
        GameObject obj = itemList[index];
        DropItem dropItem = obj.GetComponent<DropItem>();
        if (dropItem != null)
        {
            if (dropchance < dropItem.dropChance)
            {
               GameObject game=Instantiate<GameObject>(obj,pos,Quaternion.identity);
                game.name = obj.name;
            }
        }
    }
}
