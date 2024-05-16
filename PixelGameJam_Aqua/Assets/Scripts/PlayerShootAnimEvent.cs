using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootAnimEvent : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] GameObject footprint;
    [SerializeField] GameObject Lfoot;
    [SerializeField] GameObject Rfoot;
    int count = 1;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void CallShoot()
    {
        playerController.PlayerShoot();
    }    

    public void MakeFootPrint()
    {
        print("make foot");
        if(count % 2 == 0) Instantiate(footprint, Lfoot.transform.position, Lfoot.transform.rotation);
        if(count % 2 != 0) Instantiate(footprint, Rfoot.transform.position, Rfoot.transform.rotation);
        count++;

    }
}
