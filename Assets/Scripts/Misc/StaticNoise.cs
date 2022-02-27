using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticNoise : MonoBehaviour {
    public Vector2Int resolution = new Vector2Int(256, 256);
    public new Renderer renderer;
    [Space]
    public bool timedUpdate = false;
    [Min(1)]
    public int updatesPerSecond = 30;
    [Space]
    public bool grayScale = false;
    public float grayIntensity = 1.0f;
    [Space]
    public float redIntensity = 1.0f;
    public float greenIntensity = 1.0f;
    public float blueIntensity = 1.0f;

    private Texture2D noiseTexture;
    private Material material;
    private float time;

    private float UpdateThreshold => 1f / updatesPerSecond;

    private void Start() {
        noiseTexture = new Texture2D(resolution.x, resolution.y);
        noiseTexture.filterMode = FilterMode.Point;

        material = renderer.material;
    }

    private void Update() {
        if (!material) return;

        if (time >= UpdateThreshold || !timedUpdate) {
            MakeNoise();
            material.SetTexture("_MainTex", noiseTexture);
            time -= UpdateThreshold;
        }
        if (timedUpdate) time += Time.deltaTime;
        else time = 0f;
    }

    private void MakeNoise() {
        for (int x = 0; x < resolution.x; x++) {
            for (int y = 0; y < resolution.y; y++) {
                noiseTexture.SetPixel(x, y, NewColor());
            }
        }
        noiseTexture.Apply();
    }

    private Color NewColor() {
        if (!grayScale) return new Color(Random.value * redIntensity, Random.value * greenIntensity, Random.value * blueIntensity);
        else {
            var value = Random.value * grayIntensity;
            return new Color(value, value, value);
        }
    }
}
