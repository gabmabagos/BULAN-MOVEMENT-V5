using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float Speed = 10f;
    public Rigidbody2D rb;
    // Update is called once per frame
   private void Start()
    {
        rb.velocity = transform.right * Speed;
    }


    private void OnCollisionEnter2D(Collision2D collision){
        var enemy = collision.collider.GetComponent<EnemyBehaviour>();
        if (enemy){
            enemy.TakeHit(1);
        }
        Destroy(gameObject);
    }
}
