using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.PostProcessing;

[System.Serializable]
[VolumeComponentMenu("Glitch/Analog")]
public sealed class AnalogGlitch : VolumeComponent {
    public ClampedFloatParameter scanLineJitter = new ClampedFloatParameter(0f, 0f, 1f);
    public ClampedFloatParameter verticalJump = new ClampedFloatParameter(0f, 0f, 1f);
    public ClampedFloatParameter horizontalShake = new ClampedFloatParameter(0f, 0f, 1f);
    public ClampedFloatParameter colorDrift = new ClampedFloatParameter(0f, 0f, 1f);
}

[CustomPostProcess("Analog Glitch", CustomPostProcessInjectionPoint.AfterPostProcess)]
public sealed class AnalogGlitchRenderer : CustomPostProcessRenderer {
    private float verticalJumpTime;

    private AnalogGlitch component;

    private Material renderMat;

    static class ShaderIDs {
        internal readonly static int Input = Shader.PropertyToID("_MainTex");
        internal readonly static int Scan = Shader.PropertyToID("_ScanLineJitter");
        internal readonly static int Vertical = Shader.PropertyToID("_VerticalJump");
        internal readonly static int Hoirzontal = Shader.PropertyToID("_HorizontalDrift");
        internal readonly static int Color = Shader.PropertyToID("_ColorDrift");
    }

    public override bool visibleInSceneView => false;

    public override void Initialize() {
        renderMat = CoreUtils.CreateEngineMaterial("Hidden/Glitch/Analog");
    }

    public override bool Setup(ref RenderingData renderingData, CustomPostProcessInjectionPoint injectionPoint) {
        var stack = VolumeManager.instance.stack;
        component = stack.GetComponent<AnalogGlitch>();
        return (component.scanLineJitter.value + component.verticalJump.value + component.horizontalShake.value + component.colorDrift.value) > 0;
    }

    public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, ref RenderingData renderingData, CustomPostProcessInjectionPoint injectionPoint) {
        verticalJumpTime += Time.deltaTime * component.verticalJump.value * 11.3f;

        if (renderMat) {
            var sl_thresh = Mathf.Clamp01(1f - component.scanLineJitter.value * 1.2f);
            var sl_disp = 0.002f + Mathf.Pow(component.scanLineJitter.value, 3) * 0.05f;
            var vj = new Vector2(component.verticalJump.value, verticalJumpTime);
            var cd = new Vector2(component.colorDrift.value * 0.04f, Time.time * 606.11f);

            renderMat.SetVector(ShaderIDs.Scan, new Vector2(sl_disp, sl_thresh));
            renderMat.SetVector(ShaderIDs.Vertical, vj);
            renderMat.SetFloat(ShaderIDs.Hoirzontal, component.horizontalShake.value * 0.2f);
            renderMat.SetVector(ShaderIDs.Color, cd);
        }

        cmd.SetGlobalTexture(ShaderIDs.Input, source);

        CoreUtils.DrawFullScreen(cmd, renderMat, destination);
    }
}
