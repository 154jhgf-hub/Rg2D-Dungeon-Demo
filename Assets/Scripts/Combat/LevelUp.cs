using System;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public LevelDataSO LevelDataSO;
    public event Action<int> OnLevelUp;
    public event Action<int, int, int> OnExpChange;

    private void Start()
    {
        //ResetLevel();
    }
    public void AddExp(int amount)
    {
        LevelDataSO.currentExp += amount;
        int expUp = GetRequiredExp();
        while (LevelDataSO.currentExp >= expUp)
        {
            LevelDataSO.currentExp -= expUp;
            //升级
            Up();
            expUp = GetRequiredExp();
        }
        OnExpChange?.Invoke(LevelDataSO.currentExp, expUp, LevelDataSO.level);
    }

    private void Up()
    {
        LevelDataSO.level++;
        OnLevelUp?.Invoke(LevelDataSO.level);
    }

    public int GetRequiredExp()
    {
        // 可自定义公式，如：基数 * 等级，让升级曲线平滑
        return LevelDataSO.expToNextLevel + (LevelDataSO.level - 1) * 20;
    }

    public void ResetLevel()
    {
        LevelDataSO.currentExp = 0;
        LevelDataSO.baseMaxHp = 100;
        LevelDataSO.baseAttack = 10;
        LevelDataSO.expToNextLevel = 100;
        LevelDataSO.level = 1;
    }
}
