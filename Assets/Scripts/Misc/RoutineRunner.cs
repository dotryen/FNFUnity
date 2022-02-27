using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineRunner : MonoBehaviour {
    public static RoutineRunner Instance { get; private set; }

    private void Awake() {
        if (Instance) {
            Destroy(this);
            return;
        }

        Instance = this;
    }
}

public static class RoutineHelper {
    public static Coroutine Run(this IEnumerator func) {
        return RoutineRunner.Instance.StartCoroutine(func);
    }
}
