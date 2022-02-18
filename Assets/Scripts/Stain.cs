using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stain : MonoBehaviour
{
    [SerializeField] GameObject stain_canvas;
    [SerializeField] Sprite[] stain_images;
    [SerializeField] GameObject stain_image;
    public int index;

    Image image_comp;
    float time_to_clean; // time to clean should be dependent on size

    // Start is called before the first frame update
    void Start()
    {
        stain_image.GetComponent<Image>().sprite = stain_images[Random.Range(0,stain_images.Length)];
        EventManager.StartListening("BrushingStain", CleanStain);

        stain_image.transform.localScale = new Vector3(0,0,0);

        float scale = Random.Range(3.78f, 8.54f);
        
        Vector3 target_scale = new Vector3(scale, scale, scale);
        StartCoroutine(LerpScale(target_scale, 3.0f));

        image_comp = stain_image.GetComponent<Image>();

        time_to_clean = Random.Range(scale - 0.5f, scale + 1.5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CleanStain(Dictionary<string, object> message) {
        
        if ((int) message["stain"] == index) {

            StartCoroutine(LerpAlpha(0f, time_to_clean));

            // todo once emit cleaned event, stop listeners, and destroy index
            // maybe have to store a reference to the index in each stain object
            // to know which stain to remove from the list in stain manager
        }

    }

    IEnumerator LerpScale (Vector3 target_scale, float duration) {
        
        float time = 0;
        Vector3 start_scale = stain_image.transform.localScale;

        while (time < duration) {
            stain_image.transform.localScale = Vector3.Lerp(start_scale, target_scale, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        stain_image.transform.localScale = target_scale;
    }

    IEnumerator LerpAlpha (float target_alpha, float duration) {
        
        float time = 0;
        Color color = stain_image.GetComponent<Image>().color;
        Color newColor = new Color(color.r, color.g, color.b, target_alpha);
        while (time < duration) {

            stain_image.GetComponent<Image>().color = Color.Lerp(color, newColor, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        stain_image.GetComponent<Image>().color = newColor;

        EventManager.TriggerEvent("StainCleaned", new Dictionary<string, object> {
                        { "stain", index },
                    });
        
        EventManager.StopListening("BrushingStain", CleanStain);

        Destroy(gameObject);

    }
}
