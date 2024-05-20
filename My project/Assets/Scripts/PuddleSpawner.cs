using UnityEngine;

public class PuddleSpawner : GameBehaviour
{
    float minX = -18;
    float maxX = 18;
    float minZ = -18;
    float maxZ = 18;
    float minSeconds = 5;
    float maxSeconds = 15;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPuddle();
    }

    // Update is called once per frame
    public void SpawnPuddle()
    {
        if(_GM.gameState == GameManager.GameState.Playing)
        {
            Vector3 position = new Vector3(Random.Range(minX,maxX), 0.49f,Random.Range(minZ,maxZ));

            _GM.shootManager.SpawnReloadPuddle(position);

            ExecuteAfterSeconds(Random.Range(minSeconds,maxSeconds),()=> SpawnPuddle());
        }
        
    }
}
