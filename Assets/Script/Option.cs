using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Option : MonoBehaviour
{
    public PlayercController player;
    public float volumeSize;
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

    public Color crosshairColor;
    public int selectCrosshair;

    public GameObject buttons;
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

    //SetActive가 false에서 true가 되는 순간 작동합니다.
    private void OnEnable() {
        Reset();
    }

    // 설정창에 들어갈때 가존에 설정된값을 그대로 다시 적용시켜줘서, 설정을 할때마다 기존 저장값이 망가지는걸 방지합니다.
    void Reset(){

        //GameManger가 가지고있는 설정값들을 가지고옵니다.
        volumeSize = GameManager.instance.sountSize * 100;
        sensitivity = GameManager.instance.mauseSensitivity;
        crosshairColor = GameManager.instance.crosshairColor;
        selectCrosshair = GameManager.instance.chooseCrosshair;

        rColor = crosshairColor.r;
        gColor = crosshairColor.g;
        bColor = crosshairColor.b;

        //설정창에 기본값대신 가져온값을 적용시켜줍니다.
        rSilder.value = rColor;
        gSilder.value = gColor;
        bSilder.value = bColor;

        soundSlider.value = volumeSize;
        sensitivitySlider.value = sensitivity;
    }
    
    //출력이 아니라 음성을 듣는 리스러는 통해 전체 사운드 크기를 조정합니다.
    void AudioSize(){
        //AudioLisener는 1이 최대값이나 그이상의 숫자는 증폭되서 소리가 깨지게됩니다. SoundSilder는 0~100을 기준으로하니 문제가 생길수 있기에 100을 나누어줍니다.
        AudioListener.volume = volumeSize / 100;
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
   }

   // 크로스헤어 색상 값입니다. Update를 통해 실시간으로 플레이어가 변경되는 색상을 확인할수 있습니다.
   void ChangekRGB(){
        rColor = rSilder.value;
        gColor = gSilder.value;
        bColor = bSilder.value;

        crosshairColor = new Color(rColor, gColor, bColor);

        for(int i = 0; i < crosshairImage.Length; i++){
            crosshairImage[i].color = crosshairColor;
        }
   }
   //버튼에 연결되어있습니다. 버튼을 클릭시 미리 지정된 스프라이트 번호를 알려주어, 다른 씬으로 이동할때도 크로스헤어를 설정된 값으로 교체해줍니다
   public void OnSelectCrosshair(int sprite){
        selectCrosshair = sprite;
   }

    //변경된 값을 GameManager에 저장해줍니다. 이후 옵션캔버스를 끄고 화면을 움직일수있게 합니다. 미리꺼두었던 메인메뉴의 버튼들도 활성화 시킵니다.
   public void OnSaveButton(){
        buttons.SetActive(true);
        GameManager.instance.changeCrosshair = true;
        GameManager.instance.sountSize = volumeSize / 100;
        GameManager.instance.mauseSensitivity = sensitivity;
        GameManager.instance.crosshairColor = crosshairColor;
        GameManager.instance.chooseCrosshair = selectCrosshair;
        gameObject.SetActive(false);
        player.ToggleCursor(false);

   }

   //설정에서 변경된 값들을 GameManger에 저장된 값을 통해 원상복구 시킵니다. 옵션캔버스를 끄고 화면을 움직일수 있게합니다.
   public void OnCancelButton(){
        buttons.SetActive(true);
        
        gameObject.SetActive(false);
        player.ToggleCursor(false);
        AudioListener.volume = GameManager.instance.sountSize;
        sensitivitySlider.value = GameManager.instance.mauseSensitivity;
   }
}
