using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Walk,
    Attack,
    Damage,
    Dead
}
public class EnemyAI : MonoBehaviour
{
    [Header("??????")]
    public float enemyAttack = 3f;
    [Header("???")]
    public float speed = 2f;
    [Header("??????")]
    public float patrolRadius = 3f;
    [Header("????????")]
    public float attackRadius = 1.5f;
    [Header("???????")]
    public float followRadius = 5f;
    [Header("???????")]
    public float attackCD = 1f;
    [SerializeField, Header("??????��???")]
    private float attackSoundDelay = 0f;
    private float timer = 0;
    [Header("???????")]
    public LayerMask layerMask;
    private Animator animator;
    public EnemyState currentState;
    public Transform player;
    private float dis;
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    public Color red;
    private Rigidbody2D rg2D;
    public Vector2 moveDir;
    [Header("?????????")]
    public float changeDirTime = 2f;
    private float dirTimer = 0f;
    private float raycastLength = 0.7f;
    private float followPlayerTimer = 0.5f;
    private BoxCollider2D enemyCollider;
    private Attack attack;
    private Health health;
    //??????????
    private float separationRadius = 0.5f;
    private float separationForce = 3f;
    private Vector2 separationDir;
    [Header("?????")]
    public LayerMask enemyLayer;

    private LevelUp levelUp;
    private AudioSource audioSource;
    private Coroutine attackSoundRoutine;
    public Vector2 check;
    public Vector3 offset=new Vector3(0.5f,0,0);
    public float volume = 0.5f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rg2D = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>();
        attack = GetComponent<Attack>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDamage +=Damage;
        health.OnDead += Dead;
    }

    private void OnDisable()
    {
        health.OnDamage -= Damage;
        health.OnDead -= Dead;
        if (attackSoundRoutine != null)
        {
            StopCoroutine(attackSoundRoutine);
            attackSoundRoutine = null;
        }
    }
    private void Start()
    {
        currentState = EnemyState.Idle;
        timer = attackCD;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        moveDir = Direction2D.RandomDir();
        startColor = spriteRenderer.color;
        levelUp = FindObjectOfType<LevelUp>();
        audioSource = GetComponent<AudioSource>();
        EnemyManager.Instance.RegisterEnemy(this);
        //audioSource.clip = AudioManager.Instance.footsteps[1];
        //audioSource.loop = true;
    }

    private void Update()
    {
        if (player != null)
        {
            dis = Vector2.Distance(transform.position, player.position);
            if(dis<10f&& currentState == EnemyState.Walk&&!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(AudioManager.Instance.footsteps[1]);
                
            }
            else
            {
                audioSource.Stop();
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (player != null)
        {
            dis = Vector2.Distance(transform.position, player.position);
        }
        timer += Time.fixedDeltaTime;
        separationDir = SeparationDir();
        EnemyAction();
        
    }
    

    private void EnemyAction()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                spriteRenderer.color = startColor;
                Idle();
                currentState = EnemyState.Walk;
                break;
            case EnemyState.Walk:
                Walk();
                if (dis < attackRadius)
                {
                    currentState = EnemyState.Attack;
                }
                break;
            case EnemyState.Attack:
                if (timer >= attackCD)
                {
                    Attack();
                }
                else
                {
                    currentState = EnemyState.Idle;
                }
                
                break;
            case EnemyState.Damage:
                spriteRenderer.color = red;
                break;
            case EnemyState.Dead:

                break;
        }
    }
    private void Idle()
    {
        animator.SetFloat("Speed",currentState == EnemyState.Walk ? 1 : 0);
    }

    private void Walk()
    {
        if (dis > followRadius)
        {
            Wander();
            if (moveDir.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            if (dis >= 1f)
            {
                Chase();
                if (player.position.x > transform.position.x)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }
            }
            
        }
        animator.SetFloat("Speed", currentState == EnemyState.Walk ? 1 : 0);
        
    }

    private void Damage(float damage)
    {
        if (currentState == EnemyState.Damage)
        {
            return;
        }
        currentState = EnemyState.Damage;
        animator.SetTrigger("isHurt");
        AudioManager.Instance.Play3DSFX(AudioManager.Instance.hitSounds[1],transform.position,volume,1,5,1);
        StartCoroutine(WaitForAnimationAndSwitchState());
    }

    private void Attack()
    {
        timer = 0;
        animator.SetTrigger("isAttack");
        if (attackSoundRoutine != null)
        {
            StopCoroutine(attackSoundRoutine);
        }
        attackSoundRoutine = StartCoroutine(PlayAttackSoundAfterDelay());
        StartCoroutine(WaitForAnimationAndSwitchState());
    }

    private IEnumerator PlayAttackSoundAfterDelay()
    {
        if (attackSoundDelay > 0f)
        {
            yield return new WaitForSeconds(attackSoundDelay);
        }

        if (currentState == EnemyState.Dead)
        {
            attackSoundRoutine = null;
            yield break;
        }

        PlayAttackSound();
        attackSoundRoutine = null;
    }

    private void PlayAttackSound()
    {
        if (this.gameObject.CompareTag("Boss"))
        {
            AudioManager.Instance.Play3DSFX(AudioManager.Instance.enemyRoar[0],
                transform.position, 1, 1, 5, 10f);
        }
        else
        {
            AudioManager.Instance.Play3DSFX(AudioManager.Instance.attackSounds[3], transform.position,0.5f,1,5,1);
        }
    }

    IEnumerator WaitForAnimationAndSwitchState()
    {
        // ????????????
        float animationLength = GetCurrentAnimationLength();

        // ??????????????
        yield return new WaitForSeconds(animationLength);

        // ??????????????
        
        currentState = EnemyState.Idle;
    }
    float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    public void DoAttack()
    {
        attack.DooAttack(enemyAttack);
    }

    private void Wander()
    {
        dirTimer += Time.fixedDeltaTime;
        if (dirTimer >= changeDirTime)
        {
            SetNewDir();
            dirTimer = 0;
        }
        MoveWithCheck(moveDir);
    }

    private void Chase()
    {
       Vector2 dir = (player.position - transform.position).normalized;
        if (followPlayerTimer>=0.5f)
        {
            moveDir = GetBestDir(dir);
            followPlayerTimer = 0;
        }
        followPlayerTimer += Time.fixedDeltaTime;
       MoveWithCheck(moveDir);
    }

    private Vector2 GetBestDir(Vector2 pos)
    {
        Vector2 bestDir = moveDir;
        float bestDot = -Mathf.Infinity;
        foreach(Vector2 dir in Direction2D.dir8)
        {
            if (IsDirectionBlocked(dir))
            {
                continue;
            }
            float dot = Vector2.Dot(dir, pos);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDir = dir;
            }
        }
        return bestDir;
    }
    private void MoveWithCheck(Vector2 dir)
    {
        Vector2 move = (dir + separationDir * separationForce).normalized;
        if (!IsDirectionBlocked(move))
        {
            rg2D.MovePosition(rg2D.position +move * speed * Time.fixedDeltaTime);
        }
        else
        {
            SetNewDir();
        }
    }

    private bool IsDirectionBlocked(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
        {
            return true;
        }

        Vector2 normalizedDir = dir.normalized;
        if (BoxCastWall(normalizedDir))
        {
            return true;
        }

        // Diagonal movement can slip through tile corners if only the diagonal is checked.
        if (Mathf.Abs(normalizedDir.x) > 0.01f && Mathf.Abs(normalizedDir.y) > 0.01f)
        {
            Vector2 horizontal = new Vector2(Mathf.Sign(normalizedDir.x), 0f);
            Vector2 vertical = new Vector2(0f, Mathf.Sign(normalizedDir.y));
            return BoxCastWall(horizontal) || BoxCastWall(vertical);
        }

        return false;
    }

    private bool BoxCastWall(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        RaycastHit2D hit = Physics2D.BoxCast(
            GetCastOrigin(dir),
            check,
            angle,
            dir,
            raycastLength,
            layerMask);

        return hit.collider != null;
    }

    private Vector2 GetCastOrigin(Vector2 dir)
    {
        Vector2 castOffset = offset;
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            castOffset.x = Mathf.Abs(offset.x) * Mathf.Sign(dir.x);
        }
        else
        {
            castOffset.x = Mathf.Abs(offset.x) * (spriteRenderer.flipX ? -1f : 1f);
        }

        return (Vector2)transform.position + castOffset;
    }
    public void DrawOverlapBox(Vector2 center, Vector2 size, float angle, Color color, float duration = 0f)
    {
        // ??????????????????????
        Vector2 halfSize = size / 2;
        Vector2[] localCorners = new Vector2[]
        {
        new Vector2(-halfSize.x, -halfSize.y),  // ????
        new Vector2( halfSize.x, -halfSize.y),  // ????
        new Vector2( halfSize.x,  halfSize.y),  // ????
        new Vector2(-halfSize.x,  halfSize.y)   // ????
        };

        // ????????????
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector2[] worldCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            worldCorners[i] = center + (Vector2)(rotation * localCorners[i]);
        }

        // ??????????
        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(worldCorners[i], worldCorners[(i + 1) % 4], color, duration);
        }
    }
    

    private void SetNewDir()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector2Int dir = Direction2D.RandomDir();
            if (!IsDirectionBlocked(dir))
            {
                moveDir = dir;
                return;
            }
        }
        moveDir = Vector2.zero;
    }
    
    private Vector2 SeparationDir()
    {
        Vector2 separtionDir = Vector2.zero;
        int count = 0;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(rg2D.position, separationRadius,enemyLayer);
        foreach(Collider2D enemy in enemies)
        {
            if (enemy.gameObject == gameObject)
            {
                continue;
            }
            if (!enemy.CompareTag("Enemy")&&!enemy.CompareTag("Boss"))
            {
                continue;
            }
            Vector2 dir = transform.position - enemy.transform.position;
            float dis = dir.magnitude;
            if (dis < separationRadius)
            {
                float strength = (separationRadius - dis) / separationRadius;
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

    private void Dead()
    {
        EnemyManager.Instance.OnEnemyDied(this);
        currentState = EnemyState.Dead;
        levelUp.AddExp(30);
        ItemManager.Instance.CreatItem(rg2D.position);
    }
}
