using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    private Transform cam;
    private float moveSpeed = 3;
    private float jumpHeight = 5;
    
    private Vector3 spherePos;
    private float offset = 1;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private void Update()
    {
        if (!IsOwner) return;
        UpdateDetails();
        playerMove();
    }

    private void playerMove()
    {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horiz, 0, vert).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Grounded()) rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void UpdateDetails()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);

        if (cam == null)
        {
            try
            {
                cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
            }
            catch
            {
                cam = null;
            }
        }
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
}
