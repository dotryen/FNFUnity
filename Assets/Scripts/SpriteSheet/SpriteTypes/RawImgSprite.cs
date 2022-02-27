using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RawImgSprite : BaseSprite {
    [HideInInspector]
    public RectTransform rect;
    [HideInInspector]
    public RawImage image;
    public bool resize = false;

    public override Color Color { get => image.color; set => image.color = value; }

    public RectTransform RectTransform => rect;
    public RawImage Image => image;

    private Material cache;

    public override void SetupSprite() {
        image = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();
        Color = Color.white;

        image.texture = data.texture;
        if (image.material) {
            cache = new Material(image.material);
            image.material = cache;
        }
    }

    public override void SetFrame(SpriteFrame frame) {
        Rect uv = new Rect {
            x = frame.uv.tl.x,
            y = frame.uv.bl.y,
            width = frame.uv.tr.x - frame.uv.tl.x,
            height = frame.uv.tr.y - frame.uv.bl.y
        };

        image.uvRect = uv;
        if (resize) rect.sizeDelta = frame.pixels.size;
        if (cache) cache.SetVector("_Repeat", new Vector2(uv.xMax, uv.yMax));
    }
}
