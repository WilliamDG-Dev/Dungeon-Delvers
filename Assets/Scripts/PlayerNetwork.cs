using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject cameraPivot;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Rigidbody rb;
    private float moveSpeed = 3;
    private float jumpHeight = 5;
    
    private float sensitivity = 1000;
    private Vector3 spherePos;
    private float offset = 1;

    private float yaw;

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
        spherePos = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        playerMove();
        CameraLogic();
    }

    private void playerMove()
    {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        transform.position += transform.forward * vert * moveSpeed * Time.deltaTime;
        transform.position += transform.right * horiz * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && Grounded()) rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private bool Grounded()
    {
        Collider[] colliders = Physics.OverlapSphere(spherePos, 1);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Ground")
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spherePos, 1);
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

}
