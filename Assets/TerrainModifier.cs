using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;

public class TerrainModifier : MonoBehaviour
{
    [SerializeField] private float playerReach;
    [SerializeField] private LayerMask placeable;

    Transform camTransform;
    Player player;
    private TerrainGenerator tg;

    float cooldown = 0.3f;
    float nextPlace;

    private void Start() {
        player = GetComponent<Player>();
        camTransform = player.cam.transform;
        tg = TerrainGenerator.instance;
    }

    private void Update() {
        // raycast
        nextPlace -= Time.deltaTime;
        Debug.DrawRay(camTransform.position + camTransform.forward * 0.5f, camTransform.TransformDirection(Vector3.forward * playerReach), Color.red);

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && nextPlace < 0) {
            Ray ray = new Ray(camTransform.position + camTransform.forward * 0.5f, camTransform.TransformDirection(Vector3.forward * playerReach));

            if (!Physics.Raycast(ray, out RaycastHit hit, playerReach, placeable)) { return; }

            Vector3 p;
            //place
            if (Input.GetMouseButton(1)) { p = hit.point - camTransform.forward * 0.02f; }
            //break
            else { p = hit.point + camTransform.forward * 0.02f; }

            Vector2 chunkPos = new Vector2(Mathf.FloorToInt((p.x) / 16f), Mathf.FloorToInt((p.z) / 16f));
            Chunk chunk = tg.loadedChunks[chunkPos];

            int bX = Mathf.FloorToInt(p.x) - ((int)chunkPos.x*16);
            int bY = Mathf.FloorToInt(p.y);
            int bZ = Mathf.FloorToInt(p.z) - ((int)chunkPos.y*16);


            // check clause if out of bounds
            if (Input.GetMouseButton(0)) {
                chunk.blocksInChunk[bX, bY, bZ] = TextureAtlas.BlockType.Air;
                Debug.Log($"Block in Chunk: {new Vector3(bX, bY, bZ)},  ChunkPos: {chunkPos}");
                //tg.loadedChunks[new Vector2(1, 0)].blocksInChunk = new int[Chunk.chunkWidth + 2, Chunk.chunkMaxSpawnHeight + 1, Chunk.chunkWidth + 2];
            } else {
                /*if (Mathf.Max(Mathf.FloorToInt(p.x) - (chunkPos.x * 16), 1) == Mathf.Max(Mathf.FloorToInt(player.GetPos().x) - (chunkPos.x * 16), 1) && Mathf.Max(Mathf.FloorToInt(p.z) - (chunkPos.y * 16), 1) == Mathf.Max(Mathf.FloorToInt(player.GetPos().z) - (chunkPos.y * 16), 1)) {
                    if (player.GetPos().y - 1 == Mathf.FloorToInt(p.y) || player.GetPos().y + 1 == Mathf.FloorToInt(p.y) || player.GetPos().y == Mathf.FloorToInt(p.y)) {
                        return;
                    }
                }*/
                chunk.blocksInChunk[bX, bY, bZ] = TextureAtlas.BlockType.Timo;
            }

            chunk.BuildMesh();
            nextPlace = cooldown;

        }
        
    }
}

/*

        nextPlace -= Time.deltaTime;
        Debug.DrawRay(camTransform.position + camTransform.forward*0.5f, camTransform.TransformDirection(Vector3.forward * playerReach), Color.red);

        if (Input.GetMouseButton(1) && nextPlace < 0) {
            Ray ray = new Ray(camTransform.position + camTransform.forward * 0.5f, camTransform.TransformDirection(Vector3.forward * playerReach));

            if (!Physics.Raycast(ray, out RaycastHit hit, playerReach, placeable)) { return; }

            Vector3 p = hit.point - camTransform.forward * 0.5f; // move hit point towards player, so block is not in block
            int nX = (int)p.x / Mathf.Abs((int)p.x);
            int x = Mathf.FloorToInt((hit.point.x-1) / 16f);
            int z = Mathf.FloorToInt((hit.point.z-1) / 16f);

            // player in chunk pos and hit in chunk pos
            if (Mathf.Max(Mathf.FloorToInt(p.x) - (x * 16), 0) == Mathf.Max(Mathf.FloorToInt(player.GetPos().x) - (x * 16), 0) && Mathf.Max(Mathf.FloorToInt(p.z) - (z * 16), 0) == Mathf.Max(Mathf.FloorToInt(player.GetPos().z) - (z * 16), 0)) {
                if (player.GetPos().y-1 == Mathf.FloorToInt(p.y) || player.GetPos().y + 1 == Mathf.FloorToInt(p.y) || player.GetPos().y == Mathf.FloorToInt(p.y)) {
                    return;
                }
            }
            Chunk chunk = TerrainGenerator.instance.loadedChunks[new Vector2(x, z)];

            //Debug.Log($"x:{x}, z:{z}, hitPoint: {pp}, {p}");
            //Debug.Log($"{Mathf.Max(Mathf.FloorToInt(p.x) - (x * 16), 0)}, {Mathf.FloorToInt(p.y - 0.2f)}, {Mathf.Max(Mathf.FloorToInt(p.z) - (z * 16), 0)}");

            int bX = Mathf.FloorToInt(p.x - 0.2f) - (x * 16);
            int bY = Mathf.FloorToInt(p.y - 0.2f);
            int bZ = Mathf.FloorToInt(p.z - 0.2f) - (z * 16);

            chunk.blocksInChunk[bX, bY, bZ] = 2;

            chunk.BuildMesh();

            nextPlace = cooldown;

        }
*/


/*
 * 
        int min = Mathf.Abs(16 * (int)chunk.chunkPos.x - 16);
        int max = Mathf.Abs(16 * (int)chunk.chunkPos.x);
        int blockX = (int)p.x - max + min - 1;

        min = Mathf.Abs(16 * (int)chunk.chunkPos.y - 16);
        max = Mathf.Abs(16 * (int)chunk.chunkPos.y);
        int blockZ = (int)p.y - max + min - 1;

        Debug.Log(new Vector2(blockX, blockZ));

*/
