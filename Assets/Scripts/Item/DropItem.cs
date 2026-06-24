
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("µÙ¬‰∏≈¬ ")]
    public float dropChance = 0.5f;
    [Header("µÙ¬‰¡¶")]
    public float dropForce = 1f;
    private Rigidbody2D rg;
    public LayerMask itemLayer;


    private void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Drop();
    }
    private void Drop()
    {
        Vector2 dir=D();
        rg.AddForce(dir * dropForce, ForceMode2D.Impulse);
    }

    private  Vector2 D()
    {
        Vector2 separtionDir = Vector2.zero;
        int count = 0;
        Collider2D[] items = Physics2D.
            OverlapCircleAll(rg.position,0.16f, itemLayer);
        foreach (Collider2D item in items)
        {
            if (item.gameObject == gameObject)
            {
                continue;
            }
            if (!item.CompareTag("Item"))
            {
                continue;
            }
            Vector2 dir = transform.position - item.transform.position;
            float dis = dir.magnitude;
            if (dis <0.16f)
            {
                float strength = (0.16f - dis) / 0.16f;
                separtionDir += dir.normalized * strength;
                count++;
            }
        }
        if (count > 0)
        {
            separtionDir /= count;
        }
        return separtionDir.normalized;
    }
}
