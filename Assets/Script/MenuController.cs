using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{

    public GameObject startList;
    public GameObject optionList;

    //옵션버튼이 켜져있는상황에서 Start버튼을 눌렸을때 옵션세부버튼이 사라지지 않아 Start세부옵션과 겹치는 부분을 해결하기 위해 옵션세부목록을 꺼버림
    public void OpenGameType1(){
        optionList.gameObject.SetActive(false);
    }

    //스타트버튼이 켜져있는상황에서 옵션버튼을 눌렸을때 옵션세부버튼과 스타트세부버튼이 겹치는 부분을 해결하기 위해 옵션버튼 클릭시 스타트세부목록을 종료
    public void OpenGameType2(){
        startList.gameObject.SetActive(false);
    }

    //1번 게임씬 이동
    public void TypeOne(){
        SceneManager.LoadScene(1);
        GameManager.instance.canvas.gameObject.SetActive(true);
        GameManager.instance.uiCanvas.gameObject.SetActive(true);
    }

    //2번 게임씬 이동
    public void TypeTwo(){
        SceneManager.LoadScene(2);
        GameManager.instance.canvas.gameObject.SetActive(true);
        GameManager.instance.uiCanvas.gameObject.SetActive(true);
    }

    //프로그램 종료
    public void OnExitGame(){
        Application.Quit();
    }
}
