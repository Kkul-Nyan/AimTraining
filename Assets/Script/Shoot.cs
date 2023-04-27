using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public  GameObject hitTatget;
    private Camera cam;
    Vector3 sceenCenter = new Vector3( Screen.width / 2, Screen.height / 2, 0);

    private void Start() {
        cam = Camera.main;
    }
    private void Update() {
        
    }

    //유니티 inputsystem에서 마우스 좌클릭이 확인되면 레이를 발사 
    public void OnAttack(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            RaycastToObject();
        }
    }

    //레이 발사 및 오브젝트 판정, 발사파티클 작동, 피격파타클 작동
    public void RaycastToObject(){
        
        Ray ray = cam.ScreenPointToRay(sceenCenter);
        RaycastHit rayhit;
        
        if( Physics.Raycast(ray, out rayhit, 1000f)){
            Instantiate(hitTatget, rayhit.point, Quaternion.identity);
        }
    }
}
