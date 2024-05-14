using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : GameBehaviour
{

    PhotonView view;

    CharacterController controller;
    Rigidbody rb;
    Animator anim;

    [SerializeField] float speed;
    bool isDead;

    public bool isInReloadZone;
    bool isReloading;
    [SerializeField] Image[] ammoImages;
    [SerializeField] Sprite ammo_empty;
    [SerializeField] Sprite ammo_full;
    
    [SerializeField] float reloadSpeed;

    [SerializeField] GameObject firingPoint;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject groundCheck;
    [SerializeField] GameObject gunbarrel;

    bool isDashOnCooldown;
    [SerializeField] float dashCooldownTime;
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    bool isDashing;

    [SerializeField] int maxAmmo;
    [SerializeField] int currentAmmo;
    bool isProjectileOnCooldown;
    [SerializeField] float projectileCooldownTime;
    [SerializeField] float projectileForce;

    Vector3 mousePos;
    Plane plane = new Plane(Vector3.up, 0);


    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = 3;
        UpdateGunAmmoVisuals();
        isReloading = false;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();

        _GM.startNewRound.AddListener(ResetPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine)
        {
            //test
            if(Input.GetKey(KeyCode.O))
            {
                Die();
            }



            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(horizontal, 0, vertical);

            movement = IsoVectorConvert(movement);

            movement *= speed;



            //transform.rotation = Quaternion.LookRotation(movement);
            if (movement.magnitude > 0.05f)
            {
                gameObject.transform.forward = movement;
            }
            if (!Physics.CheckSphere(groundCheck.transform.position, 0.1f, _GM.groundMask))
            {
                movement += Physics.gravity;

            }


            #region Dash
            if (Input.GetButton("Jump"))
            {
                if (!isDashOnCooldown)
                {
                    isDashing = true;

                    ExecuteAfterSeconds(dashCooldownTime, () => isDashOnCooldown = false);
                    ExecuteAfterSeconds(dashDuration, () => isDashing = false);

                }
            }

            if (isDashing)
            {

                controller.Move(movement * dashForce * Time.deltaTime);

            }
            else if (!isDashing)
            {
                controller.Move(movement * Time.deltaTime);


            }

            #endregion

            anim.SetFloat("Speed", controller.velocity.magnitude);


            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                mousePos = ray.GetPoint(distance);
            }


            #region Shoot

            if (Input.GetButton("Fire1"))
            {
                if (currentAmmo > 0 && !isProjectileOnCooldown)
                {
                    isProjectileOnCooldown = true;

                    gameObject.transform.LookAt(new Vector3(mousePos.x, 0, mousePos.z));
                    firingPoint.transform.LookAt(new Vector3(mousePos.x, 0, mousePos.z));

                    anim.SetTrigger("Shoot");

                }
            }




            #endregion

            #region Reload

            if (isInReloadZone && !isReloading && currentAmmo < maxAmmo)
            {
                if (Input.GetKey(KeyCode.R))
                {
                    print("RELOAD call");
                    isReloading = true;
                    StartCoroutine(Reload());

                }
            }


            #endregion
        }

    }


    IEnumerator Reload()
    {
        if(currentAmmo < maxAmmo && isInReloadZone)
        {
            print("incresae ammo");
            currentAmmo++;
            UpdateGunAmmoVisuals();



            yield return new WaitForSeconds(reloadSpeed);
            StartCoroutine(Reload());

        }
        else
        {
            isReloading = false;
        }


    }

    public void PlayerShoot()
    {
        isProjectileOnCooldown = true;
        //transform.rotation = Quaternion.LookRotation(mousePos);
        //firingPoint.transform.LookAt(test.transform.position);
        gameObject.transform.LookAt(new Vector3(mousePos.x, 0, mousePos.z));
        firingPoint.transform.LookAt(new Vector3(mousePos.x, 0, mousePos.z));

        currentAmmo--;
        UpdateGunAmmoVisuals();


        var bullet = Instantiate(projectile, firingPoint.transform.position, gameObject.transform.rotation);

        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * projectileForce);

        ExecuteAfterSeconds(projectileCooldownTime, () => isProjectileOnCooldown = false);
    }

    void UpdateGunAmmoVisuals()
    {
        switch(currentAmmo)
        {
            case 0:
                gunbarrel.GetComponent<Renderer>().material = _GM.gunBarrelMaterials[0];

                ammoImages[0].sprite = ammo_empty;
                ammoImages[1].sprite = ammo_empty;
                ammoImages[2].sprite = ammo_empty;

                break;
            case 1:
                gunbarrel.GetComponent<Renderer>().material = _GM.gunBarrelMaterials[1];
                ammoImages[0].sprite = ammo_full;
                ammoImages[1].sprite = ammo_empty;
                ammoImages[2].sprite = ammo_empty;
                break;
            case 2:
                gunbarrel.GetComponent<Renderer>().material = _GM.gunBarrelMaterials[2];
                ammoImages[0].sprite = ammo_full;
                ammoImages[1].sprite = ammo_full;
                ammoImages[2].sprite = ammo_empty;
                break;
            case 3:
                gunbarrel.GetComponent<Renderer>().material = _GM.gunBarrelMaterials[3];
                ammoImages[0].sprite = ammo_full;
                ammoImages[1].sprite = ammo_full;
                ammoImages[2].sprite = ammo_full;
                break;
        }
    }

    void Die()
    {
        if(!isDead)
        {
            isDead = true;
            anim.SetTrigger("Dead");
            _GM.alivePlayers.Remove(gameObject);
            ExecuteAfterSeconds(1, () => gameObject.SetActive(false));
        }

    }
    
    void ResetPlayer()
    {
        isDead = false;
        currentAmmo = 3;
        UpdateGunAmmoVisuals();
        if (!_GM.alivePlayers.Contains(gameObject)) _GM.alivePlayers.Add(gameObject);
    }

    Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        Vector3 result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            Die();
        }
    }

}
