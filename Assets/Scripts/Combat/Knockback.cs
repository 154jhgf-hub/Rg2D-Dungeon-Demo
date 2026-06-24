using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private float knockDistance = 2f;
    private float knockSpeed = 10f;
    private Rigidbody2D rb;
    private Coroutine knockCoroutine;
    public LayerMask wallLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 pos)
    {
        if (knockCoroutine != null)
        {
            StopCoroutine(knockCoroutine);
        }
        Vector3 dir = (rb.position - pos).normalized;
        knockCoroutine=StartCoroutine(KnockbackRoutine(dir));
    }

    IEnumerator KnockbackRoutine(Vector2 dir)
    {
        float remainDistance = knockDistance;
        float checkDis = 0.7f;
        while (remainDistance > 0)
        {
            float moveDis = knockSpeed * Time.fixedDeltaTime;
            moveDis = Mathf.Min(moveDis, remainDistance);
            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, moveDis + checkDis, wallLayer);
            if (hit.collider != null)
            {
                float tempDis = hit.distance - 0.5f;
                tempDis = Mathf.Max(0, tempDis);
                rb.MovePosition(rb.position + dir * tempDis);
                yield break;
            }
            rb.MovePosition(rb.position + dir * moveDis);
            remainDistance -= moveDis;
            yield return new WaitForFixedUpdate();
        }
        knockCoroutine = null;
    }

}
