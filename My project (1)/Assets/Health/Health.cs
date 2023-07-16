using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float currentHealth {get; private set;}
    private Animator anim;
    private bool dead;

    public Rigidbody2D playerRB;

    public int timer = 0;

    private void Awake(){

        currentHealth= startingHealth;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage ,0 , startingHealth);
        if(currentHealth > 0){
            anim.SetTrigger("hurt");
        }
        else{
            

            if(!dead){
                anim.SetTrigger("die");
                playerRB.velocity = Vector2.right * 0;
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }

            else GetComponent<PlayerMovement>().enabled = true;
        }
    }
    
}
