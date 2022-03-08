using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NVorbis;

public class SongImportTool : EditorWindow {
    [MenuItem("Tools/Import Song")]
    static void ShowWindow() {
        GetWindow<SongImportTool>().Show();
    }

    public const string AUDIO_FORMAT = "ogg";
    public const string FOLDER = "Assets/" + SongAssets.MUSIC_FOLDER;
    public const string SAMPLE_SCENE = "Assets/Scenes/SampleStage.unity";

    private string songData;
    private string idOverride;
    private string instrumental;
    private string playerVocals;
    private string opponentVocals;

    private string songName;
    private string songDifficulty;
    private SongJSON.Song song;
    private string outputFolder;
    private string chartsFolder;

    private SongMetadata currentMeta;

    private void OnGUI() {
        WindowUtility.LocationField(ref songData, "Song Data", "Select Song Data", "", "json");
        idOverride = EditorGUILayout.TextField("Song ID (Can be empty)", idOverride);
        EditorGUILayout.Space();
        WindowUtility.LocationField(ref instrumental, "Instrumentals", "Select audio", "", AUDIO_FORMAT);
        WindowUtility.LocationField(ref playerVocals, "Player Vocals", "Select audio", "", AUDIO_FORMAT);
        WindowUtility.LocationField(ref opponentVocals, "Opponent Vocals", "Select audio", "", AUDIO_FORMAT);
        EditorGUILayout.Space();

        if (GUILayout.Button("Import chart only")) {
            PrepareForImport();
            ImportChart();
        }
        if (GUILayout.Button("Import Song")) {
            PrepareForImport();
            Import();
        }
    }

    private void PrepareForImport() {
        song = JsonConvert.DeserializeObject<SongJSON.Root>(File.ReadAllText(songData), new Vec3Convert()).song;
        var split = Path.GetFileNameWithoutExtension(songData).Split('-');

        if (split.Length == 1) {
            songName = split[0];
            songDifficulty = "Normal";
        } else {
            songName = split[0];
            songDifficulty = split[1];
        }
        
        outputFolder = FOLDER + (idOverride == string.Empty ? songName : idOverride) + "/";
        chartsFolder = outputFolder + SongAssets.CHARTS_FOLDER;

        Directory.CreateDirectory(outputFolder);
        Directory.CreateDirectory(chartsFolder);

        GetMetadata();
    }

    private void Import() {
        // metadata
        {
            currentMeta.songName = songName;
            currentMeta.length = GetOGGLength(instrumental);
            currentMeta.bpm = Mathf.RoundToInt(song.bpm);
            currentMeta.difficulties = new string[0];

            EditorUtility.SetDirty(currentMeta);
        }

        // chart
        ImportChart();

        // copying
        {
            File.Copy(SAMPLE_SCENE, outputFolder + SongAssets.STAGE, true);
            File.Copy(instrumental, outputFolder + SongAssets.INSTRUMENTAL, true);
            File.Copy(playerVocals, outputFolder + SongAssets.PLAYER_VOCALS, true);
            File.Copy(opponentVocals, outputFolder + SongAssets.OPPONENT_VOCALS, true);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ImportChart() {
        Conductor.SetBPM((int)song.bpm);
        List<Note>[] playerTracks = new List<Note>[] { new List<Note>(), new List<Note>(), new List<Note>(), new List<Note>() };
        List<Note>[] opponentTracks = new List<Note>[] { new List<Note>(), new List<Note>(), new List<Note>(), new List<Note>() };
        int playerNotes = 0;

        foreach (var section in song.notes) {
            SortedList<double, double[]> sorted = new SortedList<double, double[]>(new DuplicateKeyComparer<double>());

            foreach (double[] noteData in section.sectionNotes) {
                sorted.Add(noteData[0], noteData);
            }

            foreach (double[] noteData in sorted.Values) {
                List<Note> toAdd = new List<Note>();

                Note note = new Note {
                    time = (float)noteData[0]
                };

                toAdd.Add(note);
                playerNotes++;

                if (noteData[2] != 0) {
                    int sustainAmount = (int)Math.Ceiling(noteData[2] / Conductor.StepCrochet);

                    // turns out its exclusive LOL
                    for (int i = 0; i < sustainAmount; i++) {
                        Note sustainNote = new Note {
                            time = note.time + (Conductor.StepCrochet * i) + Conductor.StepCrochet,
                            type = i == sustainAmount - 1 ? NoteType.SustainEnd : NoteType.Sustain
                        };

                        toAdd.Add(sustainNote);
                    }
                }

                var side = (int)noteData[1] % 4;
                var playerNote = noteData[1] > 3 ? !section.mustHitSection : section.mustHitSection;

                if (playerNote) playerTracks[side].AddRange(toAdd);
                else opponentTracks[side].AddRange(toAdd);
            }
        }

        var obj = WindowUtility.GetScriptableObj<SongChart>(chartsFolder + songDifficulty + ".asset");
        obj.totalPlayerNoteCount = playerNotes;
        obj.speed = song.speed;
        obj.playerTracks = new NoteTrack[] { new NoteTrack(playerTracks[0]), new NoteTrack(playerTracks[1]), new NoteTrack(playerTracks[2]), new NoteTrack(playerTracks[3]) };
        obj.opponentTracks = new NoteTrack[] { new NoteTrack(opponentTracks[0]), new NoteTrack(opponentTracks[1]), new NoteTrack(opponentTracks[2]), new NoteTrack(opponentTracks[3]) };

        EditorUtility.SetDirty(obj);

        AddDiffToMeta(songDifficulty);
    }

    private void GetMetadata() {
        currentMeta = WindowUtility.GetScriptableObj<SongMetadata>(outputFolder + SongAssets.METADATA);
    }

    private void AddDiffToMeta(string name) {
        var length = currentMeta.difficulties.Length;

        Array.Resize(ref currentMeta.difficulties, length + 1);
        currentMeta.difficulties[length] = name;

        EditorUtility.SetDirty(currentMeta);
    }

    private float GetOGGLength(string file) {
        using (var reader = new VorbisReader(file)) {
            return (float)reader.TotalTime.TotalSeconds;
        }
    }
}

/// <summary>
/// Comparer for comparing two keys, handling equality as beeing greater
/// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable {
    public int Compare(TKey x, TKey y) {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1; // Handle equality as being greater. Note: this will break Remove(key) or
        else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
            return result;
    }
}
