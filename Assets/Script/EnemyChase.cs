using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyChase : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isJumping = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.autoTraverseOffMeshLink = false;
    }

    void Update()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (agent.isOnOffMeshLink && !isJumping)
        {
            StartCoroutine(TraverseLink());
        }
    }

    IEnumerator TraverseLink()
    {
        isJumping = true;

        animator.SetTrigger("Jump");

        OffMeshLinkData data = agent.currentOffMeshLinkData;

        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        float duration = 0.6f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        agent.CompleteOffMeshLink();
        isJumping = false;
    }
}