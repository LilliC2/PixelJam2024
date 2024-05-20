using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class WaterProjectile : GameBehaviour
{
    [SerializeField] float speed;
    Rigidbody rb;
    Animator anim;
    bool isMoving;
    bool destroy;

    public GameObject parent;

    [SerializeField] ParticleSystem popPS;

    [SerializeField] GameObject reloadzone;

    [SerializeField] AudioSource[] hit;

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
        if(rb == null) rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        _GM.shootManager.GetComponent<PhotonView>().RPC("SpawnReloadPuddle", RpcTarget.All, transform.position);
        hit[Random.Range(0, hit.Length)].Play();
       
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

        if(collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject != parent)
            {
                collision.gameObject.GetComponent<PhotonView>().RPC("Die", RpcTarget.All);
                if (!destroy)
                {
                    destroy = true;
                    DestroyProjectile();

                }
            }
      
        }

    }


}
