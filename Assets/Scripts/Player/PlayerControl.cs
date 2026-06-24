using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackType
{
    Light,
    Heavy,
    Shot
}


public class PlayerControl : MonoBehaviour
{
    [Header("ËÙ¶È")]
    public float speed = 5f;
    [Header("»ù´¡¹¥»÷")]
    public float baseAttack = 10f;
    [Header("Çá»÷")]
    public float lightAttack;
    [Header("ÖØ»÷")]
    public float heavyAttack;
    [Header("Éä»÷")]
    public float shotAttack;
    [Header("Çá»÷CD")]
    public float lightCD = 0.2f;
    [Header("ÖØ»÷CD")]
    public float heavyCD = 0.3f;
    [Header("Éä»÷CD")]
    public float shotCD = 0.3f;
    
    [Header("¹¥»÷ÀàÐÍ")]
    public AttackType attackType = AttackType.Light;
    [Header("Íæ¼ÒÐÐÎª")]
    private float attackCD;
    private float playerAttack;
    private Animator animator;
    private Vector2 move;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2;
    private Attack attack;
    private bool isAttack = true;
    public GameObject arrowPrefab;
    private Vector2 shotDir;
    private Health health;
    public GameObject levelUI;
    public GameObject levelParent;
    public LevelDataSO LevelDataSO;
    public LevelUp levelUp;
    public event Action<float> OnLevelUpHpUI;
    private AudioSource audioSource;
    // Start is called before the first frame update
    private void Awake()
    {
        health = GetComponent<Health>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2 = GetComponent<Rigidbody2D>();
        attack = GetComponent<Attack>();
        levelParent = GameObject.Find("HP");
        GameObject obj = Instantiate(levelUI);
        obj.transform.parent=levelParent.transform;
        levelUp = GetComponent<LevelUp>();
        levelUp.OnLevelUp += LevelUp;
        audioSource = GetComponent<AudioSource>();
        baseAttack = LevelDataSO.GetAttack();
        health.maxHp = (float)LevelDataSO.GetMaxHp();
    }

    private void OnEnable()
    {
        health.OnDamage += PlayerDamage;
        health.OnDead += PlayerDead;
    }

    private void OnDisable()
    {
        health.OnDamage -= PlayerDamage;
        health.OnDead -= PlayerDead;
    }

    private void Update()
    {
        if (!isAttack)
        {
            attackCD -= Time.deltaTime;
            if (attackCD <= 0)
            {
                isAttack = true;
            }
        }
    }

    private void FixedUpdate()
    {
        rigidbody2.velocity = move*speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (attackType == AttackType.Shot && !isAttack)
        {
            return;
        }
        move = context.ReadValue<Vector2>();
        move = move.normalized;
        animator.SetFloat("Speed", move.magnitude);
        if (move.magnitude > 0.1f)
        {
            audioSource.clip = AudioManager.Instance.footsteps[0];
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
        if (move.x < 0)
        {
            spriteRenderer.flipX = true;
        }else if (move.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isAttack)
        {
            return;
        }
        if (context.performed)
        {
            HandlerAttack(attackType);
            isAttack = false;
            animator.SetFloat("attackSelect",(float)attackType);
            animator.SetTrigger("isAttack");
            if ((int)attackType != 2)
            {
                AudioManager.Instance.PlaySound(
                AudioManager.Instance.attackSounds[(int)attackType]);
            }
        }
    }

    public void DoAttack()
    {
        attack.DooAttack(playerAttack);
    }
    
    public void ChangeAttackType(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           float a=(int)attackType;
            a++;
            if (a > 2)
            {
                a = 0;
            }
            attackType = (AttackType)a;
        }
    }

    public void HandlerAttack(AttackType type)
    {
        switch (type)
        {
            case AttackType.Light:
                playerAttack = baseAttack;
                attackCD = lightCD;
                break;
            case AttackType.Heavy:
                playerAttack = baseAttack*1.5f;
                attackCD = heavyCD;
                break;
            case AttackType.Shot:
                playerAttack = baseAttack;
                attackCD = shotCD;
                break;
        }
    }

    public void PlayerDamage(float a)
    {
        animator.SetTrigger("isHurt");
        AudioManager.Instance.PlaySound(AudioManager.Instance.hitSounds[0]);
    }
    public void Shott()
    {
        shotDir = spriteRenderer.flipX == true ? Vector2.left : Vector2.right;
        GameObject obj = Instantiate<GameObject>(
            arrowPrefab, transform.position, Quaternion.identity);
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        sprite.flipX = spriteRenderer.flipX;
        Shot shot = obj.GetComponent<Shot>();
        if (shot != null)
        {
            shot.ShotArrow(shotDir);
            AudioManager.Instance.PlaySound(AudioManager.Instance.attackSounds[2]);
        }
    }

    public void LevelUp(int n)
    {
        baseAttack = LevelDataSO.GetAttack();
        health.maxHp = (float)LevelDataSO.GetMaxHp();
        health.currentHp = (float)health.maxHp;
        AudioManager.Instance.PlaySound(AudioManager.Instance.pickupSounds[0]);
        OnLevelUpHpUI?.Invoke(n);
    }

    private void PlayerDead()
    {
        GameManager.Instance.endUI.SetText("ÓÎÏ·Ê§°Ü");
        GameManager.Instance.endUI.Show();
        Time.timeScale = 0;
    }
    
}
