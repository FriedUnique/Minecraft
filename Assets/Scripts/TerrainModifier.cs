using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                chunk.blocksInChunk[bX, bY, bZ] = (int)TextureAtlas.BlockType.Air;
            } else {
                if (Mathf.Max(Mathf.FloorToInt(p.x) - (chunkPos.x * 16), 1) == Mathf.Max(Mathf.FloorToInt(player.GetPos().x) - (chunkPos.x * 16), 1) && Mathf.Max(Mathf.FloorToInt(p.z) - (chunkPos.y * 16), 1) == Mathf.Max(Mathf.FloorToInt(player.GetPos().z) - (chunkPos.y * 16), 1)) {
                    if (player.GetPos().y - 1 == Mathf.FloorToInt(p.y) || player.GetPos().y + 1 == Mathf.FloorToInt(p.y) || player.GetPos().y == Mathf.FloorToInt(p.y)) {
                        return;
                    }
                }
                chunk.blocksInChunk[bX, bY, bZ] = (int)TextureAtlas.BlockType.Timo;
            }

            chunk.BuildMesh();
            nextPlace = cooldown;

        }
        
    }
}
