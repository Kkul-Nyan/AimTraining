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

    bool canLook = true;
    Vector2 mouseDelta;


    private void Awake() {
        rig = GetComponent<Rigidbody>();
    }
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        Move();
    }

    private void LateUpdate() {
        if(canLook == true){
            CameraLook();    
        }
    }
    public void Move(){
        Vector3 dir = transform.forward * moveVec.y + transform.right * moveVec.x;
        dir *= moveSpeed;
        dir.y = rig.velocity.y;
        rig.velocity = dir;
    }
    
    public void OnMove(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Performed){
            moveVec = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled){
            moveVec = Vector2.zero;
        }
    }
    
    void CameraLook(){
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3 (-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput (InputAction.CallbackContext context){
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void ToggleCursor(bool toggle){
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
