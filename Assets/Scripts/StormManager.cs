using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StormManager : MonoBehaviour
{
    [SerializeField] private GameObject Debris;
    [SerializeField] private GameObject Tumbleweed;
    [SerializeField] private GameObject Wind_Particles;
    [SerializeField] private GameObject Leaf;
    [SerializeField] private GameObject Wind_Audio;
    ParticleSystem debris_ps;
    ParticleSystem tumbleweed_ps;
    ParticleSystem wind_ps;
    ParticleSystem leaf_ps;
    int debris_max_particles;
    int tumbleweed_max_particles;
    int wind_max_particles;
    int leaf_max_particles;
    ParticleSystem.MainModule debris_main;
    ParticleSystem.MainModule tumbleweed_main;
    ParticleSystem.MainModule wind_main;
    ParticleSystem.MainModule leaf_main;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("EndingStorm", EndStorm);
        EventManager.StartListening("StartingStorm", StartStorm);
        EventManager.StartListening("EndGameWind", EndGameWind);

        debris_ps = Debris.GetComponent<ParticleSystem>();
        tumbleweed_ps = Tumbleweed.GetComponent<ParticleSystem>();
        wind_ps = Wind_Particles.GetComponent<ParticleSystem>();
        leaf_ps = Leaf.GetComponent<ParticleSystem>();

        debris_ps.Stop();
        tumbleweed_ps.Stop();
        wind_ps.Stop();
        leaf_ps.Stop();

        debris_max_particles = debris_ps.main.maxParticles;
        tumbleweed_max_particles = tumbleweed_ps.main.maxParticles;
        wind_max_particles = wind_ps.main.maxParticles;
        leaf_max_particles = leaf_ps.main.maxParticles;

        debris_main = debris_ps.main;
        tumbleweed_main = tumbleweed_ps.main;
        wind_main = wind_ps.main;
        leaf_main = leaf_ps.main;

        debris_main.maxParticles = 0;
        tumbleweed_main.maxParticles = 0;
        wind_main.maxParticles = 0;
        leaf_main.maxParticles = 0;

        wind_main.maxParticles = 500;
        wind_ps.Play();



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartStorm(Dictionary<string, object> message) {
        debris_ps.Play();
        debris_main.simulationSpeed = 3;
        StartCoroutine(LerpMaxParticles(3000, 60.0f, debris_main));//3000
        StartCoroutine(LerpSpeed(7, 60.0f, debris_main));
        tumbleweed_ps.Play();
        tumbleweed_main.simulationSpeed = 2;
        StartCoroutine(LerpMaxParticles(10, 60.0f, tumbleweed_main));
        StartCoroutine(LerpSpeed(4, 60.0f, tumbleweed_main));
        wind_main.simulationSpeed = 2;
        var emission = wind_ps.emission;
        emission.rateOverTime = 1000; //1000
        wind_ps.Play();
        StartCoroutine(LerpMaxParticles(7000, 60.0f, wind_main)); //7000
        StartCoroutine(LerpSpeed(10, 60.0f, wind_main));
        leaf_ps.Play();
        leaf_main.simulationSpeed = 1;
        StartCoroutine(LerpMaxParticles(2000, 60.0f, leaf_main)); //2000
        StartCoroutine(LerpSpeed(6, 60.0f, leaf_main));
    }

    void EndGameWind(Dictionary<string, object> message) {
        wind_main.simulationSpeed = 5;
        var emission = wind_ps.emission;
        emission.rateOverTime = 10;
        wind_main.startColor = new Color(0.8113208f, 0.7516762f, 0.5472589f);
        wind_ps.Play();
    }

    void EndStorm(Dictionary<string, object> message) {
                    debris_ps.Stop();
            tumbleweed_ps.Stop();
            wind_ps.Stop();
            leaf_ps.Stop();

    }

    IEnumerator LerpMaxParticles (int target_max, float duration, ParticleSystem.MainModule main) {
        
        float time = 0;
        // float start_alpha = text.GetComponent<Renderer>().material.color.a;
        int current_max = main.maxParticles;

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            main.maxParticles = (int) Mathf.Lerp(current_max, target_max, time/duration);

            
            time += Time.deltaTime;
            yield return null;
        }

        main.maxParticles = target_max;


    }

    IEnumerator LerpSpeed (float target_speed, float duration, ParticleSystem.MainModule main) {
        float time = 0;

        float speed = main.simulationSpeed;

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            main.simulationSpeed = Mathf.Lerp(speed, target_speed, time/duration);
            
            time += Time.deltaTime;
            yield return null;
        }

        main.simulationSpeed = target_speed;
    }
}
