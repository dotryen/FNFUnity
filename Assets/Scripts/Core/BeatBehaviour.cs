using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.Core {
    public class BeatBehaviour : MonoBehaviour {
        private int curStep;

        protected virtual void Update() {
            var cache = curStep;
            curStep = GameplayVars.CurrentStep;

            if (curStep != cache) {
                OnStep();
                if (curStep % 4 == 0) OnBeat();
            }
        }

        protected virtual void OnStep() {

        }

        protected virtual void OnBeat() {

        }
    }
}
