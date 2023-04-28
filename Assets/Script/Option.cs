using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Option : MonoBehaviour
{
    public PlayercController player;
    float volumeSize;
    public Slider soundSlider;
    public Slider sensitivitySlider;
    public float sensitivity;
    public TextMeshProUGUI soundText;
    public TextMeshProUGUI sensitivityText;
    public Image[] crosshairImage; 

    public Slider rSilder;
    public Slider gSilder;
    public Slider bSilder;
    float rColor;
    float gColor;
    float bColor;

    Color crosshairColor;
    int selectCrosshair;


    private void Start() {
        for(int i = 0; i < crosshairImage.Length; i++){
            crosshairImage[i].sprite = GameManager.instance.crosshairSprite[i];
        }
    }
    private void Update() {
        SoundSilder();
        AudioSize();
        mauseSensitivity();
        ChangekRGB();
    }
    
    //출력이 아니라 음성을 듣는 리스러는 통해 전체 사운드 크기를 조정합니다.
    void AudioSize(){
        AudioListener.volume = volumeSize;
    }
    //슬라이드를 통해 사운드크기를 조정합니다. 슬라이드를 조작하는 순간  Text도 동기화되서 어느정도크기인지 알수있습니다.
    public void SoundSilder(){
        volumeSize = (float)soundSlider.value;
        soundText.text = volumeSize.ToString();
   }
    //슬라이드를 통해 마우스민감도를 조정합니다. 슬라이드를 조작할떄 Text도 같이 동기화되서 조정됩니다.
   public void mauseSensitivity(){
        sensitivity = (float)sensitivitySlider.value;
        sensitivityText.text = sensitivity.ToString();

        GameManager.instance.mauseSensitivity = sensitivity;
   }
   void ChangekRGB(){
        rColor = rSilder.value;
        gColor = gSilder.value;
        bColor = bSilder.value;

        crosshairColor = new Color(rColor, gColor, bColor);

        for(int i = 0; i < crosshairImage.Length; i++){
            crosshairImage[i].color = crosshairColor;
        }
   }
   public void OnSelectCrosshair(int sprite){
        selectCrosshair = sprite;
   }

   public void OnSaveButton(){
        GameManager.instance.changeCrosshair = true;
        GameManager.instance.sountSize = volumeSize;
        GameManager.instance.mauseSensitivity = sensitivity;
        GameManager.instance.crosshairColor = crosshairColor;
        GameManager.instance.chooseCrosshair = selectCrosshair;
        gameObject.SetActive(false);
        player.ToggleCursor(false);
        
   }
   public void OnCancelButton(){
        gameObject.SetActive(false);
        player.ToggleCursor(false);
        AudioListener.volume = GameManager.instance.sountSize;
        sensitivitySlider.value = GameManager.instance.mauseSensitivity;

   }
}
