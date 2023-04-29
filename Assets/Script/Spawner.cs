using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType{
    monster,
    monsterWithSpawner
}
public class Spawner : MonoBehaviour
{
    public SpawnType type;
    public GameObject[] type1ObjectToSpawn;
    public GameObject[] type2ObjectToSpawn;
    public GameObject Wall; 
    public int limitX;
    public int limitZ;
    public float spawnPosY;
    public float spawnInterval = 1f;  
    public float spawnTime;
    public int minPower;
    public int maxPower;

    public float offset;

    public int minWallCount;
    public int maxWallCount;

    public GameObject[] spawnPosition;
    //enum타입에 따라 작동하는 방식이 다르게 구성했습니다.
    private void Start() {
        if(type == SpawnType.monsterWithSpawner){
            SpawnWall();
            SearchSpawnPosition();
        }
    }
    //switch문을 통해 enum에서 지정한 타입을 확인하고 각 상황에 맞게 작동합니다.
    //Game2의 경우 monster타입이 작동 Game3번의 경우 monsterWithSpawner가 작동합니다.
    private void Update()
    {
        switch (type){
            case SpawnType.monster :
                SpawnPattern1();
                break;
            case SpawnType.monsterWithSpawner :
                SpawnPattern2();
                break;
        }
    }
    //Game2의 시간규칙에 맞게 몬스터를 스폰합니다.
    void SpawnPattern1(){
        if(GameManager.instance.isPlaying){
            spawnTime += Time.deltaTime;
            if (spawnTime > spawnInterval)
            {
                Pattern1Spawn();
                spawnTime = 0f;        
            }
        }
    }
    //Game의 규칙에 맞게 몬스터를 스폰하여 움직이게합니다.
    private void Pattern1Spawn(){
        //랜덤좌표 설정 및 리스폰될 랜덤한 몬스터 결정합니다.
        float positionX = Random.Range(-limitX, limitX+1);
        int spawnPrefab = Random.Range(0, type1ObjectToSpawn.Length);
        int power = Random.Range(minPower, maxPower);
        
        //설정된 좌표를 확정해서 랜덤한 몬스터를 랜덤한 좌표에 스폰합니다.
        Vector3 spawnPos = new Vector3(positionX, spawnPosY, 12f);
        GameObject spawn = Instantiate(type1ObjectToSpawn[spawnPrefab], spawnPos, Quaternion.identity);

        //스폰과 동시에 rigidbody를 적용시킨뒤 공중에 던져줍니다.
        Rigidbody spawnRig = spawn.GetComponent<Rigidbody>();
        spawnRig.AddForce(Vector3.up * power, ForceMode.Impulse);
    }
    //Game3의 규칙에 맞게 스폰합니다. 추가적인 규칙을 적용할수있도록 작성했습니다.
    void SpawnPattern2(){
        if(GameManager.instance.isPlaying){
            spawnTime += Time.deltaTime;
            if (spawnTime > spawnInterval)
            {
                Pattern2Spawn();
                spawnTime = 0f;        
            }
        }
    }

    //오브젝트를 랜덤하게 생성합니다.
    void SpawnWall(){
        //몇개의 오브젝트를 생성할지 결정합니다.
        int wallCount = Random.Range(minWallCount, maxWallCount);
        //결정된 숫자만큼 오브젝트를 랜덤한 좌표에 생성합니다.
        for(int i = 1; i < wallCount + 1; i++){
            //null오류 방지를 위해 넣었습니다.
            Vector3[] spawndPosition = new Vector3[wallCount];
            spawndPosition[0] = new Vector3(0f, 0f, 0f);
            int positionX = Random.Range(-limitX, limitX+1); 
            int positionZ = Random.Range(-limitZ, limitZ+1);
            //오프셋변수를 통해 플레이공간을 넘어서 스폰되는것을 방지합니다.
            Vector3 spawnPos = new Vector3(positionX, 2, positionZ + offset);

            //같은 좌표에 동일한 오브젝트를 생성하는 문제를 제거했습니다. 타켓오브젝트스폰위치가 좌우이므로, 기존 스폰위치에서 좌우 역시 생성하지못하게 막았습니다.
            if(spawndPosition[i-1] != spawnPos || spawndPosition[i-1].x != spawnPos.x - 1 || spawndPosition[i-1].x != spawnPos.x + 1){
                Instantiate(Wall, spawnPos, Quaternion.identity);
                //같은좌표에 생성하지못하도록 생성된 좌표를 기록해두었습니다.
                if(i != wallCount){
                    spawndPosition[i] = spawnPos;
                }
            }
        }
    }

    //오브젝트옆에 미리 지정된 스폰포지션을 태그를 이용해서 모두 찾아줍니다.
    void SearchSpawnPosition(){
        spawnPosition = GameObject.FindGameObjectsWithTag("SpawnPosition");
    }

   // 태그를 통해 확인해둔 스폰위치에 오브젝트를 스폰시킵니다.
    void Pattern2Spawn(){
        //여러 오브젝트중 랜덤한 오브젝트 하나를 선택합니다.
        int spawnPrefab = Random.Range(0, type2ObjectToSpawn.Length);
        //여러 위치중 랜덤한 곳을 선택합니다.
        int randomPos = Random.Range(0, spawnPosition.Length); 
        Vector3 spawnPos = spawnPosition[randomPos].transform.position;
        //프리팹을 스폰합니다.
        Instantiate(type2ObjectToSpawn[spawnPrefab], spawnPos, Quaternion.identity);
    }
}
