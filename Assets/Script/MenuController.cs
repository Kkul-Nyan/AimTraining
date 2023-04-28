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
    public void TypeOne(){
        SceneManager.LoadScene(1);

    }

    //2번 게임씬 이동
    public void TypeTwo(){
        SceneManager.LoadScene(2);

    }

    //프로그램 종료
    public void OnExitGame(){
        Application.Quit();
    }
    
    public void OnOptionButton(){
        optionCanvas.gameObject.SetActive(true);
        player.ToggleCursor(true);
    }
}
