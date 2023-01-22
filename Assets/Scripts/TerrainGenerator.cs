using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
    #region Singleton

    private static TerrainGenerator _instance;

    public static TerrainGenerator instance {
        get {
            if (_instance == null) {
                Debug.LogError("TerrainGenerator is null!");
            }
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    #endregion

    public TextureAtlas atlas;
    public string textureName; // texture Atlas Nmae
    public int renderDistance; // the renderdistance is actually 6 when including the chunk in which the player is atm
    public GameObject chunkPrefab;

    [HideInInspector] public Dictionary<Vector2, Chunk> loadedChunks; // x,z cCoords
    [HideInInspector] public FastNoise noise;
    [HideInInspector] public Player player;


    private Vector2 lastPlayerChunk;
    List<Vector2> toGenerate = new List<Vector2>();
    List<Chunk> pooledChunks = new List<Chunk>();

    private void Start() {
        atlas = new TextureAtlas(textureName, 16f);
        loadedChunks = new Dictionary<Vector2, Chunk>();
        player = FindObjectOfType<Player>();
        noise = new FastNoise(1337);
        Vector2 playerChunkSpawn = new Vector2(1, 0);

        a(instant: true, playerChunk: playerChunkSpawn);

        //player = Instantiate(playerPrefab, new Vector3(playerChunkSpawn.x*16, 45, playerChunkSpawn.y*16), Quaternion.identity).GetComponent<Player>();
    }

    private void Update() {
        a(playerChunk: player.GetCurrentChunk());
    }

    private void BuildChunk(int x, int z) {
        Chunk chunk;
        if (pooledChunks.Count > 0){
            chunk = pooledChunks[0];
            chunk.gameObject.SetActive(true);
            pooledChunks.RemoveAt(0);
            chunk.transform.position = new Vector3(x * 16, 0, z * 16);
        } else {
            chunk = Instantiate(chunkPrefab, new Vector3(x * 16, 0, z * 16), Quaternion.identity).GetComponent<Chunk>();
            chunk.initChunk(atlas);
        }

        chunk.blocksInChunk = LoadChunkData(x, z);
        chunk.updateChunk(new Vector2(x, z));

        loadedChunks.Add(new Vector2(x, z), chunk);
    }

    public int[,,] LoadChunkData(int chunkX, int chunkZ) {
        int[,,] blocks = new int[Chunk.width + 2, Chunk.height + 1, Chunk.width + 2];

        for (int x = 0; x < Chunk.width + 1; x++) {
            for (int z = 0; z < Chunk.width + 1; z++) {
                for (int y = 0; y < Chunk.height + 2; y++) {
                    int xC = x + (chunkX * Chunk.width);
                    int zC = z + (chunkZ * Chunk.width);

                    if (noise.GetSimplex(xC, zC) * 10 + y < Chunk.height * 0.5f) {
                        blocks[x, y, z] = (int)TextureAtlas.BlockType.Grass;
                    }
                }
            }
        }

        return blocks;
    }

    void a(bool instant = false, Vector2 playerChunk = default(Vector2)) {
        //the current chunk the player is in
        int currentPlayerChunkX = (int)playerChunk.x;
        int currentPlayerChunkY = (int)playerChunk.y;

        //entered a new chunk
        if (lastPlayerChunk.x != currentPlayerChunkX || lastPlayerChunk.y != currentPlayerChunkY) {
            lastPlayerChunk.x = currentPlayerChunkX;
            lastPlayerChunk.y = currentPlayerChunkY;


            for (int i = currentPlayerChunkX - renderDistance; i <= currentPlayerChunkX + renderDistance; i ++)
                for (int j = currentPlayerChunkY - renderDistance; j <= currentPlayerChunkY +  renderDistance; j ++) {
                    Vector2 v = new Vector2(i, j);

                    if (!loadedChunks.ContainsKey(v) && !toGenerate.Contains(v)) {
                        if (instant)
                            BuildChunk(i, j);
                        else
                            toGenerate.Add(v);
                    }
                }

            //remove chunks that are too far away
            List<Vector2> toDestroy = new List<Vector2>();
            //unload chunks
            foreach (KeyValuePair<Vector2, Chunk> c in loadedChunks) {
                Vector2 v = c.Key;
                if (Mathf.Abs(currentPlayerChunkX - v.x) > (renderDistance) ||
                    Mathf.Abs(currentPlayerChunkY - v.y) > (renderDistance)) {
                    toDestroy.Add(c.Key);
                }
            }

            //remove any up for generation
            foreach (Vector2 v in toGenerate) {
                if (Mathf.Abs(currentPlayerChunkX - v.x) > (renderDistance + 2) ||
                    Mathf.Abs(currentPlayerChunkY - v.y) > (renderDistance + 2))
                    toGenerate.Remove(v);
            }

            foreach (Vector2 v in toDestroy) {
                loadedChunks[v].gameObject.SetActive(false);
                pooledChunks.Add(loadedChunks[v]);  
                loadedChunks.Remove(v);
            }
        }

        StartCoroutine(DelayBuildChunks());
    }


    IEnumerator DelayBuildChunks() {
        while (toGenerate.Count > 0) {
            BuildChunk((int)toGenerate[0].x, (int)toGenerate[0].y);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }
    }

}