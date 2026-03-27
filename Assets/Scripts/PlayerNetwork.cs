using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject cameraPivot;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Rigidbody rb;
    private float moveSpeed = 3;
    private float jumpHeight = 5;
    
    private float sensitivity = 100;
    private float yaw;
    private float pitch;

    private float zoomDistance = 5f;
    private float minZoom = 2f;
    private float maxZoom = 10f;
    private void Start()
    {
        if (!IsOwner)
        {
            cameraPivot.SetActive(false);
            return;
        }


    }

    private void Update()
    {
        if (!IsOwner) return;
        playerMove();
        CameraLogic();
    }

    private void playerMove()
    {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        transform.position += transform.forward * vert * moveSpeed * Time.deltaTime;
        transform.position += transform.right * horiz * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space)) rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void CameraLogic()
    {
        ResetMousePos();
        if (Input.GetMouseButton(2))
        {
            CameraRotate();
            cameraPivot.transform.rotation = Quaternion.Euler(0, yaw, 0);
        }
        else if (Input.GetMouseButton(1))
        {
            CameraRotate();
            transform.rotation = Quaternion.Euler(0, yaw, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //MouseZoom();
    }

    private void ResetMousePos()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Quaternion currentPos = cameraPivot.transform.rotation;
            cameraPivot.transform.rotation = transform.rotation;
        }
    }
    private void CameraRotate()
    {
        Cursor.lockState = CursorLockMode.Locked;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        
        yaw += mouseX;
    }
    private void MouseZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            zoomDistance -= scroll * 2f;
            zoomDistance = Mathf.Clamp(zoomDistance, minZoom, maxZoom);
            Vector3 zoomDir = (cameraPivot.transform.up + -cameraPivot.transform.forward).normalized;
            playerCamera.transform.position = zoomDir * zoomDistance;
        }
    }

}
