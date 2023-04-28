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
        delayShoot -= Time.deltaTime;
        if(delayShoot <= 0){
            canShoot = true;
        }
    }

    //유니티 inputsystem에서 마우스 좌클릭이 확인되면 레이를 발사 
    public void OnAttack(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            if(canShoot){
                RaycastToObject();
                GameManager.instance.totalShootPoint += 1;
                Instantiate(ShootParticle, shootPoint.transform.position, shootPoint.transform.rotation);
                anim.SetBool("Shoot",true);

                delayShoot = maxDelayShoot;
                canShoot = false;
            }
            
        }
        if(context.phase ==InputActionPhase.Canceled){
            anim.SetBool("Shoot",false);
        }
    }

    //레이 발사 및 오브젝트 판정, 발사파티클 작동, 피격파타클 작동
    public void RaycastToObject(){
        
        Ray ray = cam.ScreenPointToRay(sceenCenter);
        RaycastHit rayhit;
        
        if( Physics.Raycast(ray, out rayhit, 1000f)){
            Instantiate(hitTatget, rayhit.point, Quaternion.identity);
            if(rayhit.transform.CompareTag("Enemy")){
                GameManager.instance.targetHitPoint += 1;
            }
        }
    }



}
