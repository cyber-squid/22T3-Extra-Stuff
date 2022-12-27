using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace PathingAPI
{
    public class AStarManager : MonoBehaviour
    {
        public AStar aStar = new AStar();

        Node startNode;
        Node endNode;
        List<Node> patrolNodes = new List<Node>();

        bool followingSinglePath = false;
        bool followingPatrolPath = false;
        public bool stopFollowingPath = false;

        [SerializeField] bool showNodePath = true;
        [SerializeField] Color nodePathColor = Color.cyan;

        int pathingIndex = 0;
        int patrollingNodeIndex = 0;
        float moveSpeed = 1;


        // follow one path and return.
        public void FollowPathSingle(Vector3Int startPoint, Vector3Int endPoint, float movingSpeed)
        {
            if (aStar.grid == null)
            {
                Debug.LogError("No grid found! Try setting the manager's A* grid variable with a reference to the one your agent is using.");
                return;
            }


            startNode = aStar.grid.GetNode(startPoint);
            endNode = aStar.grid.GetNode(endPoint);

            Thread thread = new Thread(() => aStar.FindPath(startNode, endNode));

            if (aStar.status == AStar.PathingStatus.pathFound)
            {
                followingSinglePath = true;
                pathingIndex = 0;
                moveSpeed = movingSpeed;

                //RecursiveFollowSingle();
            }
            else
                Debug.LogError("No valid path found!");

        }

        // continuously path between a given set of points on the grid.
        public void FollowPatrolPath(Vector3Int[] patrolPoints, float movingSpeed)
        {
            if (aStar.grid == null)
            {
                Debug.LogError("No grid found! You need a grid for the A* to work with.");
                return;
            }


            //patrolNodes.Capacity = patrolPoints.Length;
            patrolNodes.Clear();

            for (int i = 0; i < patrolPoints.Length; i++)
                patrolNodes.Add(aStar.grid.GetNode(patrolPoints[i]));

            aStar.FindPath(patrolNodes[0], patrolNodes[1]);

            if (aStar.status == AStar.PathingStatus.pathFound)
            {
                followingPatrolPath = true;
                pathingIndex = 0;
                patrollingNodeIndex = 0;
                moveSpeed = movingSpeed;
            }
            else
                Debug.LogError("No valid path found!");

        }

        private void Update()
        {
            // loop for a one-shot path follow.
            if (followingSinglePath)
            {

                if (stopFollowingPath) // allow for cancelling the path follow during execution
                {
                    stopFollowingPath = false;
                    followingSinglePath = false;
                    Debug.Log("Follow stopped.");
                    return;
                }

                if (pathingIndex >= aStar.finalNodePath.Count) // stop following once we've reached the end of our node list (end of the path)
                {
                    followingSinglePath = false;
                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position,
                                                         aStar.finalNodePath[pathingIndex].worldPosition,
                                                         moveSpeed); // move to next node in our current path

                if (Vector3.Distance(transform.position, aStar.finalNodePath[pathingIndex].worldPosition) < 0.2f)
                {
                    pathingIndex++; // increment index for the node to move to in the path
                }
            }


            // loop for following paths in a loop
            if (followingPatrolPath)
            {
                if (stopFollowingPath)
                {
                    stopFollowingPath = false;
                    followingPatrolPath = false;
                    Debug.Log("Patrol stopped.");
                    return;
                }

                if (pathingIndex >= aStar.finalNodePath.Count)
                {
                    pathingIndex = 0; // reset node index to start next path fresh
                    patrollingNodeIndex++;

                    if (patrollingNodeIndex >= patrolNodes.Count - 1)  // set path from last node to the first if we've reaached the last node
                    {
                        aStar.FindPath(patrolNodes[patrolNodes.Count - 1], patrolNodes[0]);
                        patrollingNodeIndex = -1;
                    }
                    else
                    {
                        aStar.FindPath(patrolNodes[patrollingNodeIndex], patrolNodes[patrollingNodeIndex + 1]);

                        if (aStar.status == AStar.PathingStatus.pathNotFound)
                        {
                            // set new path and exit loop if that path is invalid.
                            Debug.LogError("Something went wrong with finding the next path point. Patrol was cancelled.");
                            followingPatrolPath = false;
                            return;
                        }
                    }
                }

                transform.position = Vector3.MoveTowards(transform.position,
                                             aStar.finalNodePath[pathingIndex].worldPosition,
                                             moveSpeed); // move to next node


                if (Vector3.Distance(transform.position, aStar.finalNodePath[pathingIndex].worldPosition) < 0.2f)
                {
                    pathingIndex++;
                }

            }

        }


        void OnDrawGizmos()
        {
            if (showNodePath)
            {
                Gizmos.color = nodePathColor;

                for (int i = 0; i < aStar.finalNodePath.Count; i++)
                {
                    if (aStar.finalNodePath[i] != null)
                        Gizmos.DrawSphere(aStar.finalNodePath[i].worldPosition, .7f);
                }
            }
        }

    }

}
