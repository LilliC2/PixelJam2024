using Photon.Pun;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ShootManager : MonoBehaviourPunCallbacks
{
    public bool isShootButtonPressed;

    [SerializeField] GameObject projectile;
    [SerializeField] GameObject reloadzone;
    [SerializeField] float projectileForce;

    
    [PunRPC]
    public void SpawnReloadPuddle(Vector3 pos)
    {
        Instantiate(reloadzone, pos, Quaternion.identity);
    }

    [PunRPC]
    public void SpawnBullet(Quaternion player, Vector3 firingPoint)
    {
        var bullet = Instantiate(projectile, firingPoint, player);

        bullet.GetComponent<Rigidbody>().AddForce(new Vector3(player.x,player.y,player.z) * projectileForce);
    }
}
