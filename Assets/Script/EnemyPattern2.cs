using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPattern2 : MonoBehaviour
{
    private void Update() {
        if(transform.position.y < 0.6f){
            Destroy(this.gameObject);
        }
    }
}
