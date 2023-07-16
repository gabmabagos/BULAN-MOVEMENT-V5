//PlayerMovement
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    //idk
    private BoxCollider2D coll;
    
    //movement
    private float dirX = 0f;
    public float horizontalMove = 0f;
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float jumpForce = 14;

    //dash
    public bool FacingRight=true;
    public bool canDash = true;
    public bool isDashing;
    public bool isWallSliding;

    private float wallSlidingSpeed = 2f;
    public float dashingPower = 24f;
    public float dashingTime =1f; //0.2f
    public float dashingCooldown =1f;

    //wall jump
    public bool  isWallJumping;
    public float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;

    //idk
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheck;

    //shield
    public bool shielded;
    [SerializeField] private GameObject shield;

	//firing
	public static bool isFiring = false;
	public GameObject PseudoBulletProjectile;
	public static bool dirFire = true;
	public float fireRate = 1f;
	public float fireRateTimer = 0f;


	private void Start() {
        shielded = false;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update(){

        isShielded();
        WallSlide();
        WallJump();

        if(isDashing)
        {
            return;
        }

        dirX = Input.GetAxisRaw("Horizontal");
        horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(Input.GetKeyDown("z") && canDash)
        {
            StartCoroutine(Dash());
            anim.SetTrigger("dash");
        }

		if (horizontalMove > 0f)
		{
			anim.SetInteger("state", 1);
		}

		else if (horizontalMove < 0f)
		{
			anim.SetInteger("state", 1);
		}

		else if (IsGrounded())
		{
			anim.SetInteger("state", 4);
		}

		else
		{
			anim.SetInteger("state", 0);
		}

		//7 12 added script (firing)
		fireRateTimer += Time.deltaTime;

		if (Input.GetKeyDown("j") && dirFire && fireRateTimer >= fireRate)
		{
            anim.SetTrigger("CombatRanged");
			isFiring = true;
            StartCoroutine(DelayFire());
            /*Instantiate(PseudoBulletProjectile, new Vector2(rb.transform.position.x + 2, rb.transform.position.y), transform.rotation);
            isFiring = false;
			fireRateTimer = 0f;*/
		}
		else if (Input.GetKeyDown("j") && !dirFire && fireRateTimer >= fireRate)
		{
            anim.SetTrigger("CombatRanged");
			isFiring = true;
            StartCoroutine(DelayFire());
            /*Instantiate(PseudoBulletProjectile, new Vector2(rb.transform.position.x - 2, rb.transform.position.y), transform.rotation);
			isFiring = false;
			fireRateTimer = 0f;*/
		}
        /*
        if(Input.GetKeyDown("k") && !canDash)
        {
            if(dirFire)
            {
                Instantiate(PseudoBulletProjectile, new Vector2(rb.transform.position.x + 1, rb.transform.position.y), transform.rotation);
                isFiring = false;
                fireRateTimer = 0f;
            }
            else
            {
                Instantiate(PseudoBulletProjectile, new Vector2(rb.transform.position.x - 1, rb.transform.position.y), transform.rotation);
                isFiring = false;
                fireRateTimer = 0f;
            }
        }
        */
		UpdateAnimationState();
    }

    private IEnumerator Dash(){

        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if(!FacingRight){
            rb.velocity = new Vector2(-13f,0f);
        }

        else{
            rb.velocity = new Vector2(13f,0f);
        }

        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;

    }

    public void isShielded()
    {
        if(Input.GetKeyDown("e")&&!shielded)
        {
            anim.SetBool("Shield",true);
            shield.SetActive(true);
            shielded = true;
            Invoke("notShielded", 3f);
        }

    }

    public void notShielded()
    {
        shield.SetActive(false);
        anim.SetBool("Shield",false);
        shielded = false;
    }

    private void UpdateAnimationState()
    {
		if (dirX > 0 && !FacingRight&&!isWallJumping)
        {
            FacingRight = !FacingRight;
		    transform.Rotate (0f,180f,0f);
            dirFire = true;
        }

        else if (dirX < 0 &&FacingRight&&!isWallJumping)
        {  
            FacingRight = !FacingRight;
		    transform.Rotate (0f,180f,0f);
            dirFire = false;
        }


        if (rb.velocity.y > 0.1)
        {
            PlayerVariable.isJumping=true;
            anim.SetInteger("state", 2);
        }

        else if (rb.velocity.y < -0.1)
        {
            anim.SetInteger("state", 3);
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position,0.2f,wallLayer);
    }

    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = - transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {

            wallJumpingCounter = -Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")&&wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection*8f,8f);
            wallJumpingCounter = 0f;

            if(transform.localScale.x !=wallJumpingDirection)
            {

                FacingRight = !FacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *=-1f;
                transform.localScale =localScale;
            }
            Invoke(nameof(StopWallJumping),wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping= false;
    }

    private void WallSlide()
    {
        if(IsWalled()&& !IsGrounded()&& horizontalMove != 0f)
        {
            isWallSliding= true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed,float.MaxValue));
            if(isWallSliding == true) {anim.SetTrigger("WallSliding");}

        }

        else
        {
            isWallSliding = false;
        }

    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }

    private IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(0.25f);
        if(dirFire && fireRateTimer >= fireRate)
        {
            Instantiate(PseudoBulletProjectile, new Vector2(rb.transform.position.x + 1, rb.transform.position.y), transform.rotation);
            isFiring = false;
			fireRateTimer = 0f;
        }
        else
        {
            Instantiate(PseudoBulletProjectile, new Vector2(rb.transform.position.x - 1, rb.transform.position.y), transform.rotation);
			isFiring = false;
			fireRateTimer = 0f;
        }
    }

}