using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CoinManager.Instance.AddCoin(1);
            AudioManager.Instance.PlaySound(AudioManager.Instance.pickupSounds[0]);
            ToastUI.Show("金币 +1");
            Destroy(gameObject);
        }
    }
}
