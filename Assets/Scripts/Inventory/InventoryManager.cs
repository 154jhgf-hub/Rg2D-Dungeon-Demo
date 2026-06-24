using System;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { private set; get; }

    public InventoryData inventoryData;
    public Action OnInventoryChange;
    public int bgCapacity=20;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //LoadInventory();
        }
        else
        {
            Debug.Log("�?���");
        }

        
    }
   

    public void NewDate()
    {
        inventoryData = new InventoryData(bgCapacity);
    }

    public InventoryData GetInventory()
    {
        EnsureInventory();
        return inventoryData;
    }
    public bool AddItem(Item item)
    {
        EnsureInventory();
        bool success = inventoryData.AddItem(item);
        if (success)
        {
            OnInventoryChange?.Invoke();
            //SaveInventory(inventoryData);
        }

        return success;
    }

    public void RemoveItem(int index,int amount=1)
    {
        EnsureInventory();
        inventoryData.RemoveItem(index, amount);
        OnInventoryChange?.Invoke();
        //SaveInventory(inventoryData);
    }

    public void SwapItem(int start,int end)
    {
        EnsureInventory();
        inventoryData.SwapItem(start, end);
        OnInventoryChange?.Invoke();
        //SaveInventory(inventoryData);
    }

    public void SaveInventory(InventoryData inventoryData)
    {
        if (inventoryData == null)
        {
            inventoryData = new InventoryData(bgCapacity);
            this.inventoryData = inventoryData;
        }

        inventoryData.Normalize(bgCapacity);
        string data = JsonUtility.ToJson(inventoryData);
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        File.WriteAllText(path, data);
    }

    public void LoadInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            inventoryData = JsonUtility.FromJson<InventoryData>(data);
            if (inventoryData == null)
            {
                inventoryData = new InventoryData(bgCapacity);
            }
            inventoryData.Normalize(bgCapacity);
        }
        else
        {
            inventoryData = new InventoryData(bgCapacity);
        }
    }

    private void EnsureInventory()
    {
        if (inventoryData == null)
        {
            inventoryData = new InventoryData(bgCapacity);
        }
        else
        {
            inventoryData.Normalize(bgCapacity);
        }
    }
}
