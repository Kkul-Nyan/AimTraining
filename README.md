# AimTraining
 

## **기능 구현 파트**

### Crosshair

```csharp
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

    //옵션에서 원하는 크로스헤어 정보를 GameManager로 값을 저장하고, 싱글톤인 게임메니저가 다양한 씬에 같은 크로스헤어 옵션을 전달합니다
		//GameManager에서 저장된 값을 항상 동기화시켜줍니다.
    void Update()
    {
        //GameManager에 저장된 값을 적용합니다
        if(GameManager.instance.changeCrosshair){
            crosshiar.sprite = GameManager.instance.crosshairSprite[GameManager.instance.chooseCrosshair];
            crosshiar.color = GameManager.instance.crosshairColor; 
        }

    }

}
```

### Enemy

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 에너미의 패턴을 좀더 관리하기 편하기위해 타입을 만들어주었습니다.
public enum EnemyType{
    Chaseing,
    Destory,
    LimitTime
}
public class Enemy : MonoBehaviour
{
//헤더를 통해 변수가 어떤 패턴에서 작동하는지 직관적으로 정리되게 만들었습니다..
    [Header("Type")]
    public EnemyType type;

    [Header("OnlyChaseing")]
    public float enemySpeed;
    public float limitX;
    public float limitY;
    public float randompointX;
    private Vector3 goalPoint;

    [Header("OnlyTimeLimit")]
    public float timeLimit;
    public float timer;

    private void Start() {
        //업데이트를 통해 플레이전에도 목표물이 움직이게 되는 부분을 방지합니다.
        goalPoint = transform.position;
    }

    private void Update() {
        if(GameManager.instance.isPlaying){
            Pattern();
        }
    }

    //Switch문을 통해 원하는 타입에 맞게 그 타입에 지정된 스크립트를 작동시킵니다.
    public void Pattern(){
        switch(type){
            case EnemyType.Chaseing :
                MoveEnemy();    
                break; 

            case EnemyType.Destory :
                CheckHeight();
                break;

            case EnemyType.LimitTime :
                Timer();
                break;
        }
    }

    public void RandomMove(){
        //x,y 좌료를 랜덤으로 생성해주는 부분입니다.
        float pointX = Random.Range(-randompointX, randompointX + 1);
        float pointY = Random.Range(2, 6);
        //랜덤좌표입니다. 랜덤한 좌표가 지정한 범위 이외로 벗어나는지 체크합니다.
        Vector3 preGoalPoint = new Vector3(pointX, pointY, 10);
        
        //오브젝트의 위치값에 랜덤좌표를 더해서 지정된 위치 이상으로 이동할시에는 중단시키고, 다시 작업하게 합니다.
        if(Mathf.Abs(transform.position.x + preGoalPoint.x) > limitX || (transform.position.y + preGoalPoint.y) > limitY){
            return;
        }
        //범위 이내일경우 목표지점에 랜덤좌표를 대입합니다.
        goalPoint = preGoalPoint;
        
    }

    public void MoveEnemy(){
        //게임중이 아닐시 시작지점에 목표물을 고정해둔뒤, 중단하게만듭니다.
        if(!GameManager.instance.isPlaying){
            transform.position = new Vector3(0, 3, 10);
            return;
        }
        //게임이 실행되면 MoveTowards로 랜덤좌표로 만든 위치로 이동하게합니다.
        transform.position = Vector3.MoveTowards(transform.position, goalPoint, enemySpeed * Time.deltaTime);

        //원하는 위치까지 이동하게되면 다시 랜덤좌표를 계산합니다.
        if(transform.position == goalPoint){
            RandomMove();
        }
    }

