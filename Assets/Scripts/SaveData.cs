using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

// C:\Users\stefa\AppData\LocalLow\DefaultCompany\Minecraft

public class SaveData : MonoBehaviour {
    private static Dictionary<Vector2, ChunkData> cData = new Dictionary<Vector2, ChunkData>();
    // always load and unload the current region into memory and save the old region to disk

    string path;

    public void SetRegion(int x, int y) {
        path = Application.persistentDataPath + "/Region" + x.ToString() + y.ToString() + ".neger";
    }

    // make a save region method
    public void SaveRegionToDisk() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, cData.Values.ToList());
        stream.Close();
    }


    public byte[,,] GetSavedChunkData(Vector2 chunkPos) {
        if (!cData.ContainsKey(chunkPos)) {
            return null;
        }

        ChunkData chunk = cData[chunkPos];
        byte[] a = chunk.chunkInfo;

        if(a.Length < TextureAtlas.blockDataLenght) {
            Debug.LogWarning($"No data! (Chunk Info Length: {a.Length})");
            return null; 
        }

        byte[,,] chunkDataArray = TextureAtlas.GetEmptyChunkList();

        for (int x = 0; x < Chunk.width + 1; x++) {
            for (int y = 0; y < Chunk.height + 1; y++) { 
                for (int z = 0; z < Chunk.width + 1; z++) {
                    byte b = a[(Chunk.height+1) * (Chunk.width+ 1) * x + (Chunk.width+ 1) * y + z];
                    chunkDataArray[x, y, z] = b;
                }
            }
        }

        return chunkDataArray;
    }

    public void LoadChunkDataIntoMemory() {
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            if (stream.Length == 0) {
                Debug.LogWarning("No data found!");
                return;
            }

            List<ChunkData> cDataList;
            try {
                cDataList = (List<ChunkData>)formatter.Deserialize(stream);
            }
            catch (System.InvalidCastException) {
                ChunkData c = (ChunkData)formatter.Deserialize(stream);
                cDataList = new List<ChunkData>();
                cDataList.Add(c);
            }

            cData = new Dictionary<Vector2, ChunkData>();

            // 16*16
            foreach (ChunkData data in cDataList) {
                Vector2 v = new Vector2(data.chunkX, data.chunkZ);
                if (cData.ContainsKey(v)) { continue; }
                cData.Add(v, data);
            }
                
            stream.Close();
        } else {
            Debug.LogWarning("Save file not found!");
            return;
        }
    }

    public void SaveChunkToMemory(Chunk chunk) {
        ChunkData c = new ChunkData(chunk.chunkPos, chunk.GetSaveData());

        AddToMemory(c);
    }
    
    private void AddToMemory(ChunkData c) {
        // chunk not yet in memory
        if (!cData.ContainsKey(new Vector2(c.chunkX, c.chunkZ))) {
            cData.Add(new Vector2(c.chunkX, c.chunkZ), c);
            return;
        }

        //chunk is in memory -> update it
        ChunkData data = cData[new Vector2(c.chunkX, c.chunkZ)];
        data.chunkInfo = c.chunkInfo;
        cData[new Vector2(c.chunkX, c.chunkZ)] = data;

    }
}





[System.Serializable]
public struct ChunkData {
    public int chunkX;
    public int chunkZ;
    public byte[] chunkInfo;

    public ChunkData(Vector2 v, byte[] chunkData) {
        this.chunkX = (int)v.x;
        this.chunkZ = (int)v.y;
        this.chunkInfo = chunkData;
    }
}