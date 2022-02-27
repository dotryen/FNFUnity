using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressUtil {
    public static ResourceLocationMap GetMap(int group = 0) {
        var map = (ResourceLocationMap)Addressables.ResourceLocators.ElementAt(group);
        return map;
    }

    public static KeyValuePair<string, string>[] GetAllEntries(int group = 0) {
        return GetMap(group).Locations.Select(x => new KeyValuePair<string, string>(x.Key.ToString(), x.Value.ToString())).ToArray();
    }

    public static bool FolderContainsAsset(string folder, string asset, int group = 0) {
        return GetAllEntries(group).Any(x => x.Key == (folder + asset));
    }

    public static string GetGUID(string address, int group = 0) {
        var map = GetMap(group);
        return map.Locations.First(x => (string)x.Key != address && x.Value[0].PrimaryKey == address).Key.ToString();
    }

    public static AssetReference CreateReference(string address, int group = 0) {
        return new AssetReference(GetGUID(address, group));
    }
}
