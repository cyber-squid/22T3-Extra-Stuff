using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PathingAPI
{
    public class AStar
    {
        public GridGenerator grid;

        Node startingNode;
        Node goalNode;

        //[SerializeField] Vector3Int startingPos;
        //[SerializeField] Vector3Int goalPos;

        bool leftNodeActive, rightNodeActive, upNodeActive, downNodeActive = false;

        List<Node> neighborNodes = new List<Node>(8);

        Node currentNode;
        List<Node> openNodesList = new List<Node>(1000);
        List<Node> closedNodeList = new List<Node>(1000);

        internal List<Node> finalNodePath = new List<Node>(500);

        public enum PathingStatus
        {
            pathFound,
            pathNotFound,
            locatingPath
        }

        public PathingStatus status = PathingStatus.pathNotFound;

        void OnDrawGizmos()
        {

            Gizmos.color = Color.magenta;
            if (startingNode != null)
                Gizmos.DrawSphere(startingNode.worldPosition, 1f);

            Gizmos.color = Color.yellow;
            if (goalNode != null)
                Gizmos.DrawSphere(goalNode.worldPosition, 1f);


            Gizmos.color = Color.cyan;

            for (int i = 0; i < finalNodePath.Count; i++)
            {
                if (finalNodePath[i] != null)
                    Gizmos.DrawSphere(finalNodePath[i].worldPosition, .7f);

            }


        }

        internal void FindPath(Node start, Node goal)
        {
            status = PathingStatus.locatingPath;

            // clear all lists, to start fresh
            for (int i = 0; i < closedNodeList.Count; i++)
            {
                closedNodeList[i].parent = null;
                closedNodeList[i].hasBeenChecked = false;
            }

            finalNodePath.Clear();
            openNodesList.Clear();
            closedNodeList.Clear();

            // get start node and end node
            startingNode = start;
            goalNode = goal;

            // make the start node the first node we start working from
            openNodesList.Add(startingNode);


            // get the neighbours of the current node, determine their costs and whether to add them to the viable node list
            while (openNodesList.Count > 0)
            {
                openNodesList.Sort();
                currentNode = openNodesList[0]; // sort by lowest f cost, select the lowest node (shortest path)

                openNodesList.RemoveAt(0);
                currentNode.hasBeenChecked = true;
                closedNodeList.Add(currentNode); // take it out of the open list and add it to the closed list

                //Node[] currentNeighbours = new Node[GetNeighborNodes(currentSelectedNode)];

                if (currentNode == goalNode)
                {
                    // return true to signal successful run, then allow the manager to use GetPath if true
                    GetPath(goalNode);
                    finalNodePath.Reverse();

                    status = PathingStatus.pathFound;
                    return;
                }


                GetNeighbours(currentNode); // get each of the neighbours in valid grid space.


                for (int i = 0; i < neighborNodes.Count; i++) // check each neighbour node:
                {
                    if (neighborNodes[i].isTraversable && !neighborNodes[i].hasBeenChecked) //!closedNodes.Contains(neighborNodes[i])) //
                    {

                        neighborNodes[i].hCost = neighborNodes[i].CalculateCost(goalNode); // set node's h cost

                        bool isInOpenList = openNodesList.Contains(neighborNodes[i]);
                        float currentGCost = currentNode.CalculateCost(neighborNodes[i]) + currentNode.gCost;

                        // we want to set up the node with fcost and parent if we haven't yet.
                        // we also want to reset gcost and parent if we find a better one.
                        if (!isInOpenList || currentGCost < neighborNodes[i].gCost)
                        {
                            neighborNodes[i].gCost = currentGCost;
                            neighborNodes[i].parent = currentNode;

                            if (!isInOpenList)
                            {
                                openNodesList.Add(neighborNodes[i]);
                            }

                        }
                    }
                }

                neighborNodes.Clear();

            }

            // list is empty, but still didn't find a path, meaning no valid path
            status = PathingStatus.pathNotFound;

        }



        void GetNeighbours(Node checkedNode)
        {
            // check if the space above, right, left, and below the node are valid, and add them to the neighbor nodes if so.
            if (checkedNode.gridPosition.y < grid.verticalLineCount - 1)
            {
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.up));
                upNodeActive = true;
            }

            if (checkedNode.gridPosition.x < grid.horizontalLineCount - 1)
            {
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.right));
                rightNodeActive = true;
            }

            if (checkedNode.gridPosition.x > 0)
            {
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.left));
                leftNodeActive = true;
            }

            if (checkedNode.gridPosition.y > 0)
            {
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.down));
                downNodeActive = true;
            }


            // set diagonal nodes depending on the current active neighbour nodes
            if (rightNodeActive && upNodeActive)
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.right + Vector3Int.up));

            if (leftNodeActive && upNodeActive)
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.left + Vector3Int.up));

            if (rightNodeActive && downNodeActive)
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.right + Vector3Int.down));

            if (leftNodeActive && downNodeActive)
                neighborNodes.Add(grid.GetNode(checkedNode.gridPosition + Vector3Int.left + Vector3Int.down));


            leftNodeActive = rightNodeActive = upNodeActive = downNodeActive = false; // reset bools after we're done finding neighbours
        }

        // add a parent node to our final list of nodes (and get its parent and repeat the process).
        void GetPath(Node refNode)
        {
            finalNodePath.Add(refNode);

            if (refNode.parent != null)
            {
                GetPath(refNode.parent);
            }
        }

        void CheckSurroundingNodes(Node neighborArray)
        {
            ///neighborArray.gCost = neighborArray.CalculateCost(startingNode);
            ///neighborArray.hCost = neighborArray.CalculateCost(goalNode);

            Node currentSelectedNode;

            foreach (Node neighbor in neighborNodes)
            {
                if (neighbor.FCost == Mathf.Min(neighborNodes[neighborNodes.Count].FCost))
                {

                    currentSelectedNode = neighbor;
                    //closedNodes.Add(currentSelectedNode);
                }
            }
        }

        void SetStartNode(Node node)
        {

            startingNode = node;
        }

    }
}
