using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamage
{
    [Header("最大生命值")]
    public float maxHp=10;
    [Header("当前生命值")]
    public float currentHp = 0;
    public event Action<float> OnDamage;
    public event Action OnDead;
    public event Action<float> OnAdd;
    public GameObject hpBarParent;
    public GameObject hpBarPerfab;
    public Vector3 offset;
    private HPUI hPUI;


    private void Awake()
    {
        currentHp = maxHp;
        hpBarParent = GameObject.Find("HP");
        CreatHpBar();
    }
    
    public void TakeDamage(float damage,Vector3 pos)
    {
        currentHp -= damage;
        Knockback knockback = GetComponent<Knockback>();
        if (knockback != null)
        {
            knockback.ApplyKnockback(pos);
        }
        OnDamage?.Invoke(damage);
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void SetHp(float hp)
    {
        currentHp = hp;
        if (hPUI != null)
        {
            hPUI.HpUIUpdate(hp);
        }
       
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        OnDamage?.Invoke(damage);
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void AddHp(float add)
    {
        currentHp += add;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        OnAdd?.Invoke(add);
    }

    public void Die()
    {
        OnDead?.Invoke();
        Destroy(gameObject);
    }

    private void CreatHpBar()
    {
        if (hpBarPerfab != null)
        {
           GameObject obj= Instantiate<GameObject>(hpBarPerfab);
            obj.transform.SetParent(hpBarParent.transform, false);
            hPUI = obj.GetComponent<HPUI>();
            if (hPUI != null)
            {
                hPUI.SetTarget(gameObject, offset);
            }
        }
    }
}
