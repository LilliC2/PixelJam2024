using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class InGameUIManager : Singleton<InGameUIManager>
{
    [SerializeField] GameObject[] playerPointPanels;

    [SerializeField] Sprite[] fullPointBall_sprites;

    [SerializeField] Image[] greenPoints_Images;
    [SerializeField] Image[] bluePoints_Images;
    [SerializeField] Image[] pinkPoints_Images;
    [SerializeField] Image[] orangePoints_Images;

    [SerializeField] RectTransform panelBeforePos;
    [SerializeField] 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //turn off points for players that arent in game
        for (int i = 0; i < _GM.playerGameObjList.Count; i++)
        {
            playerPointPanels[i].SetActive(true);
        }
    }


    public void RoundEndAnimation()
    {
        Time.timeScale = 0.25f;

        ExecuteAfterSeconds(2, () => );
    }

    public void UpdateScoreboard()
    {
        for (int i = 0; i < greenPoints_Images.Length; i++)
        {
            if (i < _GM.playerScores[0]) greenPoints_Images[i].sprite = fullPointBall_sprites[0];
            if (i < _GM.playerScores[1]) bluePoints_Images[i].sprite = fullPointBall_sprites[1];
            if (i < _GM.playerScores[2]) pinkPoints_Images[i].sprite = fullPointBall_sprites[2];
            if (i < _GM.playerScores[3]) orangePoints_Images[i].sprite = fullPointBall_sprites[3];
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
