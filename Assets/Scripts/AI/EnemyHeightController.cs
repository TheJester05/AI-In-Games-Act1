using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHeightController : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float chaseSpeed = 5f;
    public float stoppingDistance = 1.5f;

    [Header("Height Level Settings")]
    public float lowToMidThreshold = 2.0f;
    public float midToHighThreshold = 5.0f;
    
    [Tooltip("Points on the Mid level that the enemy must use to reach High from Low.")]
    public Transform[] midLevelPoints;

    private NavMeshAgent agent;
    private enum HeightLevel { Low, Mid, High }
    private Transform forcedMidTarget = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        HeightLevel enemyLevel = GetLevel(transform.position.y);
        HeightLevel playerLevel = GetLevel(player.position.y);

        // Rule: Low -> Mid -> High. No skipping Mid.
        if (enemyLevel == HeightLevel.Low && playerLevel == HeightLevel.High)
        {
            // Must go through Mid first
            if (forcedMidTarget == null)
            {
                forcedMidTarget = GetClosestTransform(midLevelPoints, transform.position);
            }

            if (forcedMidTarget != null)
            {
                agent.SetDestination(forcedMidTarget.position);
                
                // If we reached the mid target or the mid level, clear it
                if (Vector3.Distance(transform.position, forcedMidTarget.position) < 2f || GetLevel(transform.position.y) == HeightLevel.Mid)
                {
                    forcedMidTarget = null;
                }
            }
            else
            {
                // Fallback if no mid points are set
                Debug.LogWarning("No Mid Level Points set! Enemy might skip levels.");
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // Normal chase
            forcedMidTarget = null;
            agent.SetDestination(player.position);
        }
    }

    private HeightLevel GetLevel(float y)
    {
        if (y < lowToMidThreshold) return HeightLevel.Low;
        if (y < midToHighThreshold) return HeightLevel.Mid;
        return HeightLevel.High;
    }

    private Transform GetClosestTransform(Transform[] targets, Vector3 position)
    {
        if (targets == null || targets.Length == 0) return null;

        Transform closest = targets[0];
        float minDist = Vector3.Distance(position, closest.position);

        foreach (Transform t in targets)
        {
            float dist = Vector3.Distance(position, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }
        return closest;
    }
}
