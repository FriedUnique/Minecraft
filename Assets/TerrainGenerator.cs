using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    #region Singleton

    private static TerrainGenerator _instance;

    public static TerrainGenerator instance {
        get {
            if(_instance == null) {
                Debug.LogError("TerrainGenerator is null!");
            }
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    #endregion


    [HideInInspector] public TextureAtlas ta;
    public string textureName; // texture Atlas Nmae
    public int renderD; // the renderdistance is actually 6 when including the chunk in which the player is atm
    public GameObject chunk;

    public Dictionary<Vector2, Chunk> loadedChunks; // x,z cCoords
    public FastNoise noise;


    private ObjectPooler pooler;
    private Player player;
    private Vector2 lastPlayerChunk;

    private void Start() {
        ta = new TextureAtlas(textureName, 16f);
        player = FindObjectOfType<Player>();
        pooler = gameObject.GetComponent<ObjectPooler>(); // do we really need a object pooler for chunks?
        loadedChunks = new Dictionary<Vector2, Chunk>();
        lastPlayerChunk = player.GetCurrentChunk();
        noise = new FastNoise(1337);


        for (int x = renderD; x > -renderD-1; x--) {
            for (int z = renderD; z > -renderD-1; z--) {
                if (loadedChunks.ContainsKey(new Vector2(x, z))) continue;

                //loadedChunks.Add(new Vector2(x, z), pooler.spawnFromPool("chunks", Vector3.zero, Quaternion.identity, new object[2] { x, z }).GetComponent<Chunk>());
                pooler.spawnFromPool("chunks", new Vector3(x * 16, 0, z * 16), Quaternion.identity, new object[3] { x, z, noise }).GetComponent<Chunk>().AddToList();

                //loadedChunks.Add(new Vector2(x * 16, z * 16), pooler.spawnFromPool("chunks", new Vector3(x*16, 0 , z*16), Quaternion.identity, new object[] { x*16, z*16 }).GetComponent<Chunk>());
            }
        }
        
    }

    private void Update() {
        if (lastPlayerChunk != player.GetCurrentChunk()) {
            LoadChunks();
        } 
    }

    private void LoadChunks() {
        // negate??

        Vector2 cCoords = player.GetCurrentChunk();
        List<Vector2> toRemove = new List<Vector2>();

        // check for Chunks out of renderdistance and despawn them
        foreach (Chunk chunk in loadedChunks.Values) {
            if (chunk.chunkPos.x < cCoords.x - renderD) {
                toRemove.Add(chunk.chunkPos);
                int x = Mathf.Abs((int)cCoords.x - (int)chunk.chunkPos.x);
                chunk.OnPooled(new object[] { (int)cCoords.x + x - 1, (int)chunk.chunkPos.y });
                continue;

            } else if (chunk.chunkPos.x > cCoords.x + renderD) {
                toRemove.Add(chunk.chunkPos);
                int x = Mathf.Abs((int)cCoords.x - (int)chunk.chunkPos.x);
                chunk.OnPooled(new object[] { (int)cCoords.x - x + 1, (int)chunk.chunkPos.y });
                continue;
            }

            if (chunk.chunkPos.y < cCoords.y - renderD) {
                toRemove.Add(chunk.chunkPos);
                int y = Mathf.Abs((int)cCoords.y - (int)chunk.chunkPos.y);
                chunk.OnPooled(new object[] { (int)chunk.chunkPos.x, (int)cCoords.y + y - 1 });

            } else if (chunk.chunkPos.y > cCoords.y + renderD) {
                toRemove.Add(chunk.chunkPos);
                int y = Mathf.Abs((int)cCoords.y - (int)chunk.chunkPos.y);
                chunk.OnPooled(new object[] { (int)chunk.chunkPos.x, (int)cCoords.y - y + 1 });
            }
        }

        foreach (Vector2 v in toRemove) {
            //Chunk c = loadedChunks[v];
            loadedChunks.Remove(v);
            //c.AddToList();
        }

        lastPlayerChunk = cCoords;
    }




    /*
    // maybe only check on new chunk entry
    private void L() {
        Vector2 cCoords = player.GetCurrentChunk();
        List<Chunk> despawnedChunks = new List<Chunk>();

        // check for Chunks out of renderdistance and despawn them
        foreach(Chunk chunk in loadedChunks.Values) { 
            if(chunk.chunkPos.x < cCoords.x - renderD || chunk.chunkPos.y < cCoords.y - renderD) {
                despawnedChunks.Add(chunk);
            }
            else if(chunk.chunkPos.x > cCoords.x + renderD || chunk.chunkPos.y > cCoords.y + renderD) {
                despawnedChunks.Add(chunk);
            }
        }


        foreach (Chunk c in despawnedChunks) {
            loadedChunks.Remove(loadedChunks.FirstOrDefault(x => x.Value == c).Key); // get key from value
            c.Despawn();
        }

        if (lastCCoord.x == cCoords.x && lastCCoord.y == cCoords.y) return;
        for (int x = renderD + (int)cCoords.x; x > -renderD + (int)cCoords.x - 1; x--) {
            for (int z = renderD + (int)cCoords.y; z > -renderD + (int)cCoords.y - 1; z--) {
                if (loadedChunks.ContainsKey(new int[] { x, z })) continue;

                loadedChunks.Add(new int[] { x, z }, pooler.spawnFromPool("chunks", Vector3.zero, Quaternion.identity, new object[] { x, z }).GetComponent<Chunk>());
            }
        }

        lastCCoord = cCoords;
    }
    */

}


/*
 
 // maybe only check on new chunk entry
    private void LoadChunks() {
        Vector2 cCoords = player.GetCurrentChunk();
        List<Chunk> despawnedChunks = new List<Chunk>();

        // check for Chunks out of renderdistance and despawn them
        foreach(Chunk chunk in loadedChunks.Values) { 
            if(chunk.chunkPos.x < cCoords.x - renderD || chunk.chunkPos.y < cCoords.y - renderD) {
                despawnedChunks.Add(chunk);
            }
            else if(chunk.chunkPos.x > cCoords.x + renderD || chunk.chunkPos.y > cCoords.y + renderD) {
                despawnedChunks.Add(chunk);
            }
        }


        foreach (Chunk c in despawnedChunks) {
            loadedChunks.Remove(loadedChunks.FirstOrDefault(x => x.Value == c).Key); // get key from value
            c.Despawn();
        }

        if (lastCCoord.x == cCoords.x && lastCCoord.y == cCoords.y) return;
        for (int x = renderD + (int)cCoords.x; x > -renderD + (int)cCoords.x - 1; x--) {
            for (int z = renderD + (int)cCoords.y; z > -renderD + (int)cCoords.y - 1; z--) {
                if (loadedChunks.ContainsKey(new int[] { x, z })) continue;

                loadedChunks.Add(new int[] { x, z }, pooler.spawnFromPool("chunks", Vector3.zero, Quaternion.identity, new object[] { x, z }).GetComponent<Chunk>());
            }
        }

        lastCCoord = cCoords;
    }
 */