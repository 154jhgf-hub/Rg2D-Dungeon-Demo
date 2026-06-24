using System.Collections;
using UnityEngine;

public class Peak : MonoBehaviour
{
    public float attack = 2f;
    public float timer = 1f;
    private bool isAttack = false;
    private Coroutine coroutine;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttack&&collision.GetComponent<IDamage>()!=null)
        {
            isAttack = true;
            coroutine = StartCoroutine(ContinueDamage(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAttack && collision.GetComponent<IDamage>() != null)
        {
            isAttack = false;
            StopCoroutine(ContinueDamage(collision));
        }
    }

    private IEnumerator ContinueDamage(Collider2D collider2D)
    {
        IDamage damage = collider2D.GetComponent<IDamage>();
        if (damage != null)
        {
            while (isAttack)
            {
                damage.TakeDamage(attack);
                yield return new WaitForSeconds(timer);
            }
        }
    }
    
}
