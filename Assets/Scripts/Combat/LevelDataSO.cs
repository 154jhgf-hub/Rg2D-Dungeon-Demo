
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]
public class LevelDataSO : ScriptableObject
{
      public int level = 1;
      public int currentExp = 0;
      public int expToNextLevel = 100; // 升级所需经验基数

      // 基础属性（按等级增长）
      public int baseMaxHp = 100;
      public int baseAttack = 10;

      // 根据等级计算最终属性
      public int GetMaxHp() => baseMaxHp + (level - 1) * 20;
      public int GetAttack() => baseAttack + (level - 1) * 5;
}


