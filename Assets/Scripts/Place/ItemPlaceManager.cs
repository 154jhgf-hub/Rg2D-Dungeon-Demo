using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ItemPlaceManager : MonoBehaviour
{
    public static ItemPlaceManager Instance { private set; get; }
    [SerializeField, Header("?????????")]
    private GameObject itemObj;
    [SerializeField, Header("?????б?")]
    private List<ItemInfo> itemInfos=new List<ItemInfo>();
    [SerializeField, Header("?????????????")]
    public GameObject itemParent;
    [SerializeField, Header("?????????")]
    private GameObject enemyPerfab;
    [SerializeField, Header("Boss?????")]
    private GameObject bossPerfab;
    [SerializeField, Header("Boss生成在玩家附近")]
    private bool spawnBossNearPlayer = false;
    [SerializeField, Header("Boss附近生成距离")]
    private float bossNearPlayerDistance = 4f;
    [SerializeField, Header("????????")]
    private int enemyCount = 10;
    [SerializeField, Header("????????")]
    private GameObject playerPerfab;
    [SerializeField,Header("???????")]
    private CinemachineVirtualCamera vCamera;
    [SerializeField, Header("?????????")]
    private float distance = 5f;
    private GameObject player;
    private float currentDis;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void SetItem(ItemPlaceHelp itemPlaceHelp)
    {
        ClearItem();
        foreach(ItemInfo itemInfo in itemInfos)
        {
            if (itemInfo.itemData.placeType == PlaceType.Wall)
            {
                continue;
                
            }
            int count =UnityEngine.Random.Range(itemInfo.minQuantity, itemInfo.maxQuantity+1);
            for(int i=0;i<count;i++)
            {
                Vector2? pos = itemPlaceHelp.GetItemPlacePos(
                itemInfo.itemData.placeType, itemInfo.itemData.size, 10, itemInfo.itemData.offset);
                if (pos != null)
                {
                    Vector2 temp = pos.Value;
                    SetItemData(temp, itemInfo);
                }
            }
            
        }
    }

    private void SetItemData(Vector3 pos,ItemInfo itemInfo)
    {
       GameObject obj = Instantiate(itemInfo.itemData.prefab,pos, quaternion.identity);
        obj.transform.SetParent(itemParent.transform);
        obj.name = itemInfo.itemData.name;
        obj.transform.localPosition += new Vector3(1, 1,0) * 0.5f;
    }

    public void SetWallItem(HashSet<Vector2Int> walls,HashSet<Vector2Int> floor)
    {
        List<Vector2Int> temp = walls.OrderBy(x => UnityEngine.Random.value).ToList();
        foreach (ItemInfo itemInfo in itemInfos)
        {
            if (itemInfo.itemData.placeType == PlaceType.Wall)
            {
                int count = UnityEngine.Random.Range(itemInfo.minQuantity, itemInfo.maxQuantity + 1);
                for (int i = 0; i < count; i++)
                {
                    if (temp.Count == 0) 
                        break;
                    int index = UnityEngine.Random.Range(0, temp.Count);
                    Vector2Int p = temp[index];

                    if (floor.Contains(p + new Vector2Int(0, -1)))
                    {
                        SetItemData((Vector3Int)p, itemInfo);
                        temp.RemoveAt(index); // ????Ч
                    }
                }
                
            }
        }

        
    }

    public void SetPlayer(ItemPlaceHelp itemPlaceHelp)
    {
        ClearPlayer();
        Vector2Int? pos = itemPlaceHelp.GetItemPlacePos
            (PlaceType.OpenSpace, new Vector2Int(1, 2), 10, false);
        if (pos == null)
        {
            return;
        }
        Vector2 p = pos.Value;
       player = Instantiate(playerPerfab,(Vector3)p, Quaternion.identity);
        UseItemManager.Instance.player = player;
        player.transform.position += new Vector3(1, 1,0) * 0.5f;
        LevelUp levelUp = player.GetComponent<LevelUp>();
        levelUp.ResetLevel();
        vCamera.LookAt=player.transform;
        vCamera.Follow = player.transform;
    }

    public void SetBoss(ItemPlaceHelp itemPlaceHelp)
    {
        ClearBoss();
        Transform enemyParent = GameObject.Find("EnemyManager").transform;
        Vector2Int bossPos;
        if (spawnBossNearPlayer && TryGetBossNearPlayerPos(itemPlaceHelp, out bossPos))
        {
            itemPlaceHelp.PlaceBoss(bossPos);
            CreateBoss(bossPos, enemyParent);
            return;
        }

        float minDis = float.MaxValue;
        Vector2Int playerCenter=Vector2Int.zero;
        foreach(Vector2Int pos in RoomData.roomCenter)
        {
            float dis = Vector2.Distance(pos, player.transform.position);
            if (dis < minDis)
            {
                minDis = dis;
                playerCenter = pos;
            }
        }
        BFSRoomConnect bFSRoomConnect = new BFSRoomConnect();
        bFSRoomConnect.connect = RoomConnect.adjacencyList;
        bossPos = bFSRoomConnect.BFSFarthest(playerCenter);
        itemPlaceHelp.PlaceBoss(bossPos);
        CreateBoss(bossPos, enemyParent);
    }

    private bool TryGetBossNearPlayerPos(ItemPlaceHelp itemPlaceHelp, out Vector2Int bossPos)
    {
        bossPos = Vector2Int.zero;
        if (player == null || !itemPlaceHelp.itemPlacePos.ContainsKey(PlaceType.OpenSpace))
        {
            return false;
        }

        Vector2 playerPos = player.transform.position;
        List<Vector2Int> nearPositions = itemPlaceHelp.itemPlacePos[PlaceType.OpenSpace]
            .Where(pos => Vector2.Distance(pos, playerPos) <= bossNearPlayerDistance)
            .OrderBy(pos => Vector2.Distance(pos, playerPos))
            .ToList();

        foreach (Vector2Int pos in nearPositions)
        {
            if (pos == Vector2Int.FloorToInt(playerPos))
            {
                continue;
            }

            bossPos = pos;
            return true;
        }

        return false;
    }

    private void CreateBoss(Vector2Int bossPos, Transform enemyParent)
    {
        GameObject obj = Instantiate(bossPerfab, (Vector3Int)bossPos, Quaternion.identity);
        obj.transform.position += new Vector3(1, 1, 0) * 0.5f;
        obj.transform.SetParent(enemyParent);
    }

    public void SetEnemy(ItemPlaceHelp itemPlaceHelp)
    {
        ClearEnemy();
       
        Transform enemyParent = GameObject.Find("EnemyManager").transform;
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 creatPos;
            do
            {
                Vector2Int? pos = itemPlaceHelp.GetItemPlacePos
                (PlaceType.OpenSpace, new Vector2Int(1, 1), 10, false);
                if (pos == null)
                {
                    return;
                }
                Vector2 p = pos.Value;
                currentDis = Vector2.Distance(p, player.transform.position);
                creatPos = p;
            } while (currentDis < distance);
            GameObject obj = Instantiate(enemyPerfab, (Vector3)creatPos, Quaternion.identity);
            obj.transform.position += new Vector3(1, 1, 0) * 0.5f;
            obj.transform.SetParent(enemyParent);
        }

        
    }

    public void ClearPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            DestroyImmediate(player);
        }
    }

    public void ClearEnemy()
    {
        GameObject[] enemylist= GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemylist)
        {
            DestroyImmediate(enemy);
        }
    }

    public void ClearBoss()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        DestroyImmediate(boss);
    }
    public void ClearItem()
    {
        itemParent = GameObject.Find("Item Parent");
        if (itemParent)
        {
            DestroyImmediate(itemParent);
        }        
        itemParent = new GameObject("Item Parent");
    }
}
