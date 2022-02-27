using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticNoiseCompute : MonoBehaviour {
    public Vector2Int resolution = new Vector2Int(256, 256);
    public new Renderer renderer;
    public ComputeShader compute;
    [Space]
    public bool timedUpdate = false;
    [Min(1)]
    public int updatesPerSecond = 30;
    [Space]
    public bool grayScale = false;
    [Range(0, 1)]
    public float grayIntensity = 1.0f;
    [Space]
    [Range(0, 1)]
    public float redIntensity = 1.0f;
    [Range(0, 1)]
    public float greenIntensity = 1.0f;
    [Range(0, 1)]
    public float blueIntensity = 1.0f;

    private RenderTexture noiseTexture;
    private Material material;
    private float time;

    private int kernel;

    private float UpdateThreshold => 1f / updatesPerSecond;

    private void Start() {
        CreateTexture();
        InitializeMaterial();
        InitializeCompute();
    }

    private void Update() {
        if (!material) return;

        if (time >= UpdateThreshold || !timedUpdate) {
            Vector3 intensity;
            if (grayScale) intensity = new Vector3(grayIntensity, grayIntensity, grayIntensity);
            else intensity = new Vector3(redIntensity, greenIntensity, blueIntensity);

            compute.SetFloat("time", Time.time);
            compute.SetBool("gray", grayScale);
            compute.SetVector("intensity", intensity);

            compute.Dispatch(kernel, 1, 1, 1);
        }

        if (timedUpdate) time += Time.deltaTime;
        else time = 0f;
    }

    private void OnDestroy() {
        noiseTexture.Release();
    }

    private void CreateTexture() {
        noiseTexture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.Default);
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.enableRandomWrite = true;
        noiseTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        noiseTexture.Create();
    }

    private void InitializeMaterial() {
        material = renderer.material;
        material.SetTexture("_MainTex", noiseTexture);
    }

    private void InitializeCompute() {
        // buffer = new ComputeBuffer(resolution.x * resolution.y, sizeof(float) * 4);
        kernel = compute.FindKernel("DoShit");
        compute.SetTexture(kernel, "result", noiseTexture);
    }
}