    //일정 시간뒤 오브젝트 파괴합니다
    public void Timer(){
        timer += Time.deltaTime;
        if(timer >= timeLimit){
            SelfDestory();
        }
    }
    //오브젝트 높이를 체크 한뒤 파괴합니다
    public void CheckHeight(){
        if(transform.position.y < 0.6f){
            SelfDestory();
        }
    }
    //오브젝트 파괴합니다
    public void SelfDestory(){
        Destroy(this.gameObject);
    }
}
```

### GameManager

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public bool canLook = true;
    public bool isPlaying = false;
    public float targetHitPoint;
    public float totalShootPoint;
    public float hitPercent;
    public float basePlayTime;
    public float playTime;
    public Sprite[] crosshairSprite;

    public float sountSize = 0.5f;
    public float mauseSensitivity = 0.5f;

    public int chooseCrosshair = 1;

    //게임시작떄 기본 크로스헤어 색상을 설정합니다 
    //설정에따라 변경할수 있습니다.
    public Color crosshairColor = Color.red;
    public bool changeCrosshair = false;

    //게임매니저에 접근을 쉽게 만들기 위해 싱글톤 작업 및 점수등 데이터를 초기화 하지 않기위해 파괴불가로 설정
    public static GameManager instance;
    private void Awake() {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    private void Update() {
        Sound();
    }
    // 옵션창에서 설정된값에 맞게 사운드 조정합니다.
		// 다른씬에서도 동일한 설정값 유지를 위해 싱글톤인 GameManager에서 관리하도록했습니다.
    public void Sound(){
        AudioListener.volume = sountSize;
    }

    // R버튼을 눌리시 inputManager를 통해 값을 전달 받아서, 명중률 계산을 위해 기존의 값을 초기화해줌.
    public void GameStart(){
        if(!isPlaying){
            //플레이를 하면 기존에 가지고있는 값들을 초기화 해서 게임중 스탯을 정확히 계산하게합니다.
            isPlaying = true;
            playTime = basePlayTime;
            totalShootPoint = 0;
            targetHitPoint = 0;
        }    
    }
}
```

### MenuController

```csharp
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
```

### Option

```csharp
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
```

