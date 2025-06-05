using UnityEngine;
using UnityEngine.AI;

namespace CroakCreek
{
    public class FrogWander : MonoBehaviour
    {
        GameObject playerObject;
        NavMeshAgent agent;
        [SerializeField] LayerMask ground, player;

        Vector3 destPoint;
        bool walkPointSet;
        [SerializeField] float range;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            playerObject = GameObject.Find("Player");
        }

        private void Update()
        {
            Wander();
        }

        void Wander()
        {
            if (!walkPointSet) SearchForDest();
            if (walkPointSet) agent.SetDestination(destPoint);
            if (Vector3.Distance(transform.position, destPoint) < 10) walkPointSet = false;
        }

        void SearchForDest()
        {
            float z = Random.Range(-range, range);
            float x = Random.Range(-range, range);

            destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
            
            if (Physics.Raycast(destPoint,Vector3.down,ground))
            {
                walkPointSet = true;
            }
        }
    }
}
