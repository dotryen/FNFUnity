using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.PostProcessing;

[System.Serializable]
[VolumeComponentMenu("Glitch/Digital")]
public sealed class DigitalGlitch : VolumeComponent {
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
}

[CustomPostProcess("Digital Glitch", CustomPostProcessInjectionPoint.AfterPostProcess)]
public sealed class DigitalGlitchRenderer : CustomPostProcessRenderer {
    private Texture2D noiseTex;
    private RenderTexture trashTex1;
    private RenderTexture trashTex2;

    private DigitalGlitch component;
    private Material material;

    static class ShaderIDs {
        internal readonly static int Input = Shader.PropertyToID("_MainTex");
        internal readonly static int Noise = Shader.PropertyToID("_NoiseTex");
        internal readonly static int Trash = Shader.PropertyToID("_TrashTex");
        internal readonly static int Intensity = Shader.PropertyToID("_Intensity");
    }

    public override bool visibleInSceneView => false;

    public override void Initialize() {
        noiseTex = new Texture2D(64, 32, TextureFormat.ARGB32, false) {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };
        material = CoreUtils.CreateEngineMaterial("Hidden/Glitch/Digital");
    }

    public override bool Setup(ref RenderingData renderingData, CustomPostProcessInjectionPoint injectionPoint) {
        var stack = VolumeManager.instance.stack;
        component = stack.GetComponent<DigitalGlitch>();
        return component.active && component.intensity.value > 0f;
    }

    public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, ref RenderingData renderingData, CustomPostProcessInjectionPoint injectionPoint) {
        UpdateNoise();

        trashTex1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        trashTex2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        var frameCount = Time.frameCount;
        if (frameCount % 13 == 0) cmd.Blit(source, trashTex1);
        if (frameCount % 73 == 0) cmd.Blit(source, trashTex2);

        material.SetFloat(ShaderIDs.Intensity, component.intensity.value);

        cmd.SetGlobalTexture(ShaderIDs.Noise, noiseTex);
        cmd.SetGlobalTexture(ShaderIDs.Trash, Random.value > 0.5f ? trashTex1 : trashTex2);
        cmd.SetGlobalTexture(ShaderIDs.Input, source);

        CoreUtils.DrawFullScreen(cmd, material, destination);

        // i thought this could cause issues when issuing the command lol
        RenderTexture.ReleaseTemporary(trashTex1);
        RenderTexture.ReleaseTemporary(trashTex2);
    }

    private void UpdateNoise() {
        var color = RandomColor();
        for (int y = 0; y < noiseTex.height; y++) {
            for (int x = 0; x < noiseTex.width; x++) {
                if (Random.value > 0.89f) color = RandomColor();
                noiseTex.SetPixel(x, y, color);
            }
        }
        noiseTex.Apply();
    }

    private Color RandomColor() {
        return new Color(Random.value, Random.value, Random.value, Random.value);
    }
}
