using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FNF.Core {
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }

        private string[] maps;
        private Dictionary<string, InputAction>[] actions;

        private PlayerInput input;

        private void Awake() {
            if (Instance) {
                Destroy(gameObject);
                return;
            } else {
                Instance = this;
            }

            input = GetComponent<PlayerInput>();
            maps = new string[input.actions.actionMaps.Count];
            actions = new Dictionary<string, InputAction>[input.actions.actionMaps.Count];

            for (int i = 0; i < maps.Length; i++) {
                var map = input.actions.actionMaps[i];
                maps[i] = map.name;
                actions[i] = new Dictionary<string, InputAction>();

                for (int j = 0; j < map.actions.Count; j++) {
                    actions[i].Add(map.actions[j].name, map.actions[j]);
                }
            }
        }

        private void Start() {
            GetMap("Volume").Enable();
        }

        private InputActionMap GetMap(string name) {
            for (int i = 0; i < input.actions.actionMaps.Count; i++) {
                if (input.actions.actionMaps[i].name == name) {
                    return input.actions.actionMaps[i];
                }
            }
            return null;
        }

        public static void EnableMap(string name) {
            Instance.input.SwitchCurrentActionMap(name);
            Instance.GetMap("Volume").Enable();
        }

        public static Dictionary<string, InputAction> GetMapActions(string map) {
            for (int i = 0; i < Instance.maps.Length; i++) {
                if (Instance.maps[i] == map) {
                    return Instance.actions[i];
                }
            }
            return null;
        }

        public static InputAction GetAction(string map, string name) {
            var dic = GetMapActions(map);
            dic.TryGetValue(name, out InputAction action);
            return action;
        }
    }
}
