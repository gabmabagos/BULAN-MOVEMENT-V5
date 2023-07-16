using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject brokenbit;

    public GameObject FixedCrate;
    public Vector2 Target;

    void Start()
    {
        Target = new Vector2(brokenbit.transform.position.x, FixedCrate.transform.position.y);
        //.Log(Target);

        Target = new Vector2(FixedCrate.transform.position.x, FixedCrate.transform.position.y);
        //Debug.Log(Target);
        
    }

    void OntriggerEnter2D(Collider2D other){

        if (other.tag == "Projectile")
                BreakIt();
    } 
    void OnCollisionEnter2D(Collision2D col){

        if(col.collider.tag=="Projectile"){
            BreakIt();  
        }

    }
    public void BreakIt(){


        Destroy(this.gameObject);
        GameObject broke =(GameObject)
        Instantiate (brokenbit, Target + new Vector2(1, 0), Quaternion.identity);
        //Debug.Log(Target);
        //Instantiate (brokenbit, transform.position, Quaternion.identity);
        foreach (Transform child in broke.transform){


            child.GetComponent<Rigidbody2D>().velocity= new Vector2(Random.Range(-2f,2f),Random.Range(3f,7f));
        }
    }
}
