using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public GameObject resultCanvas;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI ShootText;
    public TextMeshProUGUI PercentText;
    PlayercController player;

    public Sprite[] awardSprite;
    public Image awardImage;

    float playTime;
    float maxPlayTime = 10f;

    private void Start() {
        player = FindObjectOfType<PlayercController>().GetComponent<PlayercController>();
        resultCanvas.SetActive(false);
        GameManager.instance.basePlayTime = maxPlayTime;
        playTime = maxPlayTime;
    }
    void Update()
    {
        CanvasControll();
        if(GameManager.instance.isPlaying){
            PlayTime();
        }
    }

    //캔버스 위쪽에서 남은 게임시간을 보여줍니다.
    public void CanvasControll(){
        if(!GameManager.instance.isPlaying){
            //GameManager에서 설정된 게임시간을 동기화합니다.
            timerText.text = "Timer : " + maxPlayTime;
        }
        else if(GameManager.instance.isPlaying){
            //게임이 실행되면 줄어드는 시간을 GameManager에서 가져와서 동기화합니다.
          timerText.text = "Timer : " + (float)Math.Round(playTime, 2); 
        }
    }

    public void Result(){
        //PlayerController에서 만들어둔 커서토글 스크립트를 작동해서 커서락을 풀어주고, 화면을 고정시킵니다.
        player.ToggleCursor(true);
        
        resultCanvas.SetActive(true);

        //GameManager가 가지고있는 맞춘 횟수와 총발사 횟수를 가져와서 켄버스 텍스트를 교체합니다.
        hitText.text = "Hit : " + GameManager.instance.targetHitPoint;
        ShootText.text ="Shoot : " + GameManager.instance.totalShootPoint;

        //GameManager가 가지고있는 사용횟수가 많은 명중률을 변수로 만들어줍니다.
        float percent = GameManager.instance.hitPercent * 100;
        
        //명중률을 미리 지정해둔 명중률 텍스트와 교체해줍니다.
        PercentText.text = "Percent : " + (float)Math.Round(percent, 1) + "%";

        // 명중률에 따라 지정된 스프라이트를 이미지에 넣어주고, 색상을 변경해줍니다.
        if(percent >= 90f){
            awardImage.sprite = awardSprite[0];
            awardImage.color = new Color(255, 185, 0);
        }
        else if(percent < 90 && percent >= 80){
            awardImage.sprite = awardSprite[1];
            awardImage.color = Color.white;
        }
        else if(percent < 80 && percent >= 70){
            awardImage.sprite = awardSprite[2];
            awardImage.color = new Color(255, 80, 0);
        }
        else if(percent < 70) {
            awardImage.sprite = awardSprite[3];
            awardImage.color = new Color(120,120,120);
        }
    }

    //게임상의 플레이 시간을 계산합니다. 게임시간이 종료되면 명중률을 계산한뒤, UIController 명령어를 수행시킵니다.
    public void PlayTime(){
        playTime -= Time.deltaTime;
        if(playTime <= 0){
            playTime = 0;
            GameManager.instance.isPlaying = false;
            GameManager.instance.hitPercent = GameManager.instance.targetHitPoint / GameManager.instance.totalShootPoint;
            Result();
        }
    }

    public void OnAgainBTN(){
        // Again 버튼을 눌리시 게임씬을 다시 불려와서 처음부터 시작하게합니다.
        player.ToggleCursor(false);
        string name = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
    }
    
    public void OnExitBTN(){
        // 종료버튼을 눌리시 메인메뉴씬으로 로드합니다.
        player.ToggleCursor(false);
        SceneManager.LoadScene(0);
    }
}
