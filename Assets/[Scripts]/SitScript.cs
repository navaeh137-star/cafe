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

        playerRoot.position = sitTarget.position;
        playerRoot.rotation = sitTarget.rotation;

        if (xrOrigin != null)
        {
            xrOrigin.CameraYOffset = sittingYOffset;
        }
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