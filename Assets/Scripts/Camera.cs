using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cameraTarget;

    void Update()
    {
        if (cameraTarget.Target.TrackingTarget == null)
        {
            try
            {
                cameraTarget.Target.TrackingTarget = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch
            {
                cameraTarget.Target.TrackingTarget = null;
            }
        }
    }
}
