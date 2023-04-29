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
