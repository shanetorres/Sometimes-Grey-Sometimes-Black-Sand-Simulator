using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    Renderer brush_renderer;
    Color mat0_color;
    Color mat1_color;
    Color mat2_color;

    // Start is called before the first frame update
    void Start()
    {
        brush_renderer = gameObject.GetComponent<Renderer>(); 
        
        mat0_color = brush_renderer.materials[0].color;
        mat1_color = brush_renderer.materials[1].color;
        mat2_color = brush_renderer.materials[2].color;

        brush_renderer.materials[0].color = new Color(mat0_color.r, mat0_color.g, mat0_color.b, 0);
        brush_renderer.materials[1].color = new Color(mat1_color.r, mat1_color.g, mat1_color.b, 0);
        brush_renderer.materials[2].color = new Color(mat2_color.r, mat2_color.g, mat2_color.b, 0);

        EventManager.StartListening("BrushingStain", TurnOnBrushLOL);
        EventManager.StartListening("StainCleaned", TurnOffBrushLOL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TurnOnBrushLOL(Dictionary<string, object> message) {
        StartCoroutine(LerpAlpha(1, 0.5f));
    }

    void TurnOffBrushLOL(Dictionary<string, object> message) {
        StartCoroutine(LerpAlpha(0, 0.5f));
    }

    IEnumerator LerpAlpha (float target_alpha, float duration) {
        
        float time = 0;

        Color color0 = brush_renderer.materials[0].color;
        Color color1 = brush_renderer.materials[1].color;
        Color color2 = brush_renderer.materials[2].color;

        Color newColor0 = new Color(mat0_color.r, mat0_color.g, mat0_color.b, target_alpha);
        Color newColor1 = new Color(mat1_color.r, mat1_color.g, mat1_color.b, target_alpha);
        Color newColor2 = new Color(mat2_color.r, mat2_color.g, mat2_color.b, target_alpha);

        while (time < duration) {
            brush_renderer.materials[0].color = Color.Lerp(color0, newColor0, time/duration);
            brush_renderer.materials[1].color = Color.Lerp(color1, newColor1, time/duration);
            brush_renderer.materials[2].color = Color.Lerp(color2, newColor2, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        brush_renderer.materials[0].color = newColor0;
        brush_renderer.materials[1].color = newColor1;
        brush_renderer.materials[2].color = newColor2;

    }
}
