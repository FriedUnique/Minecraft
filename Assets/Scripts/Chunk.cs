using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int width = 16;
    public const int height = 64;

    public int[,,] blocksInChunk;

    public Vector2 chunkPos;

    private TextureAtlas textureAtlas;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private int chunkX, chunkZ;

    public void initChunk(TextureAtlas atlas) {
        textureAtlas = atlas;
        GetComponent<MeshRenderer>().material = atlas.textureAtlasMat;

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void updateChunk(Vector2 coords) {
        chunkX = (int)coords.x;
        chunkZ = (int)coords.y;
        chunkPos = coords;
        transform.position = new Vector3(chunkX * 16, 0, chunkZ * 16);

        BuildMesh();
    }


    /// <summary>
    /// Creates the vertecies, triangels and uvs for the mesh chunk
    /// </summary>
    public void BuildMesh() {
        List<Vector3> meshVerts = new List<Vector3>();
        List<Vector2> meshUvs = new List<Vector2>();

        // calculate all the vertisies
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < width; z++) {
                for (int y = 0; y < height + 1; y++) {

                    if (blocksInChunk[x, y, z] == 0) continue;

                    TextureAtlas.BlockType b = (TextureAtlas.BlockType)blocksInChunk[x, y, z];

                    Vector3 pos = new Vector3(x, y, z);

                    // top face
                    if (blocksInChunk[x, y + 1, z] == 0) {
                        TextureAtlas.topFace(ref meshVerts, pos);
                        meshUvs.AddRange(textureAtlas.GetUVsFromAtlas(TextureAtlas.texturePos[b].top));
                    }
                    // bottom face
                    if (blocksInChunk[x, Mathf.Max(y - 1, 0), z] == 0) {
                        TextureAtlas.bottomFace(ref meshVerts, pos);
                        meshUvs.AddRange(textureAtlas.GetUVsFromAtlas(TextureAtlas.texturePos[b].bottom));
                    }
                    // right
                    if (blocksInChunk[Mathf.Min(x + 1, width + 1), y, z] == 0) {
                        TextureAtlas.rightFace(ref meshVerts, pos);
                        meshUvs.AddRange(textureAtlas.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                    // left  
                    if (x - 1 < 0 || blocksInChunk[x-1, y, z] == 0) {
                        TextureAtlas.leftFace(ref meshVerts, pos);
                        meshUvs.AddRange(textureAtlas.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                    // front
                    if (blocksInChunk[x, y, Mathf.Min(z + 1, width + 1)] == 0) {
                        TextureAtlas.frontFace(ref meshVerts, pos);
                        meshUvs.AddRange(textureAtlas.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                    // bacl 
                    if (z - 1 < 0 || blocksInChunk[x, y, z - 1] == 0) {
                        TextureAtlas.backFace(ref meshVerts, pos);
                        meshUvs.AddRange(textureAtlas.GetUVsFromAtlas(TextureAtlas.texturePos[b].side));
                    }
                }
            }
        }

        Mesh mesh = TextureAtlas.meshGen(meshVerts, meshUvs);
        meshFilter.mesh = mesh;
        //Physics.BakeMesh(mesh.GetInstanceID(), false);
        meshCollider.sharedMesh = mesh;
    }
}
