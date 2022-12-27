using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PathingAPI
{
    internal class Node : IComparable
    {
        public float gCost;
        public float hCost;

        // when someone asks for this node's fcost, return gcost + hcost.
        public float FCost
        {
            get { return gCost + hCost; }
        }


        // these properties can be checked but not changed outside this class.
        public Vector3 worldPosition { get; private set; }
        public Vector3Int gridPosition { get; private set; }


        public bool isTraversable;

        public bool hasBeenChecked = false;

        public Node parent { get; set; }



        // set these values in constructor, so we can't set them again (these values shouldn't change)
        public Node(Vector3 worldPosition, Vector3Int gridPosition)
        {
            this.worldPosition = worldPosition;
            this.gridPosition = gridPosition;
        }


        // calculate distance cost between the given node and this one. used for both G Cost and H Cost.
        public float CalculateCost(Node relativeTo)
        {
            // get Euclidean distance. d = sqrt((x2 – x1)^2 + (y2 – y1)^2)
            float xSquare = (relativeTo.worldPosition.x - this.worldPosition.x) * (relativeTo.worldPosition.x - this.worldPosition.x);
            float ySquare = (relativeTo.worldPosition.y - this.worldPosition.y) * (relativeTo.worldPosition.y - this.worldPosition.y);

            float distance = Mathf.Sqrt(xSquare + ySquare);

            return distance;
        }

        // old version of above func
        int CalculateDistance(Vector3 pointA, Vector3 pointB)
        {
            int distance = (int)(Mathf.Abs(pointB.x - pointA.x) + Mathf.Abs(pointB.y - pointA.y));
            return distance;
        }

        // use theicomparable inheritance to override the list sort func with this function.
        public int CompareTo(object obj)
        {
            Node node = (Node)obj;

            if (node.FCost > this.FCost)
                return -1;

            else if (node.FCost < this.FCost) // put this node in front of the compared node in the list when its fcost is smaller
                return 1;

            else return 0;
        }

    }

}
