using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image image;
    public LevelUp levelUp;
    private void Start()
    {
        levelUp = GameObject.FindWithTag("Player").GetComponent<LevelUp>();
        levelUp.OnExpChange += UpdateLevel;
        UpdateLevel(levelUp.LevelDataSO.currentExp,
            levelUp.GetRequiredExp(),
            levelUp.LevelDataSO.level);
    }
    public void UpdateLevel(int currentExp,int nextExp,int level)
    {
        float temp = (float)currentExp / nextExp;
        image.fillAmount = temp;
        levelText.text = level.ToString();
    }

}
