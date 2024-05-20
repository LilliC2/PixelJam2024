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


    float minX = -18;
    float maxX = 18;
    float minZ = -18;
    float maxZ = 18;
    float minSeconds = 5;
    float maxSeconds = 15;


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

    Vector3 RandomPosInGameSpace()
    {
        Vector3 position = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        return position;

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

        for (int i = 0; i < playerGameObjList.Count; i++)
        {
            ResetPlayer(playerGameObjList[i]);
            playerGameObjList[i].GetComponent<CharacterController>().enabled = false;

            playerGameObjList[i].GetComponent<PlayerController>().isDead = false;
            playerGameObjList[i].transform.position = RandomPosInGameSpace();
            runEndOfRound = false;
            playerGameObjList[i].GetComponent<CharacterController>().enabled = true;
        }


        StartCoroutine(_InGameUI.Countdown());


    }

    void ResetPlayer(GameObject player)
    {
        var script = player.GetComponent<PlayerController>();

        print("reset player");
        script.state = PlayerState.Alive;
        player.GetPhotonView().RPC("Alive", RpcTarget.All);

        player.SetActive(true);
        script.currentAmmo = 3;
        script.UpdateGunAmmoVisuals();
        player.GetComponentInChildren<Animator>().SetBool("Dead", false);
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
            if (playerScores[i] == 5)
            {
                winnerIndex = i;
                isGameWon = true;
            }

        }

        return isGameWon;
    }

    public void WinnerLoad()
    {
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
