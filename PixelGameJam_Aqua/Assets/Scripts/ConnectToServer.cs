using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }

    //when connected
    public override void OnConnectedToMaster()
    {
        //called when conencted to server
        PhotonNetwork.JoinLobby();

        base.OnConnectedToMaster();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
        base.OnJoinedLobby();
    }

}
