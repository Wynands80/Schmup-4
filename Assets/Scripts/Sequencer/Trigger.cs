using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sequencer {

    [ExecuteAlways]
    public class SequenceTrigger : MonoBehaviour {

        public float radius = 0.5f;
        public float offset = 0;

        [System.NonSerialized]
        public Sequencer sequencer;
        protected Sequence sequence;
        // [System.NonSerialized]
        public Vector3 sequencerPosition;
        protected Vector3 sequencerPositionOld;

        [Header("Gizmos")]
        public bool showLabel = true;
        public bool alwaysDisplayLabel = false;
        public bool shortLabel = false;

        public OptionColor customColor = new OptionColor();

        protected virtual string GetName() => gameObject.name;
        protected virtual string GetHandleLabel() => GetName();

        public void SetEnabled(bool value) => enabled = value;

        public bool IsValid() => sequence != null && sequencer != null;
        public Vector3 PositionWithOffset => transform.position + transform.right * offset;
        public Vector3 GetSequencerPosition() => sequencer?.GetSequencerPosition(PositionWithOffset) ?? Vector3.zero;
        
        public void ResetTrigger() {
            sequencerPosition = GetSequencerPosition();
            sequencerPositionOld = sequencerPosition;
        }

        void Init() {

            gameObject.name = GetName();

            sequencer = GetComponentInParent<Sequencer>();
            sequence = GetComponentInParent<Sequence>();
            
            ResetTrigger();

            // clean at runtime
            if (Application.isPlaying) {
                while (transform.childCount > 0) {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
        }

        void Start() {
            Init();
        }

        public bool IsInsideSequencer() {

            if (sequencer == null) return true;

            float max = sequencer?.triggerSize / 2 ?? 0f;
            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                case ScrollDirection.LEFT:
                return sequencerPosition.x >= -max && sequencerPosition.x <= max;
                case ScrollDirection.UP:
                case ScrollDirection.DOWN:
                return sequencerPosition.y >= -max && sequencerPosition.y <= max;
            }

            throw new System.Exception("That may not happen!");
        }

        public bool ShouldTrigger() {
            
            if (sequencer == null) return false;

            switch(sequencer.direction) {
                case ScrollDirection.RIGHT:
                return sequencerPosition.x <= radius && sequencerPositionOld.x > radius;
                case ScrollDirection.LEFT:
                return sequencerPosition.x >= -radius && sequencerPositionOld.x < -radius;
                case ScrollDirection.UP:
                return sequencerPosition.y <= radius && sequencerPositionOld.y > radius;
                case ScrollDirection.DOWN:
                return sequencerPosition.y >= -radius && sequencerPositionOld.y < -radius;                
            }

            throw new System.Exception("That may not happen!");
        }

        void Update() {

#if UNITY_EDITOR
            if (Application.isPlaying == false) {
                Init();
            }
#endif
            
            sequencerPosition = GetSequencerPosition();

            float max = sequencer?.triggerSize / 2 ?? 0f;
            if (IsInsideSequencer()) {
                if (ShouldTrigger()) {
                    if (Application.isPlaying) {
                        Trigger();
                        SendMessage("OnTriggerSequence", this, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        void LateUpdate() {
            sequencerPositionOld = sequencerPosition;
        }

        protected virtual void Trigger() {

        }

        public void DrawGizmos() {
#if UNITY_EDITOR

            bool hidePassedSpawner = sequencer?.hidePassedTriggers ?? false;
            var gizmosColor = sequencer?.gizmosColor ?? Color.red;

            if (true) {

                Gizmos.color = customColor.active ? customColor.color : gizmosColor;

                if (!enabled) {
                    Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
                }
                
                bool selected = Utils.GetSelected(transform);
                var positionWithOffset = PositionWithOffset;

                Gizmos.DrawLine(transform.position, positionWithOffset);
                Gizmos.DrawSphere(transform.position, selected ? 0.15f : 0.1f);
                foreach (var (A, B) in Utils.ChordAround(positionWithOffset, radius)) {
                    Gizmos.DrawLine(A, B);
                }
                if (showLabel) {
                    Vector3 cameraPos = Camera.current.WorldToScreenPoint(transform.position);
                    if (alwaysDisplayLabel || cameraPos.z < 15f) {
                        string label = GetHandleLabel();
                        if (shortLabel) {
                            label = Utils.CapitalsOnly(label);
                        }
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = Gizmos.color;
                        Handles.Label(positionWithOffset, label, style);
                    }
                }

                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, selected ? 0.1f : 0f);
                Gizmos.DrawMesh(Utils.disc, positionWithOffset, Quaternion.identity, Vector3.one * radius / 0.5f);
            }
#endif
        }

        void OnDrawGizmos() {
            DrawGizmos();
        }
    }
}
