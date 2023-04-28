using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshiar : MonoBehaviour
{
    Image crosshiar;

    private void Awake() {
        crosshiar = GetComponent<Image>();
    }

    //옵션에서 GameManager로 값을 저장하고, 싱글톤인 게임메니저가 다양한 씬에 같은 크로스헤어 옵션을 전달합니다
    void Update()
    {
        //GameManager에 저장된 값을 적용합니다
        if(GameManager.instance.changeCrosshair){
            crosshiar.sprite = GameManager.instance.crosshairSprite[GameManager.instance.chooseCrosshair];
            crosshiar.color = GameManager.instance.crosshairColor; 
        }

    }

}
