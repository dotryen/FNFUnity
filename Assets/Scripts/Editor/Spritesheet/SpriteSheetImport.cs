using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using FNF.Sprites;

public class SpriteSheetImport : EditorWindow {
    private string atlasFile;
    private string offsetFile;
    private string atlasOutput;
    private string offsetOutput;

    Texture2D tex;
    Vector2Int resolution = new Vector2Int(8192, 4096);

    Vector2Int Resolution => tex ? new Vector2Int(tex.width, tex.height) : resolution;

    [MenuItem("Tools/Import Sprite")]
    static void ShowWindow() {
        GetWindow<SpriteSheetImport>().Show();
    }

    public void OnGUI() {
        EditorGUILayout.LabelField("Texture Atlas");
        EditorGUILayout.Space();

        WindowUtility.LocationField(ref atlasFile, "Atlas", "Pick a spritesheet", "", "xml");
        tex = (Texture2D)EditorGUILayout.ObjectField("Texture", tex, typeof(Texture2D), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        if (tex == null) resolution = EditorGUILayout.Vector2IntField("Resolution", resolution);
        WindowUtility.SaveField(ref atlasOutput, "Sheet data location", "Select Location", "Spritesheet", "asset");

        if (GUILayout.Button("Import")) ImportAtlas();

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Offsets");
        EditorGUILayout.Space();

        WindowUtility.LocationField(ref offsetFile, "Offsets", "Pick an offset file (Kade Engine)", "", "txt");
        WindowUtility.SaveField(ref offsetOutput, "OffsetData location", "Select Location", "OffsetData", "asset");
        if (GUILayout.Button("Import")) ImportOffsets();
    }

    private void ImportAtlas() {
        XMLData.SubTexture[] datas = GetData();
        List<FrameCollection> collections = new List<FrameCollection>();
        List<SpriteFrame> frames = new List<SpriteFrame>();

        var currentCollection = new FrameCollection() {
            name = TrimName(datas[0].name)
        };

        for (int i = 0; i < datas.Length; i++) {
            var data = datas[i];
            string name = TrimName(data.name);

            if (name != currentCollection.name) {
                currentCollection.frames = frames.ToArray();
                collections.Add(currentCollection);
                frames.Clear();
                currentCollection = new FrameCollection() {
                    name = name
                };
            }

            frames.Add(DataToFrame(data));
        }

        currentCollection.frames = frames.ToArray();
        collections.Add(currentCollection);


        SpriteSheetData sheet = AssetDatabase.LoadAssetAtPath<SpriteSheetData>(atlasOutput);

        if (sheet == null) {
            sheet = CreateInstance<SpriteSheetData>();
            AssetDatabase.CreateAsset(sheet, atlasOutput);
        }

        sheet.texture = tex;
        sheet.animations = collections.ToArray();
        EditorUtility.SetDirty(sheet);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ImportOffsets() {
        if (!File.Exists(offsetFile)) return;

        List<Offset> offsets = new List<Offset>();

        foreach (var offset in File.ReadAllLines(offsetFile)) {
            var split = offset.Split(' ');
            offsets.Add(new Offset() {
                name = split[0],
                value = new Vector2((-int.Parse(split[1])) * SpriteGlobals.PIXEL_SIZE, (-int.Parse(split[2])) * SpriteGlobals.PIXEL_SIZE)
            });
        }

        Offsets asset = AssetDatabase.LoadAssetAtPath<Offsets>(offsetOutput);

        if (asset == null) {
            asset = CreateInstance<Offsets>();
            AssetDatabase.CreateAsset(asset, offsetOutput);
        }

        asset.offsets = offsets.ToArray();

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public string TrimName(string value) {
        if (value.Length <= 4) return value;

        string lastFour = value.Substring(value.Length - 4);
        if (lastFour.All(x => char.IsNumber(x))) {
            return value.Substring(0, value.Length - 4);
        } else {
            return value;
        }
    }

    public SpriteFrame DataToFrame(XMLData.SubTexture data) {
        var mesh = new MeshCoord();
        var rect = new RectInt();
        var uv = new QuadUV();

        // mesh
        {
            Rect sizeRect;
            if (data.frameX.HasValue) sizeRect = new Rect(data.frameX.Value, data.frameY.Value, data.frameWidth.Value, data.frameHeight.Value);
            else sizeRect = new Rect(0, 0, data.width, data.height);

            Rect scaled = new Rect(sizeRect.x * SpriteGlobals.PIXEL_SIZE, (-sizeRect.y) * SpriteGlobals.PIXEL_SIZE, sizeRect.width * SpriteGlobals.PIXEL_SIZE, sizeRect.height * SpriteGlobals.PIXEL_SIZE);

            mesh.bl = new Vector3(scaled.xMin, scaled.yMin);
            mesh.br = new Vector3(scaled.xMax, scaled.yMin);
            mesh.tl = new Vector3(scaled.xMin, scaled.yMax);
            mesh.tr = new Vector3(scaled.xMax, scaled.yMax);
        }

        // uv
        {
            uv.tl = UV(new Vector2(data.x, data.y));
            uv.bl = UV(new Vector2(data.x, data.y + data.height));
            uv.tr = UV(new Vector2(data.x + data.width, data.y));
            uv.br = UV(new Vector2(data.x + data.width, data.y + data.height));
        }

        // pixels
        {
            int width = data.frameWidth.HasValue ? data.frameWidth.Value : data.width;
            int height = data.frameHeight.HasValue ? data.frameHeight.Value : data.height;

            rect = new RectInt(data.x, data.y, width, height);
        }

        var frame = new SpriteFrame {
            uv = uv,
            mesh = mesh,
            pixels = rect
        };

        return frame;
    }

    public Rect Bound(Rect frame) {
        float x, y, r, b;
        x = Mathf.Clamp(frame.x, 0, Resolution.x);
        y = Mathf.Clamp(frame.y, 0, Resolution.y);
        r = Mathf.Clamp(frame.xMax, 0, Resolution.x);
        b = Mathf.Clamp(frame.xMin, 0, Resolution.y);

        return new Rect(x, y, r - x, b - y);
    }

    public Vector3 Coordinate(Vector2 pos) {
        return new Vector2(pos.x / Resolution.x, pos.y / Resolution.y);
    }

    public Vector2 UV(Vector2 pos) {
        return new Vector2(pos.x / Resolution.x, (Resolution.y - pos.y) / Resolution.y);
    }

    public XMLData.SubTexture[] GetData() {
        var stream = File.OpenRead(atlasFile);
        var xml = new XmlSerializer(typeof(XMLData.TextureAtlas));

        var root = (XMLData.TextureAtlas)xml.Deserialize(stream);

        stream.Close();

        return root.data;
    }
}
