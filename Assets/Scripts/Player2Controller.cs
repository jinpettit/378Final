using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player2Controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] public Animator animator;
    [Range(0, .3f)][SerializeField] private float movementSmoothing = .05f;
    public Transform hitbox;
    public float attackRangeX = 1f;
    public float attackRangeY = 1f;
    private Vector2 attackSize;
    public LayerMask opponentLayers;
    private Vector3 vel = Vector3.zero;
    private bool attacking = false;
    private bool isDead;

    //float attackRate = 0.6f;
    //float attackTimer = 0f;
    public int health = 100;

    public bool blocking = false;
    float blockRate = 0.6f;
    float blockTimer = 0f;

    private float jumpingPower = 16f;
    private bool isFacingLeft = true;
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
        isFacingLeft = true;
        isGrounded = true;
    }


    private void Flip()
    {
        if (isDead || GameObject.FindGameObjectWithTag("Player1") == null)
        {
            return;
        }

        Player1Controller player1Controller = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player1Controller>();

        if (player1Controller != null)
        {
            if (player1Controller.transform.position.x > transform.position.x && isFacingLeft
                || player1Controller.transform.position.x < transform.position.x && !isFacingLeft)
            {
                isFacingLeft = !isFacingLeft;
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
        Collider2D[] targets = Physics2D.OverlapBoxAll(hitbox.position, attackSize, 0, opponentLayers);
        foreach (Collider2D target in targets)
        {
            Debug.Log("hit");
            if (target.GetComponent<Player1Controller>().blocking == false)
            {
                target.GetComponent<Player1Controller>().health -= 20;
                target.GetComponent<Player1Controller>().animator.SetTrigger("Hit");
                Debug.Log(target.GetComponent<Player1Controller>().health);
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
        Collider2D[] targets = Physics2D.OverlapBoxAll(hitbox.position, attackSize, 0, opponentLayers);
        foreach (Collider2D target in targets)
        {
            Debug.Log("hit");
            if (target.GetComponent<Player1Controller>().blocking == false)
            {
                target.GetComponent<Player1Controller>().health -= 40;
                target.GetComponent<Player1Controller>().animator.SetTrigger("Hit");
                Debug.Log(target.GetComponent<Player1Controller>().health);
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
            // Hitbox trigger now set by animation
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
    }

    private IEnumerator HandleDeath()
    {
        GameManager.instance.Player1WinsRound();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (health <= 0)
        {
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Death");
            isDead = true;
            StartCoroutine(HandleDeath());
        }

        if (!attacking && !blocking)
        {
            if (Input.GetButtonDown("Jump2") && isGrounded == true)
            {
                animator.SetTrigger("Jump");
                body.velocity = new Vector2(body.velocity.x, jumpingPower);
                isGrounded = false;
            }

            if (Input.GetButtonUp("Jump2"))
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);

            }

            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.Period))
            {
                Heavy();
            }

            if (Input.GetKeyDown(KeyCode.Slash))
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
