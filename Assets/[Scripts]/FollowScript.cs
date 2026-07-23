using UnityEngine;

public class FollowScript : MonoBehaviour
{
    [Header("Target")]
    public Transform Target;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float followRange = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private bool ignoreYForRotation = true;

    [Header("Audio")]
    [SerializeField] private AudioClip meowClip;
    [SerializeField] private float meowRange = 3f;
    [SerializeField] private float meowCooldown = 4f;

    private AudioSource audioSource;
    private float nextMeowTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (Target == null)
            return;

        Vector3 targetPosition = ignoreYForRotation
            ? new Vector3(Target.position.x, transform.position.y, Target.position.z)
            : Target.position;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > followRange)
        {
            RotateTowards(targetPosition);
            return;
        }

        if (distanceToTarget > stopDistance)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }

        if (distanceToTarget <= meowRange && Time.time >= nextMeowTime)
        {
            PlayMeow();
            nextMeowTime = Time.time + meowCooldown;
        }

        RotateTowards(targetPosition);
    }

    private void PlayMeow()
    {
        if (meowClip == null || audioSource == null)
            return;

        if (audioSource.isPlaying)
            return;

        audioSource.PlayOneShot(meowClip);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 lookDirection = targetPosition - transform.position;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        if (ignoreYForRotation)
            lookDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
