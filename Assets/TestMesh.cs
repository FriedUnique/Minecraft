using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BuildMesh();
    }

    void BuildMesh() {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[] {
            // side x
            new Vector3(0, -0.5f, 0),
            new Vector3(0, 0.5f, 0),
            new Vector3(0, 0.5f, 1),
            new Vector3(0, -0.5f, 1),

            new Vector3(1, -0.5f, 0),
            new Vector3(1, 0.5f, 0),
            new Vector3(1, 0.5f, 1),
            new Vector3(1, -0.5f, 1),

            // front z
            new Vector3(0, -0.5f, 1), // 4
            new Vector3(0, 0.5f, 1), // 5
            new Vector3(1, 0.5f, 1), // 6
            new Vector3(1, -0.5f, 1), // 7

            new Vector3(0, -0.5f, 0), // 4
            new Vector3(0, 0.5f, 0), // 5
            new Vector3(1, 0.5f, 0), // 6
            new Vector3(1, -0.5f, 0),

            new Vector3(0, 0.5f, 0), // 8
            new Vector3(0, 0.5f, 1), // 9
            new Vector3(1, 0.5f, 1), // 10
            new Vector3(1, 0.5f, 0), // 11

            new Vector3(0, -0.5f, 0), // 12
            new Vector3(0, -0.5f, 1), // 13
            new Vector3(1, -0.5f, 1), // 14
            new Vector3(1, -0.5f, 0) // 15

        };


        mesh.triangles = new int[] { 
            0, 1, 2, 0, 2, 3, 
            4, 5, 6, 4, 6, 7, 
            8, 9, 10, 8, 10, 11, 
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 19, 19,
            20, 21, 22, 20, 22, 23
        }; //4, 5, 6, 4, 6, 7   8, 9, 10, 8, 10, 11   12, 13, 14, 12, 14, 15

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
