using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.UI {
    public class MenuOption : UIText {
        [Space]
        public Transform submenuTrans;
        public CanvasGroup group;

        [Space]
        public string description = "i shot my wife";
        public string function;

        [Space]
        public bool changeColor = false;
        public Color bgColor = new Color(253, 232, 113);

        [HideInInspector]
        public List<MenuOption> items;

        private bool init;

        public bool HasItems => items.Count != 0;

        protected override void Awake() {
            InitializeText();
            RefreshSubmenu();
            init = true;
        }

        protected void RefreshSubmenu() {
            items = new List<MenuOption>();
            foreach (Transform child in submenuTrans) {
                if (child != transform) {
                    if (child.TryGetComponent<MenuOption>(out var item)) {
                        items.Add(item);
                    }
                }
            }

            if (init) Menu.Instance.ApplySpacing(this);
        }

        protected void InitializeText() {
            base.Awake();
        }
    }
}
