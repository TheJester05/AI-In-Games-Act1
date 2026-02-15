using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimationHelper : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Animator Parameters")]
    public string speedParameter = "Speed";
    public string isJumpingParameter = "IsJumping";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Chase animation based on velocity
        float speed = agent.velocity.magnitude;
        animator.SetFloat(speedParameter, speed);

        // Jump animation based on Off-Mesh Link status
        bool isJumping = agent.isOnOffMeshLink;
        animator.SetBool(isJumpingParameter, isJumping);

        // Optional: Trigger a specific Jump trigger when starting a link
        // This can be used if the animator uses triggers instead of bools
        if (isJumping)
        {
            // Some manual logic if necessary to align animation with traversal
            // But usually SetBool is cleaner for duration of link
        }
    }
}
