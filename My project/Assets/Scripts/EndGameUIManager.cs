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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        view = GetComponent<PhotonView>();
        restartPanel.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        { 
            hostPanel.SetActive(true);
            guestPanel.SetActive(true);
        } 
        else
        {
            guestPanel.SetActive(true);
            hostPanel.SetActive(false);
        }


        UpdateWinner(_GM.winnerIndex);
    }

    public void UpdateWinner(int winnderIndex)
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


    public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");

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

}

