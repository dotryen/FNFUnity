using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SongAssets {
    #region Static

    public const string MUSIC_FOLDER = "Songs/";

    public const string STAGE = "Stage.unity";
    public const string METADATA = "Meta.asset";
    public const string CHARTS_FOLDER = "Charts/";
    public const string INSTRUMENTAL = "Inst.ogg";
    public const string VOCAL_SUFFFIX = "Vocals.ogg";
    public const string PLAYER_VOCALS = "Player" + VOCAL_SUFFFIX;
    public const string OPPONENT_VOCALS = "Opponent" + VOCAL_SUFFFIX;

    private static Dictionary<string, SongAssets> loadedSongs = new Dictionary<string, SongAssets>();

    private SongAssets(string name, string path) {
        this.name = name;
        this.path = path;
    }

    public static SongAssets GetSong(string name) {
        if (loadedSongs.TryGetValue(name, out var loaded)) {
            return loaded;
        }

        var path = $"{MUSIC_FOLDER}{name}/";
        SongAssets song = new SongAssets(name, path);

        song.stageRef = AddressableScene.Get(path + STAGE);
        if (song.stageRef == null) return null;

        try {
            // var map = AddressUtil.GetMap(0);
            song.metaRef = AddressUtil.CreateReference(path + METADATA);
            // song.chartRef = AddressUtil.CreateReference(path + CHART);
            song.instRef = AddressUtil.CreateReference(path + INSTRUMENTAL);
            song.pVocalsRef = AddressUtil.CreateReference(path + PLAYER_VOCALS);
            song.oVocalsRef = AddressUtil.CreateReference(path + OPPONENT_VOCALS);
        } catch {
            // this means a guid was not found
            Debug.LogError($"Could not load song \"{name}\"");
            return null;
        }

        loadedSongs.Add(name, song);
        return song;
    }

    public static void Unload(string name) {
        if (loadedSongs.TryGetValue(name, out var loaded)) {
            loaded.UnloadInternal(false);
            loadedSongs.Remove(name);
        }
    }

    public static Dictionary<string, string[]> GetAllSongs() {
        var dic = new Dictionary<string, List<string>>();
        var map = AddressUtil.GetMap();

        foreach (var loc in map.Locations) {
            var key = loc.Key.ToString();
            if (key.StartsWith(MUSIC_FOLDER)) {
                var split = key.Split('/');
                var songName = split[1];

                if (!dic.ContainsKey(songName)) dic.Add(songName, new List<string>());

                if (key.Contains(CHARTS_FOLDER) && key.EndsWith(".asset")) {
                    var asset = split[split.Length - 1];
                    dic[songName].Add(asset.Substring(0, asset.Length - 6));
                }
            }
        }

        var realOne = new Dictionary<string, string[]>();
        foreach (var pair in dic) {
            realOne.Add(pair.Key, pair.Value.ToArray());
        }
        return realOne;
    }

    #endregion

    #region Instance

    private readonly string path;
    private readonly string name;

    private AddressableScene stageRef;
    private AssetReference metaRef;
    private AssetReference chartRef;
    private AssetReference instRef;
    private AssetReference pVocalsRef;
    private AssetReference oVocalsRef;

    private string currentChart;

    public bool MetadataLoaded { get; private set; }
    public bool AudioLoaded { get; private set; }
    public bool ChartLoaded => string.IsNullOrEmpty(currentChart);
    public bool StageReady => stageRef.Loaded;

    public string Name => name;
    public string CurrentDifficulty => currentChart;
    public string FullName => name + "-" + currentChart;

    public SongMetadata Metadata { get; private set; }
    public SongChart Chart { get; private set; }
    public AudioClip Instrumentals { get; private set; }
    public AudioClip PlayerVocals { get; private set; }
    public AudioClip OpponentVocals { get; private set; }

    public AsyncOperationHandle? LoadMeta() {
        if (MetadataLoaded) return null;
        var operation = metaRef.LoadAssetAsync<SongMetadata>();
        operation.Completed += (handle) => {
            Metadata = handle.Result;
            MetadataLoaded = true;
        };
        return operation;
    }

    public AsyncOperationHandle? LoadChart(string difficulty) {
        if (difficulty == currentChart) return null;

        UnloadRef(chartRef);
        chartRef = AddressUtil.CreateReference(path + CHARTS_FOLDER + difficulty + ".asset");

        var operation = chartRef.LoadAssetAsync<SongChart>();
        operation.Completed += (handle) => {
            Chart = handle.Result;
            currentChart = difficulty;
        };
        return operation;
    }

    public AsyncOperationHandle[] LoadAudio() {
        if (AudioLoaded) return null;
        AsyncOperationHandle[] handles = new AsyncOperationHandle[3];

        handles[0] = instRef.LoadAssetAsync<AudioClip>();
        handles[0].Completed += (handle) => {
            Instrumentals = (AudioClip)handle.Result;
        };

        handles[1] = pVocalsRef.LoadAssetAsync<AudioClip>();
        handles[1].Completed += (handle) => {
            PlayerVocals = (AudioClip)handle.Result;
        };

        handles[2] = oVocalsRef.LoadAssetAsync<AudioClip>();
        handles[2].Completed += (handle) => {
            OpponentVocals = (AudioClip)handle.Result;
            AudioLoaded = true; // They are not loaded at the same time, they are in a queue.
        };

        return handles;
    }

    public Coroutine LoadStage() {
        var operation = stageRef.Load();

        return operation.Run();
    }

    public Coroutine ActivateStage() {
        var operation = stageRef.Activate();

        return operation.Run();
    }

    public Coroutine UnloadStage() {
        var operation = stageRef.Unload();

        return operation.Run();
    }

    public Coroutine ReloadStage() {
        var operation = stageRef.Reload();

        return operation.Run();
    }

    public void Unload() {
        UnloadInternal(true);
    }

    private void UnloadInternal(bool remove) {
        stageRef.Release();
        UnloadRef(metaRef);
        UnloadRef(chartRef);
        UnloadRef(instRef);
        UnloadRef(pVocalsRef);
        UnloadRef(oVocalsRef);
        currentChart = null;

        if (remove) {
            loadedSongs.Remove(name);
        }
    }

    private void UnloadRef(AssetReference reference) {
        if (reference == null) return;
        if (!reference.IsDone) return;
        reference.ReleaseAsset();
    }

    #endregion
}
