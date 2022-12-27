using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


/// <summary>
/// Code is sourced from my previous term's work. Generates a mesh in a given texture within a thread,
/// by first getting its colors, then using the brightness of pixels to set up an array of vertices and indices. 
/// 
/// NOTICE: The thread itself works fine, but the code inside is buggy...
/// </summary>
public class ThreadedTriRenderer : MonoBehaviour
{

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    [SerializeField] Texture2D heightmap;

    [SerializeField] int RectCountOnX = 5;
    [SerializeField] int RectCountOnZ = 5;  // number of rectangles each row.
    [SerializeField] float heightMultiplier = 1f;

    int vertexCountX = 0;
    int vertexCountZ = 0;

    int totalVertices = 0;
    int totalIndices = 0;

    Vector3[] vertices;
    int[] indices;
    Color[] colors;
    //Unity.Collections.NativeArray<Color32> heights;
    Color[] heights;
    float heightmapWidth;
    float heightmapHeight;


    [SerializeField] float scale = 1; // size of the grid, aka spacing between vertices

    Thread thread;
    bool threadFinished;

    private static object generatorLock = new object();


    void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material.color = Color.white;

        thread = new Thread(GenerateTerrainMesh);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (thread.IsAlive)
            {
                Debug.Log("thread is in progress, please wait.");
                return;
            }

            heights = heightmap.GetPixels();
            heightmapWidth = heightmap.width;
            heightmapHeight = heightmap.height;

            thread = new Thread(GenerateTerrainMesh);

            thread.Start();
            Debug.Log("initiating thread");
        }

        if (threadFinished)
        {
            Debug.Log("applying thread results");
            threadFinished = false;

            SetTerrainMesh();
        }
    }


    void SetTerrainMesh()
    {
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = indices;
        meshFilter.mesh.colors = colors;

        //meshRenderer.material = new Material(shader);

        Debug.Log($"number of vertices: {vertices.Length}\nnumber of indices: {indices.Length}");
    }

    void GenerateTerrainMesh()
    {
        Debug.Log("thread was started");

        int currentVertex = 0;
        int currentRect = 0;

        // stop this from being executed by anything else before we finish making our terrain values
        lock (generatorLock)
        {
            // determine the number of vertices and indices to create.
            vertexCountX = RectCountOnX + 1;
            vertexCountZ = RectCountOnZ + 1;

            totalVertices = vertexCountX * vertexCountZ;
            totalIndices = (RectCountOnX * RectCountOnZ) * 6;

            vertices = new Vector3[totalVertices];
            indices = new int[totalIndices];
            colors = new Color[totalVertices];



            /// Vertices
            float xRatio = (heightmapWidth / vertexCountX);
            float zRatio = (heightmapHeight / vertexCountZ); // get the ratio of how many pixels there are for each vertex.

            //Color[] heights = heightmap.GetPixels();// (int)(x * xRatio), (int)(z * zRatio));

            // iterate through each vertex, going through x axis first, then z.
            for (int z = 0; z < vertexCountZ; z++)
            {
                for (int x = 0; x < vertexCountX; x++)
                {
                    int vertexIndex = x + z * vertexCountX;

                    // stop mesh size going out of bounds.
                    if (xRatio < 1f)
                    {
                        xRatio = 1f;
                        print("Warning: The mesh width on X was set wider than the given heightmap size.");
                    }

                    if (zRatio < 1f)
                    {
                        zRatio = 1f;
                        print("Warning: The mesh width on Z was set wider than the given heightmap size.");
                    }

                    // get the pixel coordinate of the given heightmap point, rounding out with the ratio factor (coords must be int).
                    Color height = heights[(int)(x * xRatio) + ((int)(z * zRatio) * vertexCountX)];

                    // set the position and height of the vertex using loop position, and the pixel color we got.
                    colors[vertexIndex] = height;
                    vertices[vertexIndex] = new Vector3(x * scale, height.grayscale * heightMultiplier, -z * scale);


                }
            }


            /// Indices
            for (int z = 0; z < RectCountOnZ; z++)
            {
                for (int x = 0; x < RectCountOnX; x++)
                {
                    indices[currentRect + 0] = currentVertex;
                    indices[currentRect + 1] = currentVertex + 1;
                    indices[currentRect + 2] = currentVertex + vertexCountX + 1; //should be amount on x
                    indices[currentRect + 3] = currentVertex + vertexCountX + 1;
                    indices[currentRect + 4] = currentVertex + vertexCountX;
                    indices[currentRect + 5] = currentVertex;


                    currentVertex++;
                    currentRect += 6;
                }

                currentVertex++;
            }

        }

        threadFinished = true;
        Debug.Log("thread done");
    }


}
