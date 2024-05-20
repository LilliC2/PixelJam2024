using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using System.Collections;
using TMPro;

public class InGameUIManager : Singleton<InGameUIManager>
{
    [SerializeField] GameObject[] playerPointPanels;


    [SerializeField] Sprite[] fullPointBall_sprites;
    [SerializeField] Sprite[] audio_sprites;
    [SerializeField] Image audioImage;
    bool isMuted = false;
    [SerializeField] Image[] greenPoints_Images;
    [SerializeField] Image[] bluePoints_Images;
    [SerializeField] Image[] pinkPoints_Images;
    [SerializeField] Image[] orangePoints_Images;

    [SerializeField] GameObject panelBeforePos;
    [SerializeField] GameObject panelAfterPos;
    [SerializeField] GameObject pointPanel;

    [SerializeField] GameObject playerLeftText;
    [SerializeField] GameObject countdownGO;
    [SerializeField] TMP_Text countdownText;

    [SerializeField] Ease panelEase;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdownGO.SetActive(false);

        _GM.startNewRound.AddListener(ResetUIForNewRound);

        pointPanel.transform.position = panelAfterPos.transform.position;
        //turn off points for players that arent in game
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


    public IEnumerator Countdown()
    {
        print("countdown");
        countdownGO.SetActive(true);
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";

        yield return new WaitForSeconds(1);
        countdownText.text = "1";

        yield return new WaitForSeconds(1);
        countdownText.text = "GO!";

        yield return new WaitForSeconds(1);
        _GM.gameState = GameManager.GameState.Playing;
        countdownGO.SetActive(false);


    }

    public void PlayerLeftRoom()
    {

    }

    public void SetPlayerPoints()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            print("turn on " + playerPointPanels[i]);
            playerPointPanels[i].SetActive(true);
        }
    }

    public void RoundEndAnimation()
    {

        ExecuteAfterSeconds(1, () => pointPanel.transform.DOMove(panelBeforePos.transform.position,0.1f).SetEase(panelEase));
        ExecuteAfterSeconds(1.3F, () => UpdateScoreboard());

        ExecuteAfterSeconds(1.3F, () => _AM.WinRoundAudio());

        if(!_GM.CheckForWinner())
        {
            print("start new round");
            ExecuteAfterSeconds(2F, () => _GM.startNewRound.Invoke());

        }
        else ExecuteAfterSeconds(2F, () => _GM.endGame.Invoke());

    }

    public void ResetUIForNewRound()
    {
        pointPanel.transform.DOMove(panelAfterPos.transform.position, 0.1f).SetEase(panelEase);
    }

    public void UpdateScoreboard()
    {
        for (int i = 0; i < _GM.playerGameObjList.Count; i++)
        {
            //check that
            for (int j = 0; j < _GM.playerScores[i]; j++)
            {
                if(i==0) greenPoints_Images[j].sprite = fullPointBall_sprites[i];
                if(i==1) bluePoints_Images[j].sprite = fullPointBall_sprites[i];
                if(i==2) pinkPoints_Images[j].sprite = fullPointBall_sprites[i];
                if(i==3) orangePoints_Images[j].sprite = fullPointBall_sprites[i];
            }
        }




    }

  
}
