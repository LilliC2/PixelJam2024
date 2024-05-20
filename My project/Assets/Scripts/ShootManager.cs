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
        Vector3 newPos = new Vector3(pos.x, 0.49f, pos.z);
        Instantiate(reloadzone, newPos, Quaternion.identity);
    }

    [PunRPC]
    public void SpawnBullet(Quaternion playerQuaternion, Vector3 firingPoint, GameObject player)
    {
        var bullet = Instantiate(projectile, firingPoint, playerQuaternion);

        bullet.GetComponent<Rigidbody>().AddForce(new Vector3(playerQuaternion.x,playerQuaternion.y,playerQuaternion.z) * projectileForce);

        bullet.GetComponent<WaterProjectile>().parent = player;
    }
}
