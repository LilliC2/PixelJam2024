using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    //https://www.youtube.com/watch?v=93SkbMpWCGo&t=34s

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text); //room name is input
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    //called when joined room
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene"); //use when loading multiplayer scene


        base.OnJoinedRoom();
    }
}
