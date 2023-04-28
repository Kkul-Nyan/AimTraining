using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using TMPro;

public class EnemyPattern1 : MonoBehaviour
{
    public float enemySpeed;
    public float limitX;
    public float limitY;
    public float randompointX;
    public Vector3 goalPoint;
    public float playTime;
    public UiController ui;



    private void Start() {
        goalPoint = transform.position;
    }
    private void Update() {
        MoveEnemy();
        
    }

  
    public void RandomMove(){
        float pointX = Random.Range(-randompointX, randompointX + 1);
        float pointY = Random.Range(2, 6);
        Vector3 preGoalPoint = new Vector3(pointX, pointY, 10);
        
        if(Mathf.Abs(transform.position.x + preGoalPoint.x) > limitX){
            return;
        }
        goalPoint = preGoalPoint;
        
    }

    public void MoveEnemy(){
        if(!GameManager.instance.isPlaying){
            transform.position = new Vector3(0, 3, 10);
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, goalPoint, enemySpeed * Time.deltaTime);
        if(transform.position == goalPoint){
            RandomMove();
        }
    }




}
