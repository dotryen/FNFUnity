using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

[Preserve]
public class PauseMenuFunc : MenuFunctions {
    [Preserve]
    public void Resume() {
        GameController.Instance.Unpause();
    }

    [Preserve]
    public void Restart() {
        GameController.Instance.ReloadScene();
    }

    [Preserve]
    public void ToMenu() {
        GameController.Instance.ReturnToMenu();
    }
}
