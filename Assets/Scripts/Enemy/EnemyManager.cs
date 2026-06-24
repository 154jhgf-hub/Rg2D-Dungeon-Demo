
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private int totalEnemies = 0;
    private int aliveEnemies = 0;

    public System.Action OnAllEnemiesDefeated;  // 事件

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 注册敌人
    public void RegisterEnemy(EnemyAI enemy)
    {
        totalEnemies++;
        aliveEnemies++;
    }

    // 敌人死亡时调用
    public void OnEnemyDied(EnemyAI enemy)
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            Debug.Log("所有敌人已死亡！");
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    // 获取剩余敌人数
    public int GetRemainingEnemies()
    {
        return aliveEnemies;
    }
}

