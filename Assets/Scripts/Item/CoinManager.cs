
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { private set; get; }
    public int coinAmount;
    public TextMeshProUGUI coinText;
    //public Action<int> OnCoinChange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void ResetCoin()
    {
        coinAmount = 0;
        coinText.text = "½ð±Ò:" + coinAmount;
    }

    public void AddCoin(int number)
    {
        coinAmount += number;
        UpdateCoin();
    }

    
    public void UpdateCoin()
    {
        coinText.text = "½ð±Ò: " + coinAmount;
    }
}
