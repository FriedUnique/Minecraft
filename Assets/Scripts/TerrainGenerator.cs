using System;
using System.Collections;
using System.Collections.Generic;
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

    public string textureName; // texture Atlas Nmae
    public int renderDistance; // the renderdistance is actually 6 when including the chunk in which the player is atm
    public GameObject chunkPrefab;

    [HideInInspector] public TextureAtlas atlas;
    [HideInInspector] public Dictionary<Vector2, Chunk> loadedChunks; // x,z cCoords
    [HideInInspector] public Player player;
    [HideInInspector] public SaveData saver;

    private FastNoise noise;

    private Vector2 lastPlayerChunk;
    List<Vector2> chunksToLoad = new List<Vector2>();
    List<Chunk> chunkPool = new List<Chunk>();

    private void Start() {
        atlas = new TextureAtlas(textureName, 16f);
        loadedChunks = new Dictionary<Vector2, Chunk>();
        noise = new FastNoise(1337);
        player = FindObjectOfType<Player>(); // make the terrain generator spawn player

        // saver setup
        saver = GetComponent<SaveData>();
        saver.SetRegion(0, 0);
        saver.LoadChunkDataIntoMemory();

        LoadChunks(instant: true, playerChunk: new Vector2(1, 0));
    }

    private void Update() {
        LoadChunks(playerChunk: player.GetCurrentChunk());
    }

    private void OnDisable() {
        foreach(Chunk chunk in loadedChunks.Values) {
            saver.SaveChunkToMemory(chunk);
        }

        saver.SaveRegionToDisk();
    }

    private void BuildChunk(int x, int z) {
        Chunk chunk;
        if (chunkPool.Count > 0){
            chunk = chunkPool[0];
            chunk.gameObject.SetActive(true);
            chunkPool.RemoveAt(0);
            chunk.transform.position = new Vector3(x * 16, 0, z * 16);
        } else {
            chunk = Instantiate(chunkPrefab, new Vector3(x * 16, 0, z * 16), Quaternion.identity).GetComponent<Chunk>();
            chunk.initChunk(atlas);
        }

        chunk.blocksInChunk = ChunkData(x, z);
        chunk.updateChunk(new Vector2(x, z));

        //saver.SaveChunkToMemory(chunk);

        loadedChunks.Add(new Vector2(x, z), chunk);
    }

    public byte[,,] ChunkData(int chunkX, int chunkZ) {
        byte[,,] blocks = saver.GetSavedChunkData(new Vector2(chunkX, chunkZ));
        if(blocks!=null) { return blocks; }

        blocks = TextureAtlas.GetEmptyChunkList();

        for (int x = 0; x < Chunk.width + 1; x++) {
            for (int z = 0; z < Chunk.width + 1; z++) {
                for (int y = 0; y < Chunk.height + 2; y++) {
                    int xC = x + (chunkX * Chunk.width);
                    int zC = z + (chunkZ * Chunk.width);

                    if (noise.GetSimplex(xC, zC) * 10 + y < Chunk.height * 0.5f) {
                        blocks[x, y, z] = (byte)TextureAtlas.BlockType.Grass;
                    }
                }
            }
        }

        return blocks;
    }

    void LoadChunks(bool instant = false, Vector2 playerChunk = default(Vector2)) {
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

                    if (!loadedChunks.ContainsKey(v) && !chunksToLoad.Contains(v)) {
                        if (instant)
                            BuildChunk(i, j);
                        else
                            chunksToLoad.Add(v);
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

            foreach (Vector2 v in chunksToLoad) {
                if (Mathf.Abs(currentPlayerChunkX - v.x) > (renderDistance + 2) ||
                    Mathf.Abs(currentPlayerChunkY - v.y) > (renderDistance + 2))
                    chunksToLoad.Remove(v);
            }

            foreach (Vector2 v in toDestroy) {
                loadedChunks[v].gameObject.SetActive(false);
                chunkPool.Add(loadedChunks[v]);
                saver.SaveChunkToMemory(loadedChunks[v]);
                loadedChunks.Remove(v);
            }
        }

        StartCoroutine(DelayBuildChunks());
    }

    IEnumerator DelayBuildChunks() {
        while (chunksToLoad.Count > 0) {
            BuildChunk((int)chunksToLoad[0].x, (int)chunksToLoad[0].y);
            chunksToLoad.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }
    }
}