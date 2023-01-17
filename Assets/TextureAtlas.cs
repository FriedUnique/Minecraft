using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas
{
    public Material textureAtlasMat;
    public float resolution = 16f;

    public TextureAtlas(string AtlasMaterialName, float res) {
        textureAtlasMat = Resources.Load("Materials/ATLAS", typeof(Material)) as Material;
        //Debug.Log(textureAtlasMat);
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
}