using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player1Controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] public Animator animator;
    [Range(0, .3f)][SerializeField] private float movementSmoothing = .05f;
    public Transform hitbox;
    public float attackRangeX = 1f;
    public float attackRangeY = 1f;
    private Vector2 attackSize;
    public LayerMask opponentLayers;
    public int health = 100;

    private Vector3 vel = Vector3.zero;
    private bool attacking = false;
    public bool blocking = false;
    private bool isDead;
    //float attackRate = 0.6f;
    //float attackTimer = 0f;

    float blockRate = 0.6f;
    float blockTimer = 0f;
    private float jumpingPower = 16f;

    private bool isFacingRight = true;
    private bool isGrounded = true;

    //Audio
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnEnable()
    {
        // Register to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unregister from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset health and other variables
        health = 100;
        isDead = false;
        attacking = false;
        blocking = false;
        isFacingRight = true;
        isGrounded = true;
    }

    private void Flip()
    {
        if (isDead || GameObject.FindGameObjectWithTag("Player2") == null)
        {
            return;
        }

        Player2Controller player2Controller = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player2Controller>();

        if (player2Controller != null)
        {
            if (player2Controller.transform.position.x < transform.position.x && isFacingRight
                || player2Controller.transform.position.x > transform.position.x && !isFacingRight)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }


    public void Move(float move)
    {
        if (isDead)
        {
            return;
        }
        if (!attacking && !blocking)
        { //don't want players to be able to move and attack at the same time 
            Vector3 targetVelocity = new Vector2(move * 10f, body.velocity.y);
            body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref vel, movementSmoothing);
        }
        else
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
    }

    public void ShowHitbox()
    {
        if (isDead || GameObject.FindGameObjectWithTag("Player2") == null)
        {
            return;
        }
        Player2Controller opponent = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player2Controller>();
        if (opponent == null || opponent.IsDead())
        {
            return;
        }
        Collider2D[] targets = Physics2D.OverlapBoxAll(hitbox.position, attackSize, 0, opponentLayers);
        foreach (Collider2D target in targets)
        {
            Debug.Log("hit");
            if (target.GetComponent<Player2Controller>().blocking == false)
            {
                target.GetComponent<Player2Controller>().health -= 20;
                target.GetComponent<Player2Controller>().animator.SetTrigger("Hit");
                target.GetComponent<Player2Controller>().MakeActionable();
                Debug.Log(target.GetComponent<Player2Controller>().health);
                audioManager.PlaySFX(audioManager.hit);
            }
            else
            {
                audioManager.PlaySFX(audioManager.contact);
            }
        }
    }

    public void ShowHeavy()
    {
        if (isDead || GameObject.FindGameObjectWithTag("Player2") == null)
        {
            return;
        }
        Player2Controller opponent = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player2Controller>();
        if (opponent == null || opponent.IsDead())
        {
            return;
        }
        Collider2D[] targets = Physics2D.OverlapBoxAll(hitbox.position, attackSize, 0, opponentLayers);
        foreach (Collider2D target in targets)
        {
            Debug.Log("hit");
            if (target.GetComponent<Player2Controller>().blocking == false)
            {
                target.GetComponent<Player2Controller>().health -= 40;
                target.GetComponent<Player2Controller>().animator.SetTrigger("Hit");
                Debug.Log(target.GetComponent<Player2Controller>().health);
                audioManager.PlaySFX(audioManager.hit);
            }
            else
            {
                audioManager.PlaySFX(audioManager.contact);
            }
        }
    }

    public void MakeActionable()
    {
        //attackTimer = 0;
        attacking = false;
    }

    public void Attack()
    {
        if (isDead)
        {
            return;
        }
        if (!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attack");
            audioManager.PlaySFX(audioManager.swing);
            // Hitbox trigger now set by the animation
        }
    }

    public void Heavy()
    {
        if (isDead)
        {
            return;
        }
        if (!attacking)
        {
            attacking = true;
            animator.SetTrigger("Heavy");
            audioManager.PlaySFX(audioManager.swing);
        }
    }

    public void Block()
    {
        if (isDead)
        {
            return;
        }
        if (!blocking)
        {
            blocking = true;
            animator.SetTrigger("Block");
            audioManager.PlaySFX(audioManager.block);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            Vector3 normal = other.GetContact(0).normal;
            if (normal == Vector3.up)
            {
                isGrounded = true;
            }
        }
    }

    void Start()
    {
        attackSize = new Vector2(attackRangeX, attackRangeY);
        isGrounded = true;
    }

    private IEnumerator HandleDeath()
    {
        GameManager.instance.Player2WinsRound();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (health <= 0)
        {
            isDead = true;
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Death");
            StartCoroutine(HandleDeath());
        }

        if (!attacking && !blocking)
        {
            if (Input.GetButtonDown("Jump") && isGrounded == true)
            {
                animator.SetTrigger("Jump");
                body.velocity = new Vector2(body.velocity.x, jumpingPower);
                isGrounded = false;
            }

            if (Input.GetButtonUp("Jump"))
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Heavy();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Block();
            }
        }

        // if (attacking)
        // { //artificial cooldown for the attack animation
        //     attackTimer += Time.deltaTime;
        //     if (attackTimer >= attackRate)
        //     {
        //         attackTimer = 0;
        //         attacking = false;
        //     }
        // }

        if (blocking)
        { //block cooldown
            blockTimer += Time.deltaTime;
            if (blockTimer >= blockRate)
            {
                blockTimer = 0;
                blocking = false;
            }
        }

        if (!isDead)
        {
            Flip();
        }
    }
    public bool IsDead()
    {
        return isDead;
    }

    void OnDrawGizmosSelected()
    {
        if (hitbox == null)
        {
            return;
        }
        Vector2 boxSize = new Vector2(attackRangeX, attackRangeY);
        Gizmos.DrawWireCube(hitbox.position, boxSize);
    }
}
