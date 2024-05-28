using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Movement : MonoBehaviour
{
    [SerializeField] public Player1Controller controller;
    [SerializeField] Animator animator;
    public float moveSpeed = 40f;
    float horizontalMove = 0f;


    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

        bool isMoving = !Mathf.Approximately(horizontalMove, 0f);

        animator.SetBool("isWalking", isMoving);
    }

    void FixedUpdate(){
        controller.Move(horizontalMove * Time.fixedDeltaTime);
    }
}
