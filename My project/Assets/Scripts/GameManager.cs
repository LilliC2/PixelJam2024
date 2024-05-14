using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using System.Linq;
public class GameManager : Singleton<GameManager>
{

    public LayerMask groundMask;

    public List<GameObject> playerGameObjList;
    [SerializeField] List <GameObject> spawnpoints;
    [SerializeField] GameObject[] playerPrefabs;

    public Material[] gunBarrelMaterials;


    int totalPlayerCount;

    public List<GameObject> alivePlayers;

    int[] playerScores;

    public UnityEvent startNewRound;

    [SerializeField] GameObject testText;

    // Start is called before the first frame update
    void Start()
    {
        startNewRound.AddListener(FindSpawnPoints);

        FindSpawnPoints();
        CreatePlayer();
     
        var players = Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            playerGameObjList.Add(player.gameObject);
        }

        Camera.main.GetComponent<CameraController>().GetTargets();

        playerScores = new int[playerGameObjList.Count];
    }


    void CreatePlayer()
    {
        int pNum = playerGameObjList.Count;
        var spawnPoint = spawnpoints[Random.Range(0, 3)];
        var playerObj = PhotonNetwork.Instantiate(playerPrefabs[pNum].name, spawnPoint.transform.position, Quaternion.identity);

        alivePlayers.Add(playerObj);
        spawnpoints.Remove(spawnPoint);
    }

    void RemovePlayer()
    {
    }

    void FindSpawnPoints()
    {
        spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();

    }

    // Update is called once per frame
    void Update()
    {
        if(alivePlayers.Count == 1)
        {
            //show points
            testText.SetActive(true);
        }
    }

}
