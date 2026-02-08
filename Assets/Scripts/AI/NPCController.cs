using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCController : MonoBehaviour
{
    public enum AIState { Patrolling, Suspicious, Chasing, Attacking }
    public AIState currentState = AIState.Patrolling;
    public float suspicionLevel = 0f;

    public Transform[] waypoints;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4.5f;
    public float waitTime = 2f;
    public float stoppingDistance = 1.5f;

    public Transform player;
    public float detectionRadius = 12f;
    public float maxChaseDistance = 20f;
    public float fovAngle = 90f;
    public float detectionSpeed = 2f;
    public LayerMask obstacleMask;
    public float attackRange = 2f;

    private NavMeshAgent agent;
    private int currentWaypointIndex;
    private bool isWaiting;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        if (waypoints.Length > 0) agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        bool playerVisible = CanSeePlayer();
        UpdateState(playerVisible);

        switch (currentState)
        {
            case AIState.Patrolling: Patrol(); break;
            case AIState.Suspicious: SuspiciousBehavior(); break;
            case AIState.Chasing: Chase(); break;
            case AIState.Attacking: AttackReaction(); break;
        }

        HandlePathBlocking();
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRadius) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) < fovAngle / 2f)
            return !Physics.Raycast(transform.position + Vector3.up, dir, dist, obstacleMask);
        return false;
    }

    void UpdateState(bool playerVisible)
    {
        float dist = player != null ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

        if (playerVisible)
        {
            if (dist <= attackRange)
            {
                if (currentState != AIState.Attacking)
                {
                    Debug.Log("<color=red>[AI] Attacking Range</color>");
                    currentState = AIState.Attacking;
                }
            }
            else if (suspicionLevel >= 1f)
            {
                if (currentState != AIState.Chasing)
                {
                    Debug.Log("<color=orange>[AI] Chasing</color>");
                    currentState = AIState.Chasing;
                }
            }
            else
            {
                if (currentState != AIState.Suspicious && currentState != AIState.Chasing) 
                    currentState = AIState.Suspicious;
                suspicionLevel = Mathf.MoveTowards(suspicionLevel, 1f, detectionSpeed * Time.deltaTime);
            }
        }
        else
        {
            suspicionLevel = Mathf.MoveTowards(suspicionLevel, 0f, Time.deltaTime * 0.5f);
            if (currentState == AIState.Chasing || currentState == AIState.Attacking || (dist > maxChaseDistance && currentState != AIState.Patrolling))
            {
                Debug.Log("<color=blue>[AI] Lost Sight - Returning to Patrol</color>");
                ReturnToPatrol();
            }
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (waypoints.Length > 0 && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
            StartCoroutine(WaitAtWaypoint());
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        isWaiting = false;
    }

    void SuspiciousBehavior()
    {
        agent.speed = patrolSpeed * 0.5f;
        if (suspicionLevel <= 0) ReturnToPatrol();
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void AttackReaction()
    {
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (Vector3.Distance(transform.position, player.position) > attackRange + 0.5f)
        {
            agent.isStopped = false;
            currentState = AIState.Chasing;
        }
    }

    void ReturnToPatrol()
    {
        currentState = AIState.Patrolling;
        suspicionLevel = 0f;
        agent.isStopped = false;
        if (waypoints.Length > 0) agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void HandlePathBlocking()
    {
        if (agent.hasPath && (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid))
        {
            Debug.LogWarning("<color=orange>[AI] Path Blocked</color>");
            ReturnToPatrol();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Vector3 line1 = Quaternion.AngleAxis(fovAngle / 2, transform.up) * transform.forward * detectionRadius;
        Vector3 line2 = Quaternion.AngleAxis(-fovAngle / 2, transform.up) * transform.forward * detectionRadius;
        Gizmos.color = Color.blue; Gizmos.DrawRay(transform.position, line1); Gizmos.DrawRay(transform.position, line2);
    }
}
