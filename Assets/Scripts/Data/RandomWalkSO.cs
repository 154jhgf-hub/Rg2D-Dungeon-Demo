using UnityEngine;

[CreateAssetMenu(fileName ="RandomWalkParameters",menuName ="PCG/RandomWalkData")]
public class RandomWalkSO : ScriptableObject
{
    [Header("迭代次数")]
    public int iterations = 10;
    [Header("最大步距")]
    public int walkLength = 10;
    [Header("每次迭代是否随机初始位置")]
    public bool startRandomPos = true;
}
