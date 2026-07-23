using UnityEngine;
using UnityEngine.InputSystem;

public class SitScript : MonoBehaviour
{
    public Transform playerRoot;
    
    public Transform sitTarget;

    public Unity.XR.CoreUtils.XROrigin xrOrigin;
    
    public float standingYOffset = 1.1f;
    public float sittingYOffset = 0.5f;

    
    public InputActionProperty aButtonAction;

    private bool isNearSofa = false;
    private bool isSeated = false;

    void OnEnable()
    {
        if (aButtonAction.action != null)
            aButtonAction.action.Enable();
    }

    void Update()
    {
        if (isNearSofa && aButtonAction.action != null && aButtonAction.action.WasPressedThisFrame())
        {
            if (!isSeated)
            {
                SitDown();
            }
            else
            {
                StandUp();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<Unity.XR.CoreUtils.XROrigin>() != null)
        {
            isNearSofa = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<Unity.XR.CoreUtils.XROrigin>() != null)
        {
            isNearSofa = false;
        }
    }

    public void SitDown()
    {
        isSeated = true;

        if (xrOrigin != null)
        {
            xrOrigin.CameraYOffset = sittingYOffset;
        }

        float desiredYaw = sitTarget.eulerAngles.y;
        float currentCameraYaw = xrOrigin.Camera.transform.eulerAngles.y;

        float yawDelta = desiredYaw - currentCameraYaw;

        playerRoot.RotateAround(xrOrigin.Camera.transform.position, Vector3.up, yawDelta);

        Vector3 cameraOffset = xrOrigin.Camera.transform.position - playerRoot.position;
        cameraOffset.y = 0;

        playerRoot.position = sitTarget.position - cameraOffset;
    }

    public void StandUp()
    {
        isSeated = false;

        if (xrOrigin != null)
        {
            xrOrigin.CameraYOffset = standingYOffset;
        }
    }
}