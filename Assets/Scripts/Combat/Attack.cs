
using UnityEngine;

public class Attack : MonoBehaviour
{
    private float attack = 3;
    [Header("¹¥»÷·¶Î§")]
    public float attackRadius = 1;
    public LayerMask layerMask;
    private SpriteRenderer spriteRenderer;
    public Vector2 dir=Vector2.right;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void DooAttack(float attack)
    {
        this.attack = attack;
        dir = spriteRenderer.flipX == true ? Vector2.left : Vector2.right;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll
            (new Vector2 (transform.position.x,transform.position.y) + dir * (attackRadius / 2),
            new Vector2(attackRadius, attackRadius),0, layerMask);
        foreach(Collider2D collider in collider2Ds)
        {
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                damage.TakeDamage(attack,transform.position);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position +(Vector3)dir*( attackRadius / 2);
        Vector3 size = new Vector3(attackRadius, attackRadius, 0);

        Gizmos.DrawWireCube(center, size);
    }
}
