using Cinemachine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


[Serializable]
public class ObjectData
{
    public string name;
    public Vector3 pos;
    public Vector3 scale;
    public float currentHp;
    public bool hasLevelData;
    public int level;
    public int currentExp;
    public int expToNextLevel;
    public int baseMaxHp;
    public int baseAttack;
}

[Serializable]
public class Wrapper
{
    public List<ObjectData> objects;
}

[Serializable]
public class ItemWrapper
{
    public List<ItemCreat> objects;
}


[Serializable]
public class Map
{
    public List<Vector2Int> floors = new List<Vector2Int>();
}
[Serializable]
public class ItemCreat
{
    public Vector3 creatPos;
    public string name;
}
[Serializable]
public class Drop
{
    public Vector3 pos;
    public string name;
}
[Serializable]
public class DropWrapper
{
    public List<Drop> drops;
}
public class GameData : MonoBehaviour
{
    public List<ObjectData> objectDatas = new List<ObjectData>();
    public List<ItemCreat> itemCreats = new List<ItemCreat>();
    public List<Drop> Drops = new List<Drop>();
    public List<Vector2Int> floor = new List<Vector2Int>();
    public GameObject[] game;
    public GameObject[] items;
    public GameObject[] drop;
    public RoomFirstGenerator room;
    public bool isSave;
    [SerializeField, Header("???????")]
    private CinemachineVirtualCamera vCamera;
    public void SaveData()
    {
        PlayerPrefs.SetInt("coin", CoinManager.Instance.coinAmount);
        InventoryManager.Instance.SaveInventory(InventoryManager.Instance.inventoryData);
        isSave = true;
        objectDatas.Clear();
        itemCreats.Clear();
        floor.Clear();
        Drops.Clear();
        floor = room.allFloor.ToList();
        if (!(floor.Count == 0))
        {
            string mapJson = JsonUtility.ToJson(new Map { floors = floor });
            File.WriteAllText(Application.persistentDataPath + "/saved_Mapobjects.json", mapJson);
        }

        GameObject[] dropps = GameObject.FindGameObjectsWithTag("Item");
        List<GameObject> dropList = new List<GameObject>(dropps);
        foreach(GameObject gameObject2 in dropList)
        {
            Drop temp = new Drop();
            temp.pos = gameObject2.transform.position;
            temp.name = gameObject2.name;
            Drops.Add(temp);
        }
        string dropJson = JsonUtility.ToJson(new DropWrapper { drops = Drops });
        File.WriteAllText(Application.persistentDataPath + "/saved_Dropobjects.json", dropJson);

        GameObject[] itemtemp = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        List<GameObject> allItem = new List<GameObject>(itemtemp);
        allItem.AddRange(coins);
        foreach(GameObject gameObject1 in allItem)
        {
            ItemCreat itemCreat = new ItemCreat();
            itemCreat.creatPos = gameObject1.transform.position;
            itemCreat.name = gameObject1.name;
            itemCreats.Add(itemCreat);
        }
        string itemJson = JsonUtility.ToJson(new ItemWrapper { objects=itemCreats});
        File.WriteAllText(Application.persistentDataPath + "/saved_Itemobjects.json", itemJson);

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        List<GameObject> allobject = gameObjects.ToList<GameObject>();
        allobject.Add(player);
        allobject.Add(boss);
        foreach(GameObject gameObject in allobject)
        {
            ObjectData objectData = new ObjectData();
            objectData.name = gameObject.tag;
            objectData.pos = gameObject.transform.position;
            objectData.scale = gameObject.transform.localScale;
            Health health = gameObject.GetComponent<Health>();
            objectData.currentHp = health.currentHp;
            SaveLevelData(gameObject, objectData);
            objectDatas.Add(objectData);
        }
        string json = JsonUtility.ToJson(new Wrapper{ objects = objectDatas });
        File.WriteAllText(Application.persistentDataPath + "/saved_objects.json", json);
        Debug.Log("???????");
    }

