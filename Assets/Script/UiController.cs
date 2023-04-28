using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    public GameObject resultCanvas;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI ShootText;
    public TextMeshProUGUI PercentText;
    public PlayercController player;


    private void Start() {
        resultCanvas.SetActive(false);
    }
    void Update()
    {
        CanvasControll();
    }

    public void CanvasControll(){
        if(!GameManager.instance.isPlaying){
            timerText.text = "Timer : " + GameManager.instance.basePlayTime;
        }
        else if(GameManager.instance.isPlaying){
          timerText.text = "Timer : " + (float)Math.Round(GameManager.instance.playTime, 2); 
        }
    }

    public void Result(){
        player.ToggleCursor(true);
        resultCanvas.SetActive(true);
        hitText.text = "Hit : " + GameManager.instance.targetHitPoint;
        ShootText.text ="Shoot : " + GameManager.instance.totalShootPoint;
        PercentText.text = "Percent : " + (float)Math.Round(GameManager.instance.hitPercent * 100f, 1) + "%";
    }

    public void OnAgainBTN(){
        SceneManager.LoadScene(1);
    }
    
    public void OnExitBTN(){
        SceneManager.LoadScene(0);
    }
}
