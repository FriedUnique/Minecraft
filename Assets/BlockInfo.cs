using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo
{
    public Vector2 top;
    public Vector2 side;
    public Vector2 bottom;

    public BlockInfo(Vector2 t, Vector2 s, Vector2 b) {
        top = t;
        side = s;
        bottom = b;
    }

    /// <summary>
    /// BlockInfo stores information about the position of the texture of the block and its different faces in the texture atlas
    /// </summary>
    /// <param name="face"></param>
    public BlockInfo(Vector2 face) {
        top = face;
        side = face;
        bottom = face;
    }
}
