using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    private Vector3 vel = Vector3.zero;
    private bool attacking = false;

    public void Move(float move){
        if (!attacking){
            Vector3 targetVelocity = new Vector2(move * 10f, body.velocity.y);
            body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref vel, movementSmoothing);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
