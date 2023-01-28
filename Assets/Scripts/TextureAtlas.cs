using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas
{
    public const int blockDataLenght = (Chunk.width+1) * (Chunk.width+1) * (Chunk.height+1); // length of the chunk list
    public static byte[,,] GetEmptyChunkList() {
        return new byte[Chunk.width + 1, Chunk.height + 1, Chunk.width + 1];
    }

    // Material
    public Material textureAtlasMat;
    public float resolution = 16f;

    // Texture
    public TextureAtlas(string AtlasMaterialName, float res) {
        textureAtlasMat = Resources.Load($"Materials/{AtlasMaterialName}", typeof(Material)) as Material;
        resolution = res;
    }
    public Vector2[] GetUVsFromAtlas(Vector2 textureCords) {
        // the mesh has to have the texture atlas as a material
        int textureX = (int)textureCords.x;
        int textureY = (int)textureCords.y;

        return new Vector2[] {
            new Vector2(textureX/resolution, textureY/resolution),
            new Vector2(textureX/resolution, (textureY+1)/resolution-0.01f),
            new Vector2((textureX+1)/resolution-0.01f, (textureY+1)/resolution-0.01f),
            new Vector2((textureX+1)/resolution-0.01f, textureY/resolution)

        };
    }

    // Blocktypes
    public enum BlockType {
        Air,
        Grass,
        Timo,
        Asmir
    }
    public static Dictionary<BlockType, BlockInfo> texturePos = new Dictionary<BlockType, BlockInfo>() {
        { BlockType.Grass, new BlockInfo(new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)) }, // top, side, bottom
        { BlockType.Timo, new BlockInfo(new Vector2(0, 3)) },
        { BlockType.Asmir, new BlockInfo(new Vector2(0, 4)) }
    };

    // Mesh (vertecies and Mesh)
    public static void topFace(ref List<Vector3> meshVerts, Vector3 pos = default(Vector3)) {
        meshVerts.Add(pos + new Vector3(0, 1, 0));
        meshVerts.Add(pos + new Vector3(0, 1, 1));
        meshVerts.Add(pos + new Vector3(1, 1, 1));
        meshVerts.Add(pos + new Vector3(1, 1, 0));
    }
    public static void bottomFace(ref List<Vector3> meshVerts, Vector3 pos = default(Vector3)) {
        meshVerts.Add(pos + new Vector3(0, 0, 0));
        meshVerts.Add(pos + new Vector3(1, 0, 0));
        meshVerts.Add(pos + new Vector3(1, 0, 1));
        meshVerts.Add(pos + new Vector3(0, 0, 1));
    }
    public static void leftFace(ref List<Vector3> meshVerts, Vector3 pos = default(Vector3)) {
        meshVerts.Add(pos + new Vector3(0, 0, 1));
        meshVerts.Add(pos + new Vector3(0, 1, 1));
        meshVerts.Add(pos + new Vector3(0, 1, 0));
        meshVerts.Add(pos + new Vector3(0, 0, 0));
    }
    public static void rightFace(ref List<Vector3> meshVerts, Vector3 pos = default(Vector3)) {
        meshVerts.Add(pos + new Vector3(1, 0, 0));
        meshVerts.Add(pos + new Vector3(1, 1, 0));
        meshVerts.Add(pos + new Vector3(1, 1, 1));
        meshVerts.Add(pos + new Vector3(1, 0, 1));
    }
    public static void frontFace(ref List<Vector3> meshVerts, Vector3 pos = default(Vector3)) {
        meshVerts.Add(pos + new Vector3(1, 0, 1));
        meshVerts.Add(pos + new Vector3(1, 1, 1));
        meshVerts.Add(pos + new Vector3(0, 1, 1));
        meshVerts.Add(pos + new Vector3(0, 0, 1));
    }
    public static void backFace(ref List<Vector3> meshVerts, Vector3 pos = default(Vector3)) {
        meshVerts.Add(pos + new Vector3(0, 0, 0));
        meshVerts.Add(pos + new Vector3(0, 1, 0));
        meshVerts.Add(pos + new Vector3(1, 1, 0));
        meshVerts.Add(pos + new Vector3(1, 0, 0));
    }

    public static Mesh meshGen(List<Vector3> verts, List<Vector2> uvs) {
        Mesh mesh = new Mesh();
        List<int> tris = new List<int>();
        mesh.vertices = verts.ToArray();

        for (int i = 0; i < verts.Count; i += 4) {
            // 4 vertecies for plane; 6 triangle things
            tris.Add(i);
            tris.Add(i + 1);
            tris.Add(i + 2);
            tris.Add(i);
            tris.Add(i + 2);
            tris.Add(i + 3);
        }

        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }
}