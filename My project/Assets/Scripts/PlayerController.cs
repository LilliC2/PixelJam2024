using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : GameBehaviour
{

    PhotonView view;

    public int playerIndex;

    ShootManager shootManager;

    CharacterController controller;
    Rigidbody rb;
    Animator anim;

    [SerializeField] float speed;
    public bool isDead;

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
    public int currentAmmo;
    public bool isProjectileOnCooldown;
    [SerializeField] float projectileCooldownTime;
    [SerializeField] float projectileForce;

    Vector3 mousePos;
    Plane plane = new Plane(Vector3.up, 0);

    bool isShootButtonPressed = false;

    [SerializeField] AudioSource deathAudio;
    [SerializeField] AudioSource reloadAudio;
    [SerializeField] AudioSource shootAudio;

    [SerializeField] AudioSource dashAudio;



    public enum PlayerState { Alive, Dead}
    public PlayerState state;   

    // Start is called before the first frame update
    void Start()
    {
        if(!_GM.playerGameObjList.Contains(gameObject)) _GM.playerGameObjList.Add(gameObject);
        if(!_GM.alivePlayers.Contains(gameObject)) _GM.alivePlayers.Add(gameObject);

        currentAmmo = 3;
        UpdateGunAmmoVisuals();
        isReloading = false;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();

        shootManager = FindAnyObjectByType<ShootManager>();

    }

    // Update is called once per frame
    void Update()
    {

        if(view.IsMine)
        {

            if(state == PlayerState.Alive && _GM.gameState == GameManager.GameState.Playing)
            {
                anim.SetBool("Dead", false);



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
                        dashAudio.Play();
                        ExecuteAfterSeconds(dashCooldownTime, () => isDashOnCooldown = false);
                        ExecuteAfterSeconds(dashDuration, () => isDashing = false);

                    }
                }

                if (isDashing)
                {

                    if (controller.enabled) controller.Move(movement * dashForce * Time.deltaTime);

                }
                else if (!isDashing)
                {
                    if (controller.enabled) controller.Move(movement * Time.deltaTime);


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


                if (Input.GetButtonDown("Fire1"))
                    isShootButtonPressed = true;

                if (Input.GetButton("Fire1"))
                {
                    if (currentAmmo > 0 && !isProjectileOnCooldown)
                    {
                        isProjectileOnCooldown = true;
                        shootAudio.Play();
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

    }

    

    IEnumerator Reload()
    {
        if(currentAmmo < maxAmmo && isInReloadZone)
        {
            print("incresae ammo");
            currentAmmo++;
            UpdateGunAmmoVisuals();

            reloadAudio.Play();

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

        Quaternion quaternion = new Quaternion(gameObject.transform.forward.x, gameObject.transform.forward.y, gameObject.transform.forward.z, 0);

        shootManager.GetComponent<PhotonView>().RPC("SpawnBullet", RpcTarget.All, quaternion,firingPoint.transform.position, _GM.playerGameObjList.IndexOf(gameObject));

        gameObject.transform.LookAt(new Vector3(mousePos.x, 0, mousePos.z));
        firingPoint.transform.LookAt(new Vector3(mousePos.x, 0, mousePos.z));

        currentAmmo--;
        UpdateGunAmmoVisuals();




        ExecuteAfterSeconds(projectileCooldownTime, () => isProjectileOnCooldown = false);
    }

    


    public void UpdateGunAmmoVisuals()
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

    [PunRPC]
    void Die()
    {
        if(state == PlayerState.Alive)
        {
            _GM.alivePlayers.Remove(gameObject);

            deathAudio.Play();
            state = PlayerState.Dead;
            if(!isDead)
            {
                isDead = true;
                anim.SetBool("Dead", true);

            }
            _GM.CheckForEndOfRound();
            ExecuteAfterSeconds(1, () => gameObject.SetActive(false));


        }

    }


    [PunRPC]
    public void Alive()
    {
        anim.SetBool("Dead", false);

    }


    Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        Vector3 result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }



}
