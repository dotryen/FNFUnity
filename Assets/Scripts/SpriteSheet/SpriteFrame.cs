using UnityEngine;

namespace FNF.Sprites {
    [System.Serializable]
    public struct FrameCollection {
        public string name;
        public SpriteFrame[] frames;

        public int FrameCount => frames.Length;
    }

    [System.Serializable]
    public struct SpriteFrame {
        public RectInt pixels;
        public QuadUV uv;
        public MeshCoord mesh;
    }

    [System.Serializable]
    public struct QuadUV {
        public Vector2 tl;
        public Vector2 tr;
        public Vector2 bl;
        public Vector2 br;
    }

    [System.Serializable]
    public struct MeshCoord {
        public Vector3 tl;
        public Vector3 tr;
        public Vector3 bl;
        public Vector3 br;
    }
}
