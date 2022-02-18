using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject overman_system;
    Camera mainCamera;
    [SerializeField] private GameObject underroomlight;
    [SerializeField] private GameObject entrancelight;
    [SerializeField] private GameObject storm_door;
    [SerializeField] private GameObject overman_door;
    [SerializeField] private GameObject overman_building;
    [SerializeField] private GameObject overman_isvisible;
    [SerializeField] private GameObject house;
    [SerializeField] private GameObject wind;
    [SerializeField] private AudioClip storm_start;
    [SerializeField] private AudioClip storm_loop;
    [SerializeField] private AudioClip gentle_wind;
    [SerializeField] private AudioClip overman_song;
    [SerializeField] private AudioClip overman_song_intro;
    [SerializeField] private AudioClip overman_song_loop;
    [SerializeField] private GameObject clouds;

    bool housecandespawn;
    bool overmancanspawn;
    bool housedespawned;
    bool overmanspawned;
    bool overman_spawn_nomatterwhat;
    bool house_despawn_nomatterwhat;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.backgroundColor = new Color(0.6705883f, 0.6705883f, 0.6705883f);

        terrain.terrainData.terrainLayers[0].diffuseRemapMax = new Vector4(1f, 1f, 1f, 1f);
        EventManager.StartListening("NpcAction", NpcActionListener);
        EventManager.StartListening("EnteredOvermanRoom", EnteredOvermanRoom);
        EventManager.StartListening("StartStorm", StormStarted);
        EventManager.StartListening("EndingStorm", EndStorm);
        // EventManager.TriggerEvent("StartStorm", null);

        overman_door.GetComponent<Collider>().enabled = false;
        housecandespawn = false;
        overmancanspawn = false;
        housedespawned = false;
        overmanspawned = false;
        overman_spawn_nomatterwhat = false;
        house_despawn_nomatterwhat = false;

        // Vector4 color = new Vector4(.12f, .12f, .12f, 5f);
        // StartCoroutine(LerpTerrainColor(color, 5.0f));

        // Color targetColor = new Color(0.1969117f, 0.2089766f, 0.2358491f);
        // StartCoroutine(LerpSkyColor(targetColor, 5.0f));
        
        // Debug.Log(terrain.terrainData.terrainLayers[0].diffuseRemapMin);
    }

    // Update is called once per frame
    void Update()
    {
        // have listener for end of last conversation and last npc inside house
        // start storm
        // spawn house after time period
        // have listener for when getting closer to house so that the storm dies down
        if (housecandespawn && !housedespawned) {
            if (!house.GetComponent<Renderer>().isVisible || house_despawn_nomatterwhat) {
                house.SetActive(false);
                StartCoroutine(OvermanSpawnTimer());
                housedespawned = true;
            }
        }

        if(overmancanspawn && !overmanspawned) {
            if (!overman_isvisible.GetComponent<Renderer>().isVisible || overman_spawn_nomatterwhat) {
                overman_building.SetActive(true);
                EventManager.TriggerEvent("OvermanBuildingSpawned", null);
                overmanspawned = true;
            }
        }
    }

    void NpcActionListener(Dictionary<string, object> message) {
        Debug.Log("NpcActionListener");
        // last_npc_actions[(string) message["function"]]();
        if ((string) message["function"] == "despawnhouse") {
            StartCoroutine(EndGame());
        }
    }

    void StormStarted(Dictionary<string, object> message) {
        StartCoroutine(StartStorm());
    }

    void EndStorm(Dictionary<string, object> message) {
        storm_door.SetActive(true);
        StartCoroutine(LerpVolume(0, 3.0f));
        StartCoroutine(LerpFog(new Color(0.5849056f, 0.5849056f, 0.5849056f), 0.001f, 1.0f));
    }

    IEnumerator StartStorm() {
        yield return new WaitForSeconds(15.0f);
        StartCoroutine(StormNoise());
        yield return new WaitForSeconds(1.0f);
        Color targetColor = new Color(0.1969117f, 0.2089766f, 0.2358491f);
        StartCoroutine(LerpSkyColor(targetColor, 30.0f));
        StartCoroutine(LerpAmbientColor(new Color(0.1603774f,0.1603774f,0.1603774f), new Color(0.1886792f,0.1886792f,0.1886792f), new Color(0.3207547f,0.3207547f,0.3207547f), 20f));
        StartCoroutine(LerpFog(new Color(0,0,0), 0.05f, 30f));
        StartCoroutine(LerpClouds(new Color (0.2169811f, 0.2169811f, 0.2169811f), 30f));
        StartCoroutine(BuildingsHandler());
        // yield return new WaitForSeconds(10.0f);
        EventManager.TriggerEvent("StartingStorm", null);
    }

    IEnumerator StormNoise() {
        wind.GetComponent<AudioSource>().loop = false;
        StartCoroutine(LerpVolume(.65f, 4.0f));
        wind.GetComponent<AudioSource>().PlayOneShot(storm_start);
        yield return new WaitForSeconds(80f);
        wind.GetComponent<AudioSource>().clip = storm_loop;
        wind.GetComponent<AudioSource>().loop = true;
        wind.GetComponent<AudioSource>().Play();
    }

    IEnumerator EndGame() {
        Debug.Log("ENDING GAME");
        yield return new WaitForSeconds(5.0f);
        overman_system.SetActive(false);
        EventManager.TriggerEvent("ConversationEnded", null);
        EventManager.TriggerEvent("DespawnHouse", null);
        EventManager.TriggerEvent("EndGameWind", null);
        wind.GetComponent<AudioSource>().volume = 0;
        wind.GetComponent<AudioSource>().clip = gentle_wind;
        wind.GetComponent<AudioSource>().loop = true;
        wind.GetComponent<AudioSource>().Play();
        StartCoroutine(LerpVolume(.4f, 10.0f));
        StartCoroutine(LerpFog(new Color(0.8509804f,0.8039216f,0.7019608f), 0.01f, 1f));
        StartCoroutine(LerpClouds(new Color (0.8584906f, 0.8129339f, 0.7491545f), 1f));
        Color targetColor = new Color(0.8784314f, 0.8666667f, 0.7647059f);
        StartCoroutine(LerpSkyColor(targetColor, 1f));
    }

    void EnteredOvermanRoom(Dictionary<string, object> message) {
        overman_door.GetComponent<Collider>().enabled = true;
        StartCoroutine(LerpAmbientColor(new Color(0.9137f,0.9137f,0.9137f), new Color(0.8117647f,0.869943f,0.6127626f), new Color(0.9622642f,0.8901961f,0.7372549f), 2.5f));
        StartCoroutine(LerpClouds(new Color (0.3301887f, 0.3301887f, 0.3301887f), 2.5f));
        Destroy(underroomlight);
        Destroy(entrancelight);
        StartCoroutine(OvermanSong());
        
    }

    IEnumerator OvermanSong() {
        wind.GetComponent<AudioSource>().loop = false;
        wind.GetComponent<AudioSource>().clip = overman_song_intro;
        wind.GetComponent<AudioSource>().volume = .141f;
        wind.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(12.4f);
        wind.GetComponent<AudioSource>().clip = overman_song_loop;
        wind.GetComponent<AudioSource>().loop = true;
        wind.GetComponent<AudioSource>().volume = .141f;
        wind.GetComponent<AudioSource>().Play();
    }

    IEnumerator BuildingsHandler() {
        yield return new WaitForSeconds(10.0f);
        Destroy(GameObject.FindGameObjectWithTag("last_npc"));
        housecandespawn = true;
        yield return new WaitForSeconds(30.0f);
        house_despawn_nomatterwhat = true;
    }

    IEnumerator OvermanSpawnTimer() {
        yield return new WaitForSeconds(90.0f);
        overmancanspawn = true;
        yield return new WaitForSeconds(60.0f);
        overman_spawn_nomatterwhat = true;
    }


    IEnumerator LerpTerrainColor (Vector4 target_color, float duration) {
        
        float time = 0;
        Vector4 start_color = terrain.terrainData.terrainLayers[0].diffuseRemapMax;

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            terrain.terrainData.terrainLayers[0].diffuseRemapMax = Vector4.Lerp(start_color, target_color, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        terrain.terrainData.terrainLayers[0].diffuseRemapMax = target_color;
    }

    IEnumerator LerpSkyColor (Color target_color, float duration) {
        
        float time = 0;
        Color start_color = mainCamera.backgroundColor;

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            mainCamera.backgroundColor = Color.Lerp(start_color, target_color, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        mainCamera.backgroundColor = target_color;
    }

    IEnumerator LerpClouds (Color target_color, float duration) {
        
        float time = 0;
        Color start_color = clouds.GetComponent<Image>().color;

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            clouds.GetComponent<Image>().color = Color.Lerp(start_color, target_color, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        clouds.GetComponent<Image>().color = target_color;
    }

    IEnumerator LerpVolume (float target_volume, float duration) {
        float time = 0;
        float start_volume = wind.GetComponent<AudioSource>().volume;

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            wind.GetComponent<AudioSource>().volume = Mathf.Lerp(start_volume, target_volume, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        wind.GetComponent<AudioSource>().volume = target_volume;
        if (target_volume == 0) {
            wind.GetComponent<AudioSource>().Stop();
        }
    }

    IEnumerator LerpAmbientColor (Color target_skycolor, Color target_groundcolor, Color target_equatorcolor, float duration) {
        float time = 0;
        Color start_skycolor = RenderSettings.ambientSkyColor;
        Color start_groundcolor = RenderSettings.ambientGroundColor;
        Color start_equatorcolor = RenderSettings.ambientEquatorColor;

        while (time < duration) {

            RenderSettings.ambientSkyColor = Color.Lerp(start_skycolor, target_skycolor, time/duration);
            RenderSettings.ambientGroundColor = Color.Lerp(start_groundcolor, target_groundcolor, time/duration);
            RenderSettings.ambientEquatorColor = Color.Lerp(start_equatorcolor, target_equatorcolor, time/duration);
            
            time += Time.deltaTime;
            yield return null;
        }

        RenderSettings.ambientSkyColor = target_skycolor;
        RenderSettings.ambientGroundColor = target_groundcolor;
        RenderSettings.ambientEquatorColor = target_equatorcolor;
    }

    IEnumerator LerpFog (Color target_color, float target_density, float duration) {
        
        float time = 0;
        Color start_color = RenderSettings.fogColor;
        float start_density = RenderSettings.fogDensity;

        while (time < duration) {
            RenderSettings.fogColor = Color.Lerp(start_color, target_color, time/duration);
            RenderSettings.fogDensity = Mathf.Lerp(start_density, target_density, time/duration);
            
            time += Time.deltaTime;
            yield return null;
        }

        RenderSettings.fogColor = target_color;
        RenderSettings.fogDensity = target_density;
    }
}
