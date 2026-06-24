using UnityEngine;

public class Shot : MonoBehaviour
{
    [Header("速度")]
    public float speed = 5f;
    [Header("存在时间")]
    public float lifeTime = 2f;
    [Header("攻击力")]
    public float attack = 3f;
    private Rigidbody2D rigidbody2;
    

    private void Awake()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void ShotArrow(Vector2 dir)
    {
        rigidbody2.velocity = speed * dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")||collision.CompareTag("Boss"))
        {
            IDamage damage = collision.GetComponent<IDamage>();
            if (damage != null)
            {
                damage.TakeDamage(attack);
                Destroy(gameObject);
            }
        }else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