### PlayerController

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayercController : MonoBehaviour
{
    public float moveSpeed;
    Vector2 moveVec;
    Rigidbody rig;

    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    public bool canLook = true;
    Vector2 mouseDelta;

    public Canvas rCanvas;

    private void Awake() {
        rig = GetComponent<Rigidbody>();
    }
    private void Start() {
        //게임 시작과 동시에 커서를 안보이게 만들기
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        Move();
    }

    private void LateUpdate() {
        if(canLook == true){
            //GameManger에 저장된 민감도 설정값을 가져옵니다. 다른씬에 가도 동일한 민감도를 적용하게됩니다.
            lookSensitivity = GameManager.instance.mauseSensitivity;
            CameraLook();    
        }
    }
    public void Move(){
        //InputManager에서 받은 콜백 값으로 캐릭터를 앞,뒤,좌,우 움직임도록 만듬
        // y축의 경우 따로 건들 일이 없으나 충돌 등으로 혹시 이상이 생길지몰라  velocity.y값으로 계속 연동시켜줌
        Vector3 dir = transform.forward * moveVec.y + transform.right * moveVec.x;
        dir *= moveSpeed;
        dir.y = rig.velocity.y;
        rig.velocity = dir;
    }
    
    // InputManager에서 Vector2값을 받아서 Move()에서 쓰일 백터값을 전달해줌
    public void OnMove(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Performed){
            moveVec = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled){
            moveVec = Vector2.zero;
        }
    }
    
    // 마우스 델타값을 이용해서 마우스가 움직이는 곳으로 카메라가 회전하게 만들어줌.
    // 별도의 Sensitivity값을 통해 마우스 민감도를 조정할수 있도록 함.
    void CameraLook(){
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3 (-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    //inputManager를 통해 마우스 델타값을 Vector2값으로 받아오고 이를 CameraLook에서 사용할수 있도록 값을 전달해줌
    public void OnLookInput (InputAction.CallbackContext context){
        mouseDelta = context.ReadValue<Vector2>();
    }

    //결과표를 받았을때 다시 게임을 할것인지 다른 게임을 한것인지 Canvas에서 원하는 버튼을 클릭해야할때 커서락을 풀어주도록 만듬
    public void ToggleCursor(bool toggle){
        canLook = !toggle;
        GameManager.instance.canLook = canLook;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    //R키를 눌리시 InputManager에서 콜백을 받아서 스크립트를 작동. 버튼을 눌리는 단계에서 GameManager에 있는 GameStart스크립트실행
    public void GamePlay(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            GameManager.instance.GameStart();
            rCanvas.gameObject.SetActive(false);
        }

    }
}
```

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public Transform shootPoint;
    Animator anim;
    public GameObject hitTatget;
    public GameObject ShootParticle;
    private Camera cam;
    Vector3 sceenCenter = new Vector3( Screen.width / 2, Screen.height / 2, 0);
    public float delayShoot;
    public float maxDelayShoot;
    bool canShoot;

    private void Start() {
        cam = Camera.main;
        anim = GetComponent<Animator>();
    }
    private void Update() {
        // 발사 애니메이션이 끝나면 다시 발사 하도록 딜레이를 주었습니다.
        delayShoot -= Time.deltaTime;
        if(delayShoot <= 0){
            canShoot = true;
        }
    }

    //유니티 inputsystem에서 마우스 좌클릭이 확인되면 레이를 발사합니다.
    public void OnAttack(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            if(GameManager.instance.canLook){
                if(canShoot){
                    RaycastToObject();
                    GameManager.instance.totalShootPoint += 1;
                    //파티클 프리팹을 실행합니다.
                    GameObject shootPrefab = Instantiate(ShootParticle, shootPoint.transform.position, shootPoint.transform.rotation);
                    //파티클이 가지고있는 발사오디오소스를 실행합니다.
                    AudioSource shootAudio = shootPrefab.GetComponent<AudioSource>();
                    shootAudio.Play();
                    //발사 애니메이션을 작동시킵니다.
                    anim.SetBool("Shoot",true);
                    //딜레이를 지정된 maxDelay를 통해 초기화해줍니다.
                    delayShoot = maxDelayShoot;
                    canShoot = false;
                }       
            }

        }
        if(context.phase ==InputActionPhase.Canceled){
            //마우스버튼동작이 끝나면 다음 애니메이션 작동을 위해 Bool값을 변경해줍니다. HasExit기능을통해 미리 Bool값이 변동되어도 1회 실행을 하게됩니다.
            anim.SetBool("Shoot",false);
        }
    }

    //레이 발사 및 오브젝트 판정, 발사파티클 작동, 피격파타클 작동합니다.
    public void RaycastToObject(){
        
        Ray ray = cam.ScreenPointToRay(sceenCenter);
        RaycastHit rayhit;
        
        if( Physics.Raycast(ray, out rayhit, 1000f)){
            //레이를 발사할시 무조건 피격 파티클을 작동합니다.
            GameObject firePrefab = Instantiate(hitTatget, rayhit.point, Quaternion.identity);
            AudioSource fireSound = firePrefab.GetComponent<AudioSource>();
            
            //Enemy태그를 가진 오브젝트에만 명중사운드를 실행하며, GameManager가 가지고있는 명중횟수를 1증가시킵니다.
            if(rayhit.transform.CompareTag("Enemy")){
                GameManager.instance.targetHitPoint += 1;
                fireSound.Play();
            }
            //Enemy2태그를 가진 오브젝트에 명중시, GameMangerrㅏ 가지고있는 명중횟수를 1증가 시키고, 타켓오브젝트를 파괴합니다.
            else if(rayhit.transform.CompareTag("Enemy2")){
                GameManager.instance.targetHitPoint += 1;
                fireSound.Play();
                Destroy(rayhit.transform.gameObject);
            }
        }
    }

}
```

### Shoot

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public Transform shootPoint;
    Animator anim;
    public GameObject hitTatget;
    public GameObject ShootParticle;
    private Camera cam;
    Vector3 sceenCenter = new Vector3( Screen.width / 2, Screen.height / 2, 0);
    public float delayShoot;
    public float maxDelayShoot;
    bool canShoot;

    private void Start() {
        cam = Camera.main;
        anim = GetComponent<Animator>();
    }
    private void Update() {
        // 발사 애니메이션이 끝나면 다시 발사 하도록 딜레이를 주었습니다.
        delayShoot -= Time.deltaTime;
        if(delayShoot <= 0){
            canShoot = true;
        }
    }

    //유니티 inputsystem에서 마우스 좌클릭이 확인되면 레이를 발사합니다.
    public void OnAttack(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            if(GameManager.instance.canLook){
                if(canShoot){
                    RaycastToObject();
                    GameManager.instance.totalShootPoint += 1;
                    //파티클 프리팹을 실행합니다.
                    GameObject shootPrefab = Instantiate(ShootParticle, shootPoint.transform.position, shootPoint.transform.rotation);
                    //파티클이 가지고있는 발사오디오소스를 실행합니다.
                    AudioSource shootAudio = shootPrefab.GetComponent<AudioSource>();
                    shootAudio.Play();
                    //발사 애니메이션을 작동시킵니다.
                    anim.SetBool("Shoot",true);
                    //딜레이를 지정된 maxDelay를 통해 초기화해줍니다.
                    delayShoot = maxDelayShoot;
                    canShoot = false;
                }       
            }

        }
        if(context.phase ==InputActionPhase.Canceled){
            //마우스버튼동작이 끝나면 다음 애니메이션 작동을 위해 Bool값을 변경해줍니다. HasExit기능을통해 미리 Bool값이 변동되어도 1회 실행을 하게됩니다.
            anim.SetBool("Shoot",false);
        }
    }

    //레이 발사 및 오브젝트 판정, 발사파티클 작동, 피격파타클 작동합니다.
    public void RaycastToObject(){
        
        Ray ray = cam.ScreenPointToRay(sceenCenter);
        RaycastHit rayhit;
        
        if( Physics.Raycast(ray, out rayhit, 1000f)){
            //레이를 발사할시 무조건 피격 파티클을 작동합니다.
            GameObject firePrefab = Instantiate(hitTatget, rayhit.point, Quaternion.identity);
            AudioSource fireSound = firePrefab.GetComponent<AudioSource>();
            
            //Enemy태그를 가진 오브젝트에만 명중사운드를 실행하며, GameManager가 가지고있는 명중횟수를 1증가시킵니다.
            if(rayhit.transform.CompareTag("Enemy")){
                GameManager.instance.targetHitPoint += 1;
                fireSound.Play();
            }
            //Enemy2태그를 가진 오브젝트에 명중시, GameMangerrㅏ 가지고있는 명중횟수를 1증가 시키고, 타켓오브젝트를 파괴합니다.
            else if(rayhit.transform.CompareTag("Enemy2")){
                GameManager.instance.targetHitPoint += 1;
                fireSound.Play();
                Destroy(rayhit.transform.gameObject);
            }
        }
    }
}
```

```
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
```

### Spawner

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum을 통해 여러방식의 스포너를 손쉽게 관리하기위해 만들었습니다.
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
```

