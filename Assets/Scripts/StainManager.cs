using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StainManager : MonoBehaviour
{
    private static StainManager stainManager;

    public static StainManager Instance { get { return stainManager; } }


    private void Awake()
    {
        if (stainManager != null && stainManager != this)
        {
            Destroy(this.gameObject);
        } else {
            stainManager = this;
        }
    }

    [SerializeField] GameObject StainSpawn;
    [SerializeField] GameObject Stain;
    bool stain_spawning;
    Quaternion rot = Quaternion.Euler(0,0,0);
    int live_stains;
    [SerializeField] bool stains_can_spawn = false;
    [SerializeField] int max_stains;
    [SerializeField] float shortest_spawn_time;
    [SerializeField] float longest_spawn_time;
    [SerializeField] GameObject overman;
    public static List<GameObject> stains = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        stain_spawning = false;
        live_stains = 0;
        EventManager.StartListening("StainCleaned", StainCleaned);
        EventManager.StartListening("DespawnHouse", StopStains);
        EventManager.StartListening("OvermanBuildingSpawned", StartStains);
        EventManager.StartListening("NpcAction", NpcActionListener);
    }

    // Update is called once per frame
    void Update()
    {
        if (stains_can_spawn & !stain_spawning) {
            if (live_stains < max_stains) {
                StartCoroutine(SpawnStain()); 
            }   
        }
    }

    IEnumerator SpawnStain() {
        Debug.Log("Spawning stain");
        stain_spawning = true;

        GameObject new_stain = Stain;

        int spawn_area = Random.Range(0, 4);

        Vector3 pos = GetRandomPointInsideCollider(StainSpawn.GetComponent<BoxCollider>());

        while (Vector3.Distance(pos, overman.transform.position) < 2.0f) {
            pos = GetRandomPointInsideCollider(StainSpawn.GetComponent<BoxCollider>());
        }
        new_stain = Instantiate(new_stain, pos, rot);

        new_stain.GetComponent<Stain>().index = stains.Count;
        stains.Add(new_stain);

        float spawn_time = Random.Range(shortest_spawn_time, longest_spawn_time);
        yield return new WaitForSeconds(spawn_time);

        stain_spawning = false;
        live_stains++;

    }

    Vector3 GetRandomPointInsideCollider( BoxCollider boxCollider ) {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(
            Random.Range( -extents.x, extents.x ),
            Random.Range( -extents.y, extents.y ),
            Random.Range( -extents.z, extents.z )
        );

        Vector3 pos = boxCollider.transform.TransformPoint( point );
    
        return new Vector3(pos.x, boxCollider.transform.position.y, pos.z);
    }

    void StartStains(Dictionary<string, object> message) {
        stains_can_spawn = true;
        Debug.Log("START STAINS CALLED, staind_can_spawn: " + stains_can_spawn);
    }

    void StopStains(Dictionary<string, object> message) {
        stains_can_spawn = false;
        GameObject[] stains = GameObject.FindGameObjectsWithTag("stain");
        for (int i = 0; i < stains.Length; i++) {
            Destroy(stains[i]);
        }
        // for (int i = 0; i < stains.Count; i++) {
        //     Stain stain = stains[i].GetComponent<Stain>();
        //     stains.RemoveAt(stain.index);
        //     Destroy(stain);
        // }
    }

    void NpcActionListener(Dictionary<string, object> message) {
        if ((string) message["function"] == "stopstains") {
            stains_can_spawn = false;
            EventManager.TriggerEvent("ActionCompleted", null);
        } else if ((string) message["function"] == "startstains") {
            stains_can_spawn = true;
            EventManager.TriggerEvent("ActionCompleted", null);
            StartCoroutine(SpawnStain()); 
        }
    }

    void StainCleaned(Dictionary<string, object> message) {
        stains.RemoveAt((int) message["stain"]);
        for (int i = 0; i < stains.Count; i++) {
            stains[i].GetComponent<Stain>().index = i;
        }

        live_stains--;
    }
}
