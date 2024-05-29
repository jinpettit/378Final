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

    float attackRate = 0.6f;
    float attackTimer = 0f;
    public int health = 100;

    public bool blocking = false;
    float blockRate = 0.6f;
    float blockTimer = 0f;

    //Audio
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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
            body.velocity = Vector2.zero;
        }
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
            Collider2D[] targets = Physics2D.OverlapBoxAll(hitbox.position, attackSize, 0, opponentLayers);
            foreach (Collider2D target in targets)
            {
                Debug.Log("hit");
                if (target.GetComponent<Player1Controller>().blocking == false)
                {
                    target.GetComponent<Player1Controller>().health -= 30;
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

    void Start()
    {
        attackSize = new Vector2(attackRangeX, attackRangeY);
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2);
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
            animator.Play("Death");
            isDead = true;
            StartCoroutine(HandleDeath());
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            Block();
        }

        if (attacking)
        { //artificial cooldown for the attack animation
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                attackTimer = 0;
                attacking = false;

            }
        }

        if (blocking)
        { //block cooldown
            blockTimer += Time.deltaTime;
            if (blockTimer >= blockRate)
            {
                blockTimer = 0;
                blocking = false;
            }
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
