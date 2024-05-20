using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

using UnityEngine.SceneManagement;
using Photon.Realtime;

public class CreateAndJoinRooms : GameBehaviour
{
    [SerializeField] Sprite[] audio_sprites;
    [SerializeField] Image audioImage;
    bool isMuted = false;

    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public GameObject hostWait;
    public GameObject guestWait;
    public GameObject createJoinScreen;
    public GameObject[] playerVisualsHost;
    public GameObject[] playerVisualsGuest;

    public TMP_Text playerConnectHost;
    public TMP_Text playerConnectGuest;
    public GameObject needMorePlayersText;
    public GameObject invalidRoomCode;


    //https://www.youtube.com/watch?v=93SkbMpWCGo&t=34s


    private void Start()
    {
        createJoinScreen.SetActive(true);
        hostWait.SetActive(false); guestWait.SetActive(false);
        needMorePlayersText.SetActive(false);
        invalidRoomCode.SetActive(false);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text); //room name is input
    }

    public void JoinRoom()
    {
        invalidRoomCode.SetActive(false);

        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        invalidRoomCode.SetActive(true);

        base.OnJoinRandomFailed(returnCode, message);
    }

    //called when joined room
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            WaitForPlayersToJoinHost();
        }
        else WaitForPlayersToJoinGuest();

        base.OnJoinedRoom();
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            needMorePlayersText.SetActive(true);
            PhotonNetwork.LoadLevel("GameScene"); //ONLY FOR TESTING


        }
        else
        {
            PhotonNetwork.LoadLevel("GameScene"); //use when loading multiplayer scene
        }


    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

    }

    public override void OnLeftRoom()
    {
        createJoinScreen.SetActive(true);
        hostWait.SetActive(false);
        guestWait.SetActive(false);
        base.OnLeftRoom();
    }

    public void WaitForPlayersToJoinHost()
    {
        createJoinScreen.SetActive(false);
        hostWait.SetActive(true);
        print("update text");
        playerConnectHost.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4 players connected";
        playerVisualsHost[PhotonNetwork.CurrentRoom.PlayerCount - 1].SetActive(true);
    }

    public void WaitForPlayersToJoinGuest()
    {
        createJoinScreen.SetActive(false);
        guestWait.SetActive(true);
        print("update text");
        playerConnectGuest.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4 players connected";
        playerVisualsGuest[PhotonNetwork.CurrentRoom.PlayerCount - 1].SetActive(true);
        playerVisualsGuest[0].SetActive(true);


    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        print("player entered room");
        needMorePlayersText.SetActive(false);
        playerConnectHost.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4 players connected";
        playerConnectGuest.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4 players connected";
        playerVisualsHost[PhotonNetwork.CurrentRoom.PlayerCount-1].SetActive(true);
        playerVisualsGuest[PhotonNetwork.CurrentRoom.PlayerCount-1].SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        playerConnectHost.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4 players connected";
        playerConnectGuest.text = PhotonNetwork.CurrentRoom.PlayerCount + "/4 players connected";
        playerVisualsHost[PhotonNetwork.CurrentRoom.PlayerCount].SetActive(false);
        playerVisualsGuest[PhotonNetwork.CurrentRoom.PlayerCount].SetActive(false);
    }

        public void MuteAudio()
    {
        isMuted = !isMuted;


        if(isMuted)
        {
                audioImage.sprite = audio_sprites[0];
                _AM.audioMixerVolumeController.SetFloat("MasterAudio", -80);
        }
        else
        {
            audioImage.sprite = audio_sprites[1];
                _AM.audioMixerVolumeController.SetFloat("MasterAudio", -0.04f);
        }
    }

}
