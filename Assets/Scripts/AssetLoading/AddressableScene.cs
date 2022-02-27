using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableScene {
    public static AddressableScene Get(string address) {
        AssetReference sceneReference;

        try {
            sceneReference = AddressUtil.CreateReference(address);
        } catch {
            return null;
        }

        return new AddressableScene(sceneReference);
    }

    private AssetReference reference;
    private SceneInstance? scene;
    // private Scene? tempScene;
    private bool active;

    public bool Loaded => scene.HasValue;
    public bool Active => active;

    private AddressableScene(AssetReference refer) {
        reference = refer;

        // SceneManager.sceneLoaded += OnSceneChange;
    }

    ~AddressableScene() {
        // SceneManager.sceneLoaded -= OnSceneChange;
    }

    public AsyncOperationHandle? Load(LoadSceneMode mode = LoadSceneMode.Single) {
        if (Loaded) return null;

        var operation = reference.LoadSceneAsync(mode, false);
        operation.Completed += (handle) => {
            scene = handle.Result;
        };

        return operation;
    }

    public IEnumerator Activate() {
        if (Active || !Loaded) return null;
        return ActivateRoutine();
    }

    public IEnumerator Unload() {
        if (!Loaded) return null;
        // Debug.Log(Environment.StackTrace);
        return UnloadRoutine();
    }

    public IEnumerator Reload(LoadSceneMode mode = LoadSceneMode.Single) {
        if (!Loaded) return null;
        return ReloadRoutine(mode);
    }

    public void Release() {
        if (Loaded) return;
        reference.ReleaseAsset();
    }

    private IEnumerator ActivateRoutine() {
        yield return ScreenFader.EnterFade();

        yield return scene.Value.ActivateAsync();
        active = true;

        yield return ScreenFader.ExitFade();
    }

    private IEnumerator UnloadRoutine() {
        yield return ScreenFader.EnterFade();

        SceneManager.CreateScene("TEMP_SCENE");
        yield return reference.UnLoadScene();
        // yield return SceneManager.UnloadSceneAsync(scene.Value.Scene);

        scene = null;
        active = false;
    }

    private IEnumerator ReloadRoutine(LoadSceneMode mode) {
        yield return UnloadRoutine();
        // yield return scene.Value.ActivateAsync();
        yield return Load(mode);

        yield return scene.Value.ActivateAsync();
        active = true;

        yield return ScreenFader.ExitFade();
    }
}
