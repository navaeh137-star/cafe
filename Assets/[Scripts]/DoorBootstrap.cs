using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorBootstrap : MonoBehaviour
{
    [Header("Door Setup")]
    [SerializeField] private Transform doorMeshRoot;
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 85f;
    [SerializeField] private float openSmoothing = 8f;
    [SerializeField] private bool closeOnRelease = true;
    [SerializeField] private bool allowOppositeSideGrab = true;

    [Header("Grab Setup")]
    [SerializeField] private bool autoAddXRGrabInteractable = true;
    [SerializeField] private bool preferRightController = true;

    private XRGrabInteractable doorInteractable;
    private Quaternion closedLocalRotation;
    private float currentAngle;
    private bool isGrabbed;
    private IXRSelectInteractor grabbedInteractor;

    private void Awake()
    {
        if (doorMeshRoot == null)
            doorMeshRoot = transform;

        if (doorPivot == null)
        {
            doorPivot = new GameObject($"{doorMeshRoot.name}_Pivot").transform;
            doorPivot.SetParent(doorMeshRoot.parent, true);
            doorPivot.position = doorMeshRoot.position;
            doorPivot.rotation = doorMeshRoot.rotation;
        }

        if (doorMeshRoot.parent != doorPivot)
            doorMeshRoot.SetParent(doorPivot, true);

        closedLocalRotation = doorPivot.localRotation;
        SetupDoorInteractable();
    }

    private void OnEnable()
    {
        if (doorInteractable != null)
        {
            doorInteractable.selectEntered.AddListener(OnDoorGrabbed);
            doorInteractable.selectExited.AddListener(OnDoorReleased);
        }
    }

    private void OnDisable()
    {
        if (doorInteractable != null)
        {
            doorInteractable.selectEntered.RemoveListener(OnDoorGrabbed);
            doorInteractable.selectExited.RemoveListener(OnDoorReleased);
        }
    }

    private void Update()
    {
        if (doorPivot == null || doorMeshRoot == null)
            return;

        float targetAngle = isGrabbed && grabbedInteractor != null ? GetGrabAngle() : 0f;
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * openSmoothing);

        Quaternion targetRotation = Quaternion.AngleAxis(currentAngle, doorPivot.up) * closedLocalRotation;
        doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, targetRotation, Time.deltaTime * openSmoothing);
    }

    private void SetupDoorInteractable()
    {
        if (doorMeshRoot == null)
            return;

        doorInteractable = doorMeshRoot.GetComponent<XRGrabInteractable>();

        if (doorInteractable == null && autoAddXRGrabInteractable)
            doorInteractable = doorMeshRoot.gameObject.AddComponent<XRGrabInteractable>();

        if (doorInteractable != null)
        {
            doorInteractable.trackRotation = false;
            doorInteractable.trackPosition = true;
            doorInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }

        if (doorMeshRoot.GetComponent<Rigidbody>() == null)
            doorMeshRoot.gameObject.AddComponent<Rigidbody>();

        Rigidbody rigidbody = doorMeshRoot.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    private float GetGrabAngle()
    {
        if (grabbedInteractor == null || doorPivot == null)
            return 0f;

        Vector3 handDirection = grabbedInteractor.transform.position - doorPivot.position;
        Vector3 planarDirection = Vector3.ProjectOnPlane(handDirection, doorPivot.up);

        if (planarDirection.sqrMagnitude < 0.0001f)
            return 0f;

        float signedAngle = Vector3.SignedAngle(doorPivot.forward, planarDirection, doorPivot.up);

        if (allowOppositeSideGrab)
        {
            float sideBias = Vector3.Dot(handDirection.normalized, doorPivot.right);
            if (sideBias < 0f)
                signedAngle = -signedAngle;
        }

        return Mathf.Clamp(signedAngle, -openAngle, openAngle);
    }

    private void OnDoorGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        grabbedInteractor = args.interactorObject;

        if (preferRightController && grabbedInteractor is XRBaseControllerInteractor controllerInteractor)
        {
            if (controllerInteractor.controllerNode == XRNode.RightHand)
                return;
        }
    }

    private void OnDoorReleased(SelectExitEventArgs args)
    {
        if (closeOnRelease)
            isGrabbed = false;

        grabbedInteractor = null;
    }
}
