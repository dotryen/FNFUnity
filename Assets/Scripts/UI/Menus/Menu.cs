using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace FNF.UI {
    using Core;

    public class Menu : UIBehaviour {
        public static Menu Instance { get; private set; }

        public bool Active {
            get {
                return active;
            }

            set {
                active = value;
            }
        }
        [SerializeField]
        private bool active = true;

        [Header("Options")]
        public float leftOffset = 90f;
        public float sectionSpacing = 600f;
        [Range(0f, 1f)]
        public float lerpScale = 0.16f;

        [Space]
        public Vector2 optionMoveAmount = new Vector2(20f, 120f);
        public float backgroundMoveDivisor = 10f;

        [Header("Visuals")]
        public Image background;
        public TextMeshProUGUI description;

        [Header("Audio")]
        public AudioSource source;
        public AudioClip selectClip;
        public AudioClip backClip;

        private MenuFunctions functions;
        private List<MenuOption> rootItems;
        private Stack<MenuOption> optionStack;
        private Stack<int> stackIndexes;
        private Color targetColor;
        private Color cachedColor;
        private Dictionary<string, InputAction> inputMap;

        public int CurrentIndex { get; set; }
        public List<MenuOption> CurrentList => optionStack.Count == 0 ? rootItems : optionStack.Peek().items;
        public MenuOption CurrentItem => CurrentList[CurrentIndex];
        public Color TargetColor {
            get {
                return targetColor;
            }

            set {
                if (Active) return;
                targetColor = value;
            }
        }

        private float LerpAmount => Utils.MakeFrameIndependant(Time.unscaledDeltaTime, lerpScale, 60);

        protected override void Awake() {
            base.Awake();
            functions = GetComponent<MenuFunctions>();
            inputMap = InputManager.GetMapActions("Menus");

            rootItems = new List<MenuOption>();
            optionStack = new Stack<MenuOption>();
            stackIndexes = new Stack<int>();

            cachedColor = background.color;
            targetColor = cachedColor;

            foreach (Transform child in transform) {
                if (child != transform) {
                    if (child.TryGetComponent<MenuOption>(out var item)) {
                        rootItems.Add(item);
                    }
                }
            }

            Instance = this;
        }

        private void Start() {
            foreach (var child in rootItems) {
                ApplySpacing(child);
            }
        }

        protected virtual void Update() {
            if (Active) ProcessInputs();
            Render();
        }

        protected void ProcessInputs() {
            int modifier = 0;
            if (inputMap["Up"].WasPressedThisFrame()) modifier--;
            if (inputMap["Down"].WasPressedThisFrame()) modifier++;

            CurrentIndex = (int)Mathf.Repeat(CurrentIndex + modifier, CurrentList.Count);
            if (modifier != 0) source.PlayOneShot(selectClip);

            // entering submenus
            if (CurrentItem.HasItems) {
                if (inputMap["Right"].WasPressedThisFrame()) {
                    var item = CurrentItem;

                    optionStack.Push(item);
                    stackIndexes.Push(CurrentIndex);

                    CurrentIndex = 0;
                    if (item.changeColor) targetColor = item.bgColor;

                    source.PlayOneShot(selectClip);
                }
            } else if (inputMap["Submit"].WasPressedThisFrame()) { // button
                functions.Invoke(CurrentItem.function, 0);
                functions.InvokingMenu = this;
                functions.InvokingOption = CurrentItem;
            }

            // exiting submenus
            if (optionStack.Count > 0) {
                if (inputMap["Left"].WasPressedThisFrame()) {
                    optionStack.Pop();
                    CurrentIndex = stackIndexes.Pop();

                    if (optionStack.Count == 0) {
                        targetColor = cachedColor;
                    } else {
                        var item = optionStack.Peek();
                        if (item.changeColor) targetColor = item.bgColor;
                    }

                    source.PlayOneShot(backClip);
                }
            }
        }

        protected void Render() {
            // beginning layout
            for (int i = 0; i < rootItems.Count; i++) {
                var xOff = leftOffset - (sectionSpacing * optionStack.Count);
                var index = optionStack.Count != 0 ? stackIndexes.ElementAt(optionStack.Count - 1) : CurrentIndex;

                var item = rootItems[i];
                var transform = item.transform;
                var target = i - index;
                // var scaled = Remap(target, 0, 1, 0, 1.3f);
                var scaled = target * 1.3f;

                Vector2 pos = transform.anchoredPosition;
                pos.x = Mathf.Lerp(pos.x, (target * optionMoveAmount.x) + xOff, LerpAmount);
                pos.y = Mathf.Lerp(pos.y, (scaled * -optionMoveAmount.y) + (720 * -0.48f), LerpAmount);
                transform.anchoredPosition = pos;
            }

            // current list
            if (optionStack.Count != 0) {
                for (int i = 0; i < CurrentList.Count; i++) {
                    var item = CurrentList[i];
                    var transform = item.transform;
                    var target = i - CurrentIndex;
                    var scaled = target * 1.3f;

                    Vector2 pos = transform.anchoredPosition;
                    pos.x = Mathf.Lerp(pos.x, (target * optionMoveAmount.x) + sectionSpacing, LerpAmount);
                    pos.y = Mathf.Lerp(pos.y, scaled * -optionMoveAmount.y, LerpAmount);
                    transform.anchoredPosition = pos;
                }
            }

            // alpha checks
            for (int i = 0; i < CurrentList.Count; i++) {
                var item = CurrentList[i];

                float parentAlpha = 0.6f;
                float childAlpha = 0f;
                bool selected = i == CurrentIndex;

                if (selected) parentAlpha = 1f;
                if (item.HasItems) {
                    if (selected) {
                        childAlpha = 0.2f;
                    }
                }

                item.group.alpha = parentAlpha;
                foreach (var child in item.items) {
                    child.group.alpha = childAlpha;
                }
            }

            // background shit
            {
                var backgroundMoveAmount = optionMoveAmount / backgroundMoveDivisor;
                var totalMoveAmount = (CurrentList.Count - 1) * backgroundMoveAmount;

                var pos = background.rectTransform.anchoredPosition;
                var targetPos = new Vector2(backgroundMoveAmount.x * stackIndexes.Count * -1f, (backgroundMoveAmount.y * CurrentIndex) - (totalMoveAmount.y / 2f));

                background.rectTransform.anchoredPosition = Vector2.Lerp(pos, targetPos, LerpAmount);
                background.color = Color.Lerp(background.color, targetColor, LerpAmount);
            }

            description.text = CurrentItem.description;
        }

        public void ApplySpacing(MenuOption item) {
            for (int i = 0; i < item.items.Count; i++) {
                var child = item.items[i];
                child.transform.anchoredPosition = new Vector2(sectionSpacing, (i * 1.3f) * -optionMoveAmount.y);
                child.group.alpha = 0f;
                ApplySpacing(child);
            }
        }

        public void ActivateNextFrame() {
            StartCoroutine(EnableNextFrame());
        }

        IEnumerator EnableNextFrame() {
            yield return null;
            Active = true;
        }
    }
}
