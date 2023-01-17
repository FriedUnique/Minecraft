using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> pools;

    [System.Serializable]
    public class Pool {
        public string poolName;
        public GameObject poolObject;
        public int size;
    }

    public List<Pool> poolList; // made in inspector

    private void Awake() {
        pools = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in poolList) {
            Queue<GameObject> queue = new Queue<GameObject>();

            //if(pool.poolName == "chunks") { pool.size = gameObject.GetComponent<TerrainGenerator>().renderCount; }

            // fill pool
            for (int i = 0; i < pool.size ; i++) {
                GameObject go = Instantiate(pool.poolObject);
                go.SetActive(false);
                queue.Enqueue(go);
            }

            pools.Add(pool.poolName, queue); // add to dict
        }
    }

    public GameObject spawnFromPool(string name, Vector3 pos, Quaternion rot, object[] args) {
        GameObject go = pools[name].Dequeue();
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.SetActive(true);

        pools[name].Enqueue(go); // requeue the object for later use as well;

        go.GetComponent<IPooledObject>()?.OnPooled(args);

        return go;
    }

}

