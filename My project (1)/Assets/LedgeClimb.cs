using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimb : MonoBehaviour
{
    private bool greenBox, redBox;
    public float redXOffset, redYOffset, redXsize, redYsize, greenXOffset, greenYOffset, greenXsize, greenYsize;
    private Rigidbody2D rb;
    private Animator anim;
    private float startingGrav;
    public LayerMask groundMask;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startingGrav = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYsize), 0f);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYOffset), new Vector2(redXsize, redYsize), 0f);

        if (greenBox && !redBox && !PlayerVariable.isGrabbing && PlayerVariable.isJumping)
        {
            PlayerVariable.isGrabbing = true;

        }

        if (PlayerVariable.isGrabbing)
        {
            PlayerVariable.isGrabbing = false;
            anim.SetBool("LedgeClimb", true);
            StartCoroutine(ClimbUp());
            rb.gravityScale = startingGrav;
            PlayerVariable.isGrabbing = false;
            StartCoroutine(DisableClimbAnimation());
        }
    }

    IEnumerator ClimbUp()
    {
        PlayerVariable.isGrabbing = false;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        yield return new WaitForSeconds(0.001f);
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
        Debug.Log(transform.position.x + 0.2f + transform.position.y + 0.2f);
        //transform.position = new Vector2(transform.position.x + (0.2f), transform.position.y + 0.2f);
    }

    IEnumerator DisableClimbAnimation()
    {
        yield return new WaitForSeconds(0.01f);
        anim.SetBool("LedgeClimb", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYOffset), new Vector2(redXsize, redYsize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXsize, greenYsize));
    }
}
