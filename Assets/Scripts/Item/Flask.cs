
using UnityEngine;

public class Flask : MonoBehaviour
{
    public float addHealth = 5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health health = collision.GetComponent<Health>();
            health.AddHp(addHealth);
            ToastUI.Show($"HP +{addHealth}");
            Destroy(gameObject);
        }
    }
}
