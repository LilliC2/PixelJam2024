using UnityEngine;

using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;


public class DisconnectUI : GameBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject panel;
    public override void OnPlayerLeftRoom(Player player)
    {
        panel.SetActive(true);
        _GM.gameState = GameManager.GameState.PlayerDisconnected;
    }

        public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");

    }
}