### UiController

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public GameObject resultCanvas;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI ShootText;
    public TextMeshProUGUI PercentText;
    PlayercController player;

    public Sprite[] awardSprite;
    public Image awardImage;

    float playTime;
    float maxPlayTime = 10f;

    private void Start() {
        player = FindObjectOfType<PlayercController>().GetComponent<PlayercController>();
        resultCanvas.SetActive(false);
				//각 씬에 미리설정된 플레이타임으로 교체해줍니다.
        GameManager.instance.basePlayTime = maxPlayTime;
        playTime = maxPlayTime;
    }
    void Update()
    {
        CanvasControll();
        if(GameManager.instance.isPlaying){
            PlayTime();
        }
    }

    //캔버스 위쪽에서 남은 게임시간을 보여줍니다.
    public void CanvasControll(){
        if(!GameManager.instance.isPlaying){
            //GameManager에서 설정된 게임시간을 동기화합니다.
            timerText.text = "Timer : " + maxPlayTime;
        }
        else if(GameManager.instance.isPlaying){
            //게임이 실행되면 줄어드는 시간을 GameManager에서 가져와서 동기화합니다.
          timerText.text = "Timer : " + (float)Math.Round(playTime, 2); 
        }
    }

    public void Result(){
        //PlayerController에서 만들어둔 커서토글 스크립트를 작동해서 커서락을 풀어주고, 화면을 고정시킵니다.
        player.ToggleCursor(true);
        
        resultCanvas.SetActive(true);

        //GameManager가 가지고있는 맞춘 횟수와 총발사 횟수를 가져와서 켄버스 텍스트를 교체합니다.
        hitText.text = "Hit : " + GameManager.instance.targetHitPoint;
        ShootText.text ="Shoot : " + GameManager.instance.totalShootPoint;

        //GameManager가 가지고있는 사용횟수가 많은 명중률을 변수로 만들어줍니다.
        float percent = GameManager.instance.hitPercent * 100;
        
        //명중률을 미리 지정해둔 명중률 텍스트와 교체해줍니다.
        PercentText.text = "Percent : " + (float)Math.Round(percent, 1) + "%";

        // 명중률에 따라 지정된 스프라이트를 이미지에 넣어주고, 색상을 변경해줍니다.
        if(percent >= 90f){
            awardImage.sprite = awardSprite[0];
            awardImage.color = new Color(255, 185, 0);
        }
        else if(percent < 90 && percent >= 80){
            awardImage.sprite = awardSprite[1];
            awardImage.color = Color.white;
        }
        else if(percent < 80 && percent >= 70){
            awardImage.sprite = awardSprite[2];
            awardImage.color = new Color(255, 80, 0);
        }
        else if(percent < 70) {
            awardImage.sprite = awardSprite[3];
            awardImage.color = new Color(120,120,120);
        }
    }

    //게임상의 플레이 시간을 계산합니다. 게임시간이 종료되면 명중률을 계산한뒤, UIController 명령어를 수행시킵니다.
    public void PlayTime(){
        playTime -= Time.deltaTime;
        if(playTime <= 0){
            playTime = 0;
            GameManager.instance.isPlaying = false;
            GameManager.instance.hitPercent = GameManager.instance.targetHitPoint / GameManager.instance.totalShootPoint;
            Result();
        }
    }

    public void OnAgainBTN(){
        // Again 버튼을 눌리시 게임씬을 다시 불려와서 처음부터 시작하게합니다.
        player.ToggleCursor(false);
        string name = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
    }
    
    public void OnExitBTN(){
        // 종료버튼을 눌리시 메인메뉴씬으로 로드합니다.
        player.ToggleCursor(false);
        SceneManager.LoadScene(0);
    }
}
```

## **문제점 및 개선 방안**

1. 옵션창에서 설정을 저장한뒤 나가서 다시 설정을 들어가면 초기화 되는 부분(수정완료) → GameManager가 저장값을 가지고있다. 옵션창에 들어갈떄 다시 적용 시킵니다.
2. 사격애니메이션이 종료전에 사격시 애니메이션이 작동이 먹통됨 → 사격이후 재사격시 DeltaTime을 이용해서 딜레이를 주었습니다.

## **개선예정**

개선할부분이 세부적으로도 큰부분으로도 다양하다 생각합니다. 세부적으로는 플레이시간을 커스텀 한다거나, 레벨시스탬을 도입해서 난이도를 점차적으로 증가 시키는 부분(예 : 1번게임 오브젝트 속도증가, Z축움직임 활성화, 좀더 넓은 범위를 움직임 등등) 이 있을겁니다. 큰시스탬부분으로는

1. 좀더 다양한 게임 모드 추가
2. 점수 랭킹시스탬(기존기록 저장)
3. 커스텀 크로스헤어시스탬
4. 다양한 무기 추가

를 생각했습니다.
