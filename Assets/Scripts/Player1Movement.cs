using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Movement : MonoBehaviour
{
    [SerializeField] public Player1Controller controller;
    public float moveSpeed = 40f;
    float horizontalMove = 0f;


    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
    }

    void FixedUpdate(){
        controller.Move(horizontalMove * Time.fixedDeltaTime);
    }
}
