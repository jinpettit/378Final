using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] Animator animator;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    public Transform hitbox;
    public float attackRangeX = 1f;
    public float attackRangeY = 1f;
    private Vector2 attackSize;
    public LayerMask opponentLayers;

    private Vector3 vel = Vector3.zero;
    private bool attacking = false;

    float attackRate = 0.6f;
    float attackTimer = 0f;

    public void Move(float move){
        if (!attacking){ //don't want players to be able to move and attack at the same time
            Vector3 targetVelocity = new Vector2(move * 10f, body.velocity.y);
            body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref vel, movementSmoothing);
        }
    }

    public void Attack(){
        if (!attacking){
            attacking = true;
            animator.SetTrigger("Attack");
            Collider2D[] targets = Physics2D.OverlapBoxAll(hitbox.position, attackSize, 0, opponentLayers);
            foreach (Collider2D target in targets){
                Debug.Log("hit");
            }
        }
        
    }

    void Start(){
        attackSize = new Vector2(attackRangeX, attackRangeY);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            Attack();
        }

        if (attacking){ //artificial cooldown for the attack animation
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate){
                attackTimer = 0;
                attacking = false;

            }
        }
    }

    void OnDrawGizmosSelected(){
        if (hitbox == null){
            return;
        }
        Vector2 boxSize = new Vector2(attackRangeX, attackRangeY);
        Gizmos.DrawWireCube(hitbox.position, boxSize);
    }
}
