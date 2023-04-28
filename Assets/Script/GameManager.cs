using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public Canvas uiCanvas;
    public Canvas canvas;
    public bool isPlaying = false;
    public float targetHitPoint;
    public float totalShootPoint;
    public float hitPercent;
    public float basePlayTime;
    public float playTime;

    public UiController ui;

    //게임매니저에 접근을 쉽게 만들기 위해 싱글톤 작업 및 점수등 데이터를 초기화 하지 않기위해 파괴불가로 설정
    public static GameManager instance;
    private void Awake() {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    private void Update() {
        if(isPlaying){
            PlayTime();
        }
    }

    // R버튼을 눌리시 inputManager를 통해 값을 전달 받아서, 명중률 계산을 위해 기존의 값을 초기화해줌.
    public void GameStart(){
        if(!isPlaying){
            //플레이를 하면 기존에 가지고있는 값들을 초기화 해서 게임중 스탯을 정확히 계산하게합니다.
            canvas.gameObject.SetActive(false);
            isPlaying = true;
            playTime = basePlayTime;
            totalShootPoint = 0;
            targetHitPoint = 0;
        }
        
    }
    //게임상의 플레이 시간을 계산합니다. 게임시간이 종료되면 명중률을 계산한뒤, UIController 명령어를 수행시킵니다.
    public void PlayTime(){
        playTime -= Time.deltaTime;
        if(playTime <= 0){
            playTime = 0;
            
            isPlaying = false;
            hitPercent = targetHitPoint / totalShootPoint;
            ui.Result();

        }
    }

}
