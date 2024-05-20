using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;



public class EndGameUIManager : GameBehaviour
{
    PhotonView view;

    [SerializeField] GameObject restartPanel;
    [SerializeField] GameObject hostPanel;
    [SerializeField] GameObject guestPanel;
    [SerializeField] TMP_Text countdownText;

    [SerializeField] GameObject[] winnerHeads;
    [SerializeField] GameObject[] loserBodies;

   [SerializeField] Sprite[] audio_sprites;
    [SerializeField] Image audioImage;
    bool isMuted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        view = GetComponent<PhotonView>();
        restartPanel.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        { 
            hostPanel.SetActive(true);
            guestPanel.SetActive(false);
        } 
        else
        {
            guestPanel.SetActive(true);
            hostPanel.SetActive(false);
        }

        gameObject.GetComponent<PhotonView>().RPC("UpdateWinner", RpcTarget.All,_GM.winnerIndex);

    }

    [PunRPC]
    public void UpdateWinner(int winnderIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < winnerHeads.Length; i++)
            {
                if (i == winnderIndex)
                {
                    winnerHeads[i].gameObject.SetActive(true);
                    loserBodies[i].gameObject.SetActive(false);
                }
                else
                {
                    winnerHeads[i].gameObject.SetActive(false);
                    loserBodies[i].gameObject.SetActive(true);
                }
            }
        }

    }


    public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom(false);

    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");

        base.OnLeftRoom();
    }

    public void CallCoundownButton()
    {
        view.RPC("StartCountdown", RpcTarget.All, transform.position);
    }

    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("GameScene"); //use when loading multiplayer scene
    }

    [PunRPC]
    public void StartCountdown()
    {
        restartPanel.SetActive(true);

        countdownText.text = "3";
        ExecuteAfterSeconds(1, () => countdownText.text = "2");
        ExecuteAfterSeconds(1, () => countdownText.text = "1");


        if (PhotonNetwork.IsMasterClient) {RestartGame();} 
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

