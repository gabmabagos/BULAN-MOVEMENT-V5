using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsuedoBullet : MonoBehaviour
{
    public GameObject PseudoBullet;
    private Rigidbody2D bulletrb;
    public float fireSpeed = 20f;

    private void Start()
    {
        bulletrb = GetComponent<Rigidbody2D>();
        if (PlayerMovement.dirFire == true) bulletrb.velocity = Vector2.right * fireSpeed;
        else bulletrb.velocity = Vector2.left * fireSpeed;
        Destroy(gameObject, 20f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Breakable") || other.CompareTag("Shield"))// || other.CompareTag("Untagged"))
        {
            bulletrb.velocity = Vector2.right * 0;
            bulletrb.transform.position = new Vector2(bulletrb.transform.position.x , bulletrb.transform.position.y);
            Destroy(gameObject, 1f);
        }
    }
}