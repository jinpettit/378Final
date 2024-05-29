using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2Health : MonoBehaviour
{
    public Image healthBar;
    public Player2Controller player; // Reference to the Player1Controller
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player2Controller>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = player.health / 100f;
    }
}
