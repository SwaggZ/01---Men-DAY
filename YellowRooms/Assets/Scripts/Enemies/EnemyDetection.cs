using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 20f;
    [Range(0f, 360f)] public float fieldOfView = 110f;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    [Header("Visibility Threshold")]
    [Range(0f, 1f)] public float visibilityThreshold = 0.3f;
    public int samplesPerAxis = 2;

    [Header("Close Proximity Detection")]
    public float closeRangeRadius = 5f;

    [Header("Distance Control")]
    public float preferredDistance = 10f;
    public float distanceBuffer = 2f;
    public float minimumDistance = 4f;

    [Header("Strafe Movement")]
    public float strafeInterval = 2f;
    public float strafeDistance = 2f;

    [Header("Detection State")]
    public bool canSeePlayer;
    public Transform playerRef;

    [Header("References")]
    public Transform eyes;
    private NavMeshAgent agent;

    [Header("Turning")]
    public float turnSpeed = 5f;
    private Quaternion targetRotation;

    private float lastStrafeTime;
    private int strafeDirection = 0;

    [Header("Retreat Settings")]
    public float retreatDistance = 3f;

    [SerializeField] private float distanceToPlayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        StartCoroutine(FOVCheckRoutine());
    }

    private void Update()
    {
        if (canSeePlayer && playerRef != null)
        {
            RotateTowardPlayer();
            HandleMovement();
        }
        else
        {
            agent.ResetPath();
        }

        if (canSeePlayer && playerRef != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerRef.position);
            RotateTowardPlayer();
            HandleMovement();
        }
        else
        {
            agent.ResetPath();
            distanceToPlayer = 0f;
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distanceToPlayer);
    }

    private void RotateTowardPlayer()
    {
        Vector3 dirToPlayer = (playerRef.position - eyes.position).normalized;
        dirToPlayer.y = 0f;

        if (dirToPlayer != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void HandleMovement()
    {
        if (playerRef == null) return;

        Vector3 toPlayer = playerRef.position - transform.position;
        toPlayer.y = 0f;
        float distanceToPlayer = toPlayer.magnitude;
        Vector3 directionToPlayer = toPlayer.normalized;

        NavMeshHit hit;

        // Case 1: Player is too close — move directly back
        if (distanceToPlayer < minimumDistance)
        {
            Vector3 retreat = transform.position - directionToPlayer * retreatDistance;
            if (NavMesh.SamplePosition(retreat, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            return;
        }

        // Strafe logic: always try to orbit at exact preferredDistance
        if (Time.time - lastStrafeTime > strafeInterval)
        {
            lastStrafeTime = Time.time;
            strafeDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        }

        Vector3 right = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        Vector3 strafeOffset = right * strafeDistance * strafeDirection;

        // Always target a point around the player at preferredDistance
        Vector3 orbitTarget = playerRef.position - directionToPlayer * preferredDistance + strafeOffset;

        if (NavMesh.SamplePosition(orbitTarget, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private IEnumerator FOVCheckRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(eyes.position, detectionRadius, playerMask);
        Collider[] closeChecks = Physics.OverlapSphere(eyes.position, closeRangeRadius, playerMask);

        if (closeChecks.Length != 0)
        {
            Transform target = closeChecks[0].transform;
            canSeePlayer = true;
            playerRef = target;
            return;
        }

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Collider col = target.GetComponent<Collider>();

            if (col == null)
            {
                canSeePlayer = false;
                playerRef = null;
                return;
            }

            Bounds bounds = col.bounds;
            int visiblePoints = 0;
            int totalPoints = 0;

            for (int x = -samplesPerAxis; x <= samplesPerAxis; x++)
            {
                for (int y = -samplesPerAxis; y <= samplesPerAxis; y++)
                {
                    for (int z = -samplesPerAxis; z <= samplesPerAxis; z++)
                    {
                        Vector3 offset = new Vector3(
                            bounds.extents.x * x / samplesPerAxis,
                            bounds.extents.y * y / samplesPerAxis,
                            bounds.extents.z * z / samplesPerAxis
                        );

                        Vector3 point = bounds.center + offset;
                        Vector3 dirToPoint = (point - eyes.position).normalized;
                        float dist = Vector3.Distance(eyes.position, point);

                        if (Vector3.Angle(eyes.forward, dirToPoint) < fieldOfView / 2f)
                        {
                            totalPoints++;
                            if (!Physics.Raycast(eyes.position, dirToPoint, dist, obstructionMask))
                            {
                                visiblePoints++;
                            }
                        }
                    }
                }
            }

            float visibilityRatio = (float)visiblePoints / Mathf.Max(totalPoints, 1);

            if (visibilityRatio >= visibilityThreshold)
            {
                canSeePlayer = true;
                playerRef = target;
                return;
            }
        }

        canSeePlayer = false;
        playerRef = null;
    }

    private void OnDrawGizmos()
    {
        if (eyes == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyes.position, detectionRadius);

        Vector3 fovLine1 = Quaternion.Euler(0, fieldOfView / 2, 0) * eyes.forward * detectionRadius;
        Vector3 fovLine2 = Quaternion.Euler(0, -fieldOfView / 2, 0) * eyes.forward * detectionRadius;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eyes.position, fovLine1);
        Gizmos.DrawRay(eyes.position, fovLine2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(eyes.position, closeRangeRadius);

        if (canSeePlayer && playerRef != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(eyes.position, playerRef.position);
        }
    }
}
