using UnityEngine;
using UnityEngine.AI;

namespace CroakCreek
{
    public class DebugNavmeshAgent : MonoBehaviour
    {
        public bool velocity;
        public bool desiredVelocity;
        public bool path;

        NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void OnDrawGizmos()
        {
            if (velocity)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
            }

            if (desiredVelocity)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
            }

            if (path)
            {
                Gizmos.color = Color.black;
                var agentPath = agent.path;
                Vector3 prevCorner = transform.position;
                foreach(var corner in agentPath.corners)
                {
                    Gizmos.DrawLine(prevCorner, corner);
                    Gizmos.DrawSphere(corner, 0.1f);
                    prevCorner = corner;
                }
            }
        }
    }
}
