using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public Canvas optionCanvas;    
    public PlayercController player;
    

    //1번 게임씬 이동
    public void Type1(){
        SceneManager.LoadScene(1);
    }

    //2번 게임씬 이동
    public void Type2(){
        SceneManager.LoadScene(2);
    }
    //3번 게임씬 이동
    public void Type3(){
        SceneManager.LoadScene(3);
    }

    //프로그램 종료
    public void OnExitGame(){
        Application.Quit();
    }
    
    //옵션Canvas 엸기
    public void OnOptionButton(){
        //오브젝트 버튼을 옵션창 실행시 비활성화합니다(옵션캔버스 클릭이지만 뒤에있던 버튼이 클릭되는 버그 발견)
        transform.gameObject.SetActive(false);
        optionCanvas.gameObject.SetActive(true);
        player.ToggleCursor(true);
    }
}
