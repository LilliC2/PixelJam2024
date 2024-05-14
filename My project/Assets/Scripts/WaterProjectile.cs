using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterProjectile : GameBehaviour
{
    [SerializeField] float speed;
    Rigidbody rb;
    Animator anim;
    bool isMoving;
    bool destroy;
    [SerializeField] ParticleSystem popPS;
    [SerializeField] GameObject reloadzone;

    public void PopParticle()
    {
        popPS.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.AddForce(transform.forward * speed);
    }

    void DestroyProjectile()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Instantiate(reloadzone, transform.position, Quaternion.identity);
       
       ExecuteAfterFrames(10,() =>  anim.SetTrigger("Pop"));
       ExecuteAfterSeconds(0.75f,() =>  Destroy(gameObject));

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Environment"))
        {
            if(!destroy)
            {
                destroy = true;
                DestroyProjectile();

            }

        }
    }


}
