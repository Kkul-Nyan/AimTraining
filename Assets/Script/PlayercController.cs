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
