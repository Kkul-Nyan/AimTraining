using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] objectToSpawn; 
    public float limitX;
    public float spawnPosY;
    public float spawnInterval = 1f;  
    public float spawnTime;
    private float timeLastSpawn = 0f;
    public int minPower;
    public int maxPower;

    private void Update()
    {
        if(GameManager.instance.isPlaying){
            spawnTime += Time.deltaTime;
            if (spawnTime > spawnInterval)
            {
                Spawn();
                spawnTime = 0f;        
            }
        }
    }

    private void Spawn(){
        float positionX = Random.Range(-limitX, limitX);
        int spawnPrefab = Random.Range(0, objectToSpawn.Length);
        int power = Random.Range(minPower, maxPower);
        
        Vector3 spawnPos = new Vector3(positionX, spawnPosY, 12f);
        GameObject spawn = Instantiate(objectToSpawn[spawnPrefab], spawnPos, Quaternion.identity);
        Rigidbody spawnRig = spawn.GetComponent<Rigidbody>();
        spawnRig.AddForce(Vector3.up * power, ForceMode.Impulse);
    }

}
