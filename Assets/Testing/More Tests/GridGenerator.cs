using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PathingAPI
{
    // could add a bool for setting y to z?
    public class GridGenerator : MonoBehaviour
    {
        public LayerMask obstacleLayer = 3;

        public int horizontalLineCount = 8;
        public int verticalLineCount = 10;

        [SerializeField] float horizontalSpacing = 2;
        [SerializeField] float verticalSpacing = 4;

        [SerializeField] bool drawGridLines = true;
        [SerializeField] Color lineColor = new Color(1, 0, 0, 0.5f);

        Node[] nodes; // our array of nodes in the grid, each with their own values and properties


        void OnDrawGizmos()
        {

            if (drawGridLines)
            {
                Gizmos.color = lineColor;

                for (int y = 0; y < verticalLineCount + 1; y++)
                {
                    for (int x = 0; x < horizontalLineCount + 1; x++)
                    {
                        // start from this gameobject, and add a line at the vector value corresponding to
                        // the current loop iteration (multiplied by how far lines should be spaced).
                        Gizmos.DrawLine(transform.position + new Vector3(0, y * verticalSpacing, 0),
                                                             new Vector3(horizontalLineCount * horizontalSpacing, y * verticalSpacing, 0));

                        Gizmos.DrawLine(transform.position + new Vector3(x * horizontalSpacing, 0, 0),
                                                             new Vector3(x * horizontalSpacing, verticalLineCount * verticalSpacing, 0));
                    }
                }
            }
        }


        private void Awake()
        {

            nodes = new Node[horizontalLineCount * verticalLineCount]; //set # of nodes with grid size


            for (int y = 0; y < verticalLineCount; y++)
            {
                //nodes[i] = new Node();

                for (int x = 0; x < horizontalLineCount; x++)
                {
                    int currentNode = x + y * horizontalLineCount;

                    // world position should be offset by the grid cell size (adding a half cell to offset centers the node within its cell).
                    // grid position is its position within the grid.
                    nodes[currentNode] = new Node(new Vector3(x * horizontalSpacing + (horizontalSpacing / 2),
                                                  y * verticalSpacing + (verticalSpacing / 2), 0),
                                                  new Vector3Int(x, y, 0)); //true);

                    nodes[currentNode].isTraversable = !Physics.CheckBox(nodes[currentNode].worldPosition,
                                                                        new Vector3(horizontalSpacing, 1, verticalSpacing),
                                                                        Quaternion.identity,
                                                                        obstacleLayer);

                    // create a sphere at the current node's world pos. 
                    //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), nodes[currentNode].worldPosition, Quaternion.identity);

                }
            }
        }

        internal Node GetNode(Vector3Int gridPosition)
        {
            int givenPosition = gridPosition.x + gridPosition.y * horizontalLineCount;

            if (givenPosition > horizontalLineCount * verticalLineCount)
            {
                Debug.LogError("The node index was out of bounds.");
                return null;
            }

            return (nodes[givenPosition]);
        }


    }

}
