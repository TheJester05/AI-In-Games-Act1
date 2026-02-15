using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Use physics raycast hit from mouse click to set agent destination
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMove : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        RaycastHit m_HitInfo = new RaycastHit();
        private Animator animationController_AI;

        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>(); // Gets the NavMeshAgent component on the same GameObject
            animationController_AI = GetComponent<Animator>();   // Gets the Animator component on the same GameObject
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                    m_Agent.destination = m_HitInfo.point;
            }


            // If the agent is close enough to its destination (within stopping distance)...
            if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
            {
                animationController_AI.SetBool("Running", false); // Tell Animator we are NOT running (so it can play idle/walk/stop)
            }
            else
            {
                animationController_AI.SetBool("Running", true);  // Tell Animator we ARE running (so it can play run animation)
            }
        }

      
    }
}