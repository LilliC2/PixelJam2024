using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager= FindAnyObjectByType<GameManager>();
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        base.OnPlayerLeftRoom(otherPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
