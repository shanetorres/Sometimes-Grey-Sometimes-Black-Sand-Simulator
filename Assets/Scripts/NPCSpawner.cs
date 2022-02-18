using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] npcs;
    [SerializeField] private GameObject[] spawn_areas;
    [SerializeField] private int max_npcs;
    [SerializeField] private GameObject player;

    Vector3 npc_location;
    bool npcs_can_spawn;
    bool npc_spawning;
    
    int live_npcs = 0;
    Quaternion rot = Quaternion.Euler(0,0,0);


    // Start is called before the first frame update
    void Start()
    {
        npcs_can_spawn = true;

        EventManager.StartListening("npcDestroyed", OnNPCDestroyed);
        EventManager.StartListening("AllConversationsHad", AllConversationsHadListener);
    }

    // Update is called once per frame
    void Update()
    {
        if (npcs_can_spawn & !npc_spawning) {
            if (live_npcs < max_npcs) {
                StartCoroutine(SpawnNPC());
            }
            
        }
    }

    IEnumerator SpawnNPC() {
        npc_spawning = true;

        int npc_type = Random.Range(0, 6);
        GameObject npc = npcs[npc_type];

        int spawn_area = Random.Range(0, 4);

        Vector3 pos = GetRandomPointInsideCollider(spawn_areas[spawn_area].GetComponent<BoxCollider>());
        Instantiate(npc, pos, rot);

        yield return new WaitForSeconds(2.0f);

        npc_spawning = false;
        live_npcs++;

    }

    void SpawnLastNPC() {
        GameObject npc = npcs[6];
        // Vector3 distanceFromPlayer = new Vector3(0,5,-50);
        int spawn_index = GetClosestSpawnArea(player.transform);
        Debug.Log("CLOSEST SPAWN AREA: " + spawn_index);
        Vector3 lastNPCSpawnPoint = GetRandomPointInsideCollider(spawn_areas[spawn_index].GetComponent<BoxCollider>());
        Debug.Log("SPAWNING LAST NPC");
        Debug.Log(lastNPCSpawnPoint);

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
        SpawnLastNPC();
        EventManager.StopListening("AllConversationsHad", AllConversationsHadListener);
    }
}
