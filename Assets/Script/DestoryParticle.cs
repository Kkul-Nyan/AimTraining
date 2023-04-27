using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryParticle : MonoBehaviour
{
    GameObject gameObject;
    public float curTime;
    public float maxLifeTime = 10;
    private void Awake() {
        gameObject = GetComponent<GameObject>();
    }
    void Update()
    {
        if (curTime < maxLifeTime){
            curTime += Time.deltaTime; 
        }
        else{
            Destroy(gameObject);
        }
    }
}
