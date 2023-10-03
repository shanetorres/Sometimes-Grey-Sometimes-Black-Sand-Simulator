using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] npcs;
    [SerializeField] private GameObject[] spawn_areas;
    [SerializeField] private int max_npcs;
    [SerializeField] private GameObject player;

    Vector3 npc_location;
    bool npcs_can_spawn;
    bool npc_spawning;
    
    public int live_npcs = 0;
    Quaternion rot = Quaternion.Euler(0,0,0);
    bool last_npc_can_spawn;


    // Start is called before the first frame update
    void Start()
    {
        npcs_can_spawn = false;
        last_npc_can_spawn = false;
        npc_spawning = false;

        EventManager.StartListening("npcDestroyed", OnNPCDestroyed);
        EventManager.StartListening("AllConversationsHad", AllConversationsHadListener);
        EventManager.StartListening("StopSpawningNPCs", StopSpawning);
        StartCoroutine(SpawnWait());
    }

    // Update is called once per frame
    void Update()
    {
        if (npcs_can_spawn & !npc_spawning) {
            if (live_npcs < max_npcs) {
                StartCoroutine(SpawnNPC());
            }
        }

        if (last_npc_can_spawn && live_npcs == 0) {
            SpawnLastNPC();
            last_npc_can_spawn = false;
        }
    }

    IEnumerator SpawnWait() {
        Debug.Log("SPAWN WAIT");
        yield return new WaitForSeconds(1.0f);
        npcs_can_spawn = true;
    }

    IEnumerator SpawnNPC() {
        npc_spawning = true;

        int npc_type = Random.Range(0, 6);
        GameObject npc = npcs[npc_type];

        int spawn_area = Random.Range(0, 4);

        Vector3 pos = GetRandomPointInsideCollider(spawn_areas[spawn_area].GetComponent<BoxCollider>());
        Instantiate(npc, pos, rot);
        live_npcs++;

        float spawn_time = Random.Range(2.0f, 15.0f);
        yield return new WaitForSeconds(spawn_time);

        npc_spawning = false;


    }

    void SpawnLastNPC() {
        GameObject npc = npcs[6];
        // Vector3 distanceFromPlayer = new Vector3(0,5,-50);
        int spawn_index = GetClosestSpawnArea(player.transform);
        Vector3 lastNPCSpawnPoint = GetRandomPointInsideCollider(spawn_areas[spawn_index].GetComponent<BoxCollider>());
        GameObject.FindGameObjectWithTag("house").GetComponent<NavMeshObstacle>().enabled = true;

        Instantiate(npc, lastNPCSpawnPoint, rot);
    }

    Vector3 GetRandomPointInsideCollider( BoxCollider boxCollider ) {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(
            Random.Range( -extents.x, extents.x ),
            Random.Range( -extents.y, extents.y ),
            Random.Range( -extents.z, extents.z )
        );

        Vector3 pos = boxCollider.transform.TransformPoint( point );
    
        return new Vector3(pos.x, 1.912f, pos.z);
    }

    void OnNPCDestroyed(Dictionary<string, object> message) {
        Debug.Log("DESTROYED:" + live_npcs);
        if (live_npcs > 0) {
            live_npcs--;
        }
    }

    void StopSpawning(Dictionary<string, object> message) {
        Debug.Log("CALLED STOP SPAWNING!");
        npcs_can_spawn = false;
    }

    int GetClosestSpawnArea(Transform p_transform)
    {
        int tMin = 0;;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = p_transform.position;
        for (int i = 0; i < spawn_areas.Length; i++)
        {
            float dist = Vector3.Distance(currentPos, spawn_areas[i].transform.position);
            if (dist < minDist)
            {
                tMin = i;
                minDist = dist;
            }
        }
        return tMin;
    }

    void AllConversationsHadListener(Dictionary<string, object> message) {
        npcs_can_spawn = false;
        last_npc_can_spawn = true;
        EventManager.StopListening("AllConversationsHad", AllConversationsHadListener);
    }
}
