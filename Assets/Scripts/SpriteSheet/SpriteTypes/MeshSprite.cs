using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshSprite : BaseSprite {
    [Space]
    public string colorID = "_Color";

    protected MeshFilter filter;
    protected new MeshRenderer renderer;
    protected Mesh mesh;
    protected Material material;

    private Color cachedColor;

    public override Color Color {
        get {
            return cachedColor;
        }

        set {
            cachedColor = value;
            material.SetColor(colorID, value);
        }
    }

    public override void SetupSprite() {
        filter = GetComponent<MeshFilter>();
        renderer = GetComponent<MeshRenderer>();
        cachedColor = Color.white;

        if (filter.mesh == null) {
            mesh = new Mesh();
            mesh.name = "Sprite Mesh";

            filter.mesh = mesh;
        } else {
            mesh = filter.mesh;
        }

        material = new Material(renderer.material);
        renderer.material = material;
        material.SetTexture("_MainTex", data.texture);
    }

    public override void SetFrame(SpriteFrame frame) {
        mesh.SetVertices(new Vector3[] { frame.mesh.bl, frame.mesh.br, frame.mesh.tl, frame.mesh.tr });
        mesh.SetUVs(0, new Vector2[] { frame.uv.bl, frame.uv.br, frame.uv.tl, frame.uv.tr });
        mesh.RecalculateBounds();
    }
}
