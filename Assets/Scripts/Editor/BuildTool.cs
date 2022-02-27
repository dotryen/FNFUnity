using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build;

public static class BuildTools {
    public const string LICENSE_FOLDER = "Licenses\\";

    [PostProcessBuild(1)]
    public static void PostProcessBuild(BuildTarget target, string path) {
        var directory = Path.GetDirectoryName(path);
        Directory.CreateDirectory(directory + "\\" + LICENSE_FOLDER);

        foreach (var file in Directory.GetFiles(LICENSE_FOLDER)) {
            File.Copy(file, directory + "\\" + LICENSE_FOLDER +  Path.GetFileName(file));
        }
    }
}