    public void LoadGameData()
    {
        CoinManager.Instance.coinAmount= PlayerPrefs.GetInt("coin");
        CoinManager.Instance.UpdateCoin();
        InventoryManager.Instance.LoadInventory();
        InventoryUI.Instance.RefreshUI();
        string dropPath = Application.persistentDataPath + "/saved_Dropobjects.json";
        if (!File.Exists(dropPath))
        {
            Debug.Log("?????????????");
            return;
        }
        string dropData = File.ReadAllText(dropPath);
        DropWrapper dropWrapper = JsonUtility.FromJson<DropWrapper>(dropData);
        foreach(Drop drop in dropWrapper.drops)
        {
            GameObject obj = FindDrop(drop);
            if (obj != null)
            {
                GameObject game = Instantiate(obj);
                game.transform.position = drop.pos;
                game.name = drop.name;
            }
        }

        string mapPath = Application.persistentDataPath + "/saved_Mapobjects.json";
        if (!File.Exists(mapPath))
        {
            Debug.Log("?????????");
            return;
        }
        string mapData = File.ReadAllText(mapPath);
        Map map = JsonUtility.FromJson<Map>(mapData);
        HashSet<Vector2Int> floo = map.floors.ToHashSet<Vector2Int>();
        room.LoadMap(floo);

        string itemPath = Application.persistentDataPath + "/saved_Itemobjects.json";
        if (!File.Exists(itemPath))
        {
            Debug.Log("?????????????");
            return;
        }
        string itemJson = File.ReadAllText(itemPath);
        ItemWrapper itemWrapper = JsonUtility.FromJson<ItemWrapper>(itemJson);
        ItemPlaceManager.Instance.ClearItem();
        foreach(ItemCreat itemCreat in itemWrapper.objects)
        {
            GameObject obj = FindItem(itemCreat);
            if (obj != null)
            {
                GameObject gameObject = Instantiate(obj);
                gameObject.transform.position = itemCreat.creatPos;
                gameObject.name = itemCreat.name;
                gameObject.transform.SetParent(ItemPlaceManager.Instance.itemParent.transform);
            }
        }



        string path = Application.persistentDataPath + "/saved_objects.json";
        if (!File.Exists(path))
        {
            Debug.Log("??????");
            return;
        }
        string json = File.ReadAllText(path);
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
        ItemPlaceManager.Instance.ClearBoss();
        ItemPlaceManager.Instance.ClearEnemy();
        ItemPlaceManager.Instance.ClearPlayer();
        Transform enemyParent = GameObject.Find("EnemyManager").transform;
        foreach(ObjectData objectData1 in wrapper.objects)
        {
            if (objectData1.name == "Player")
            {
                GameObject player = FindGameObject(objectData1);
                GameObject gameObject = Instantiate(player);
                gameObject.transform.position = objectData1.pos;
                gameObject.transform.localScale = objectData1.scale;
                UseItemManager.Instance.player = gameObject;
                LoadLevelData(gameObject, objectData1);
                Health health = gameObject.GetComponent<Health>();
                health.SetHp(objectData1.currentHp);
                vCamera.LookAt = gameObject.transform;
                vCamera.Follow = gameObject.transform;
                break;
            }
        }

        foreach (ObjectData objectData in wrapper.objects)
        {
            GameObject obj=FindGameObject(objectData);
            if (obj == null||objectData.name=="Player")
            {
                continue;
            }
            GameObject gameObject = Instantiate(obj);
            gameObject.transform.position = objectData.pos;
            gameObject.transform.localScale = objectData.scale;
            LoadLevelData(gameObject, objectData);
            Health health = gameObject.GetComponent<Health>();
            health.SetHp(objectData.currentHp);
            if (obj.CompareTag("Enemy") || obj.CompareTag("Boss"))
            {
                gameObject.transform.SetParent(enemyParent.transform);
            }
            
        }
    }

    private GameObject FindGameObject(ObjectData objectData)
    {
        foreach (GameObject gameObject in game)
        {
            if (objectData.name == gameObject.tag)
            {
                return gameObject;
            }
        }
        return null;
    }

    private void SaveLevelData(GameObject gameObject, ObjectData objectData)
    {
        LevelUp levelUp = gameObject.GetComponent<LevelUp>();
        if (levelUp == null || levelUp.LevelDataSO == null)
        {
            return;
        }

        LevelDataSO levelData = levelUp.LevelDataSO;
        objectData.hasLevelData = true;
        objectData.level = levelData.level;
        objectData.currentExp = levelData.currentExp;
        objectData.expToNextLevel = levelData.expToNextLevel;
        objectData.baseMaxHp = levelData.baseMaxHp;
        objectData.baseAttack = levelData.baseAttack;
    }

    private void LoadLevelData(GameObject gameObject, ObjectData objectData)
    {
        if (!objectData.hasLevelData)
        {
            return;
        }

        LevelUp levelUp = gameObject.GetComponent<LevelUp>();
        if (levelUp == null || levelUp.LevelDataSO == null)
        {
            return;
        }

        LevelDataSO levelData = levelUp.LevelDataSO;
        levelData.level = objectData.level;
        levelData.currentExp = objectData.currentExp;
        levelData.expToNextLevel = objectData.expToNextLevel;
        levelData.baseMaxHp = objectData.baseMaxHp;
        levelData.baseAttack = objectData.baseAttack;
    }
    
    private GameObject FindDrop(Drop dropss)
    {
        foreach(GameObject gameObject in drop)
        {
            if (gameObject.name == dropss.name)
            {
                return gameObject;
            }
        }
        return null;
    }

    private GameObject FindItem(ItemCreat itemCreat)
    {
        foreach(GameObject gameObject in items)
        {
            if (gameObject.name == itemCreat.name)
            {
                return gameObject;
            }
        }
        return null;
    }
}
