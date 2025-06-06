using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace CroakCreek
{
    public class AiLocomotion : MonoBehaviour
    {
        public Transform playerTransform;
        public float maxTime = 1.0f;
        public float maxDistance = 1.0f;
        NavMeshAgent agent;
        float timer = 0.0f;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                float sqrDistance = (playerTransform.position - agent.destination).sqrMagnitude;
                if (sqrDistance > maxDistance*maxDistance)
                {
                    agent.destination = playerTransform.position;
                }
                timer = maxTime;
            }
            
        }
    }
}
