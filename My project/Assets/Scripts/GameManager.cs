using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using System.Linq;
using Photon.Realtime;
using static PlayerController;
using NUnit.Framework;

public class GameManager : Singleton<GameManager>
{

    public ShootManager shootManager;

    public enum GameState { Playing, Loading, Win, BetweenRounds, PlayerDisconnected}
    public GameState gameState;

    ShootManager shotManager;


    public LayerMask groundMask;

    public List<GameObject> playerGameObjList;
    [SerializeField] List<GameObject> spawnpoints;
    [SerializeField] GameObject[] playerPrefabs;

    public Material[] gunBarrelMaterials;


    int totalPlayerCount;

    public List<GameObject> alivePlayers;

    public List<int> playerScores;

    public UnityEvent startNewRound;
    public UnityEvent endGame;

    bool runEndOfRound;

    public int winnerIndex;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Loading;
        startNewRound.AddListener(ResetForNewRound);

        FindSpawnPoints();
        CreatePlayer();

        var player = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (var obj in player)
        {
            playerGameObjList.Add(obj.gameObject);
        }

        endGame.AddListener(WinnerLoad);

        StartCoroutine(_InGameUI.Countdown());
    }


    void CreatePlayer()
    {
        int pNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        var spawnPoint = spawnpoints[Random.Range(0, 3)];
        var playerObj = PhotonNetwork.Instantiate(playerPrefabs[pNum].name, spawnPoint.transform.position, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().playerIndex = pNum;

        playerScores.Add(0);
        alivePlayers.Add(playerObj);
        spawnpoints.Remove(spawnPoint);

        _InGameUI.SetPlayerPoints();

    }

    void FindSpawnPoints()
    {
        spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();

    }

    void ResetForNewRound()
    {

        print("new round");
        foreach (var player in playerGameObjList)
        {

            ResetPlayer(player);
            player.GetComponent<CharacterController>().enabled = false;
            var spawnPoint = spawnpoints[playerGameObjList.IndexOf(player)];

            player.GetComponent<PlayerController>().isDead = false;
            player.transform.position = spawnPoint.transform.position;
            runEndOfRound = false;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponentInChildren<Animator>().SetBool("Dead", false);


        }

        StartCoroutine(_InGameUI.Countdown());


    }

    void ResetPlayer(GameObject player)
    {
        var script = player.GetComponent<PlayerController>();

        print("reset player");
        script.state = PlayerState.Alive;
        player.SetActive(true);
        script.currentAmmo = 3;
        script.UpdateGunAmmoVisuals();
        if (!_GM.alivePlayers.Contains(player)) _GM.alivePlayers.Add(player);
    }

    void UpdateScore()
    {
        if (playerScores.Count != playerGameObjList.Count) playerScores = new List<int>(new int[playerGameObjList.Count]);

        int index = 0;
        if (alivePlayers.Count == 1)
        {
            index = alivePlayers[0].GetComponent<PlayerController>().playerIndex;

        }
         
        playerScores[index]++;
    }

    public bool CheckForWinner()
    {
        bool isGameWon = false;
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i] == 5) isGameWon = true;
        }

        return isGameWon;
    }

    public void WinnerLoad()
    {
        bool foundWinner = false;
        if(foundWinner == false)
        {
            for (int i = 0; i < playerScores.Count; i++)
            {
                if (playerScores[i] == 5)
                {
                    winnerIndex = i;
                    foundWinner = true;

                }
            }
        }


        PhotonNetwork.LoadLevel("WinScene"); //use when loading multiplayer scene
    }

    public void CheckForEndOfRound()
    {
        if (gameState == GameState.Playing)
        {
            if (alivePlayers.Count <= 1)
            {
                print("End of ROund");
                UpdateScore();
                _InGameUI.RoundEndAnimation();
                gameState = GameState.BetweenRounds;

                //check for winner

            }
        }
    }


}
