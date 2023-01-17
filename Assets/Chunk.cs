using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour, IPooledObject
{
    public const int chunkWidth = 16;
    public const int chunkMaxSpawnHeight = 64;
    public const float coeff = 1 / chunkWidth; // ?

    public TextureAtlas.BlockType[,,] blocksInChunk;

    public Vector2 chunkPos;
    public float chunkScale = 1f; // scales the chunk

    private FastNoise fn;
    private TextureAtlas ta;
    private int chunkX, chunkZ;

    public void OnPooled(object[] args) {
        chunkX = (int)args[0];
        chunkZ = (int)args[1];
        if(args.Length > 2)
            fn = (FastNoise)args[2];

        chunkPos = new Vector2(chunkX, chunkZ);
        blocksInChunk = new TextureAtlas.BlockType[chunkWidth + 2, chunkMaxSpawnHeight + 1, chunkWidth + 2];

        AddToList();

        LoadChunkData();
        BuildMesh();
    }


    void LoadChunkData() {
        for (int x = 0; x < chunkWidth+1; x++) {
            for (int z = 0; z < chunkWidth+1; z++) {
                for (int y = 0; y < chunkMaxSpawnHeight + 2; y++) {
                    int xC = x + (chunkX * chunkWidth);
                    int zC = z + (chunkZ * chunkWidth);

                    //Mathf.PerlinNoise(xC * 0.06f, zC * 0.06f) * 10 + y < chunkMaxSpawnHeight * 0.5f

                    if (fn.GetSimplex(xC, zC) * 10 + y < chunkMaxSpawnHeight * 0.5f) {
                        blocksInChunk[x, y, z] = TextureAtlas.BlockType.Grass;
                    }
                }
            }
        }
    }

    /* Not a good solution for modifying chunks*/
    public void BuildMesh() {
        // creates the verts, tris and uvs

        ta = TerrainGenerator.instance.ta;
        GetComponent<MeshRenderer>().material = ta.textureAtlasMat;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        List<Vector3> meshVerts = new List<Vector3>();
        List<int> meshTris = new List<int>();
        List<Vector2> meshUvs = new List<Vector2>();

        // calculate all the vertisies
        for (int x = 0; x < chunkWidth; x++) {
            for (int z = 0; z < chunkWidth; z++) {
                for (int y = 0; y < chunkMaxSpawnHeight + 1; y++) {

                    if (blocksInChunk[x, y, z] == 0) continue;

                    int xC = x + (chunkX * chunkWidth);
                    int zC = z + (chunkZ * chunkWidth);
                    TextureAtlas.BlockType b = blocksInChunk[x, y, z];

                    Vector3 pos = new Vector3(x, y, z) * chunkScale;

                    // top face
                    if (blocksInChunk[x, y + 1, z] == 0) {
                        meshVerts.Add(pos + new Vector3(0, 1, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 1, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 1, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 1, 0) * chunkScale);

                        meshUvs.AddRange(ta.GetUVsFromAtlas(TextureAtlas.texturePos[b].top));
                    }

                    // bottom face
                    if (blocksInChunk[x, Mathf.Max(y - 1, 0), z] == 0) {
                        meshVerts.Add(pos + new Vector3(0, 0, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 0, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 0, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 0, 1) * chunkScale);

                        meshUvs.AddRange(ta.GetUVsFromAtlas(TextureAtlas.texturePos[b].bottom));
                    }

                    // side faces
                    if (blocksInChunk[Mathf.Min(x + 1, chunkWidth + 1), y, z] == 0) {
                        meshVerts.Add(pos + new Vector3(1, 0, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 1, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 1, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 0, 1) * chunkScale);

                        meshUvs.AddRange(ta.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                    if (blocksInChunk[Mathf.Max(x - 1, 0), y, z] == 0) {
                        meshVerts.Add(pos + new Vector3(0, 0, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 1, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 1, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 0, 0) * chunkScale);

                        meshUvs.AddRange(ta.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                    if (blocksInChunk[x, y, Mathf.Min(z + 1, chunkWidth + 1)] == 0) {
                        meshVerts.Add(pos + new Vector3(1, 0, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 1, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 1, 1) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 0, 1) * chunkScale);

                        meshUvs.AddRange(ta.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                    if (blocksInChunk[x, y, Mathf.Max(z - 1, 0)] == 0) {
                        meshVerts.Add(pos + new Vector3(0, 0, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(0, 1, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 1, 0) * chunkScale);
                        meshVerts.Add(pos + new Vector3(1, 0, 0) * chunkScale);

                        meshUvs.AddRange(ta.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }

                }
            }
        }

        mesh.vertices = meshVerts.ToArray();

        for (int i = 0; i < meshVerts.Count; i+=4) {
            // 4 vertecies for plane; 6 triangle things
            meshTris.Add(i);
            meshTris.Add(i+1);
            meshTris.Add(i+2);
            meshTris.Add(i);
            meshTris.Add(i+2);
            meshTris.Add(i+3);
        }

        mesh.triangles = meshTris.ToArray();
        mesh.uv = meshUvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        // add the mesh to the mesh collider
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }



    public void AddToList() {
        if (TerrainGenerator.instance.loadedChunks.ContainsKey(chunkPos) == false) {
            TerrainGenerator.instance.loadedChunks.Add(chunkPos, this);
        }

        transform.position = new Vector3(chunkX*16, 0, chunkZ*16);
    }
}
