using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endif

namespace Sequencer {

    public enum ScrollDirection {
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }
    
    public class Sequencer : MonoBehaviour {

        public ScrollDirection direction = ScrollDirection.UP;
        public float triggerSize = 30f;
        public float velocity = 2f;
        public float timeScale = 1f;

        public Color gizmosColor = new Color(1f, 0.5f, 1f);
        public bool hidePassedTriggers = true;

        public float scroll = 0;

        public Vector3 GetSequencerPosition(Vector3 position) => transform.InverseTransformPoint(position);
        public Vector3 GetScrollPosition(Vector3 position) => scroller.transform.InverseTransformPoint(position);

        void Start() {
            Prepare();
        }

        void UpdateScrollerPosition() {
            switch (direction)
            {
                case ScrollDirection.RIGHT:
                scroller.transform.localPosition = Vector3.left * scroll;
                break;
                case ScrollDirection.LEFT:
                scroller.transform.localPosition = Vector3.right * scroll;
                break;
                case ScrollDirection.UP:
                scroller.transform.localPosition = Vector3.down * scroll;
                break;
                case ScrollDirection.DOWN:
                scroller.transform.localPosition = Vector3.up * scroll;
                break;
            }
        }

        void Update() {
            scroll += velocity * timeScale * Time.deltaTime * Item.timeScale;
            UpdateScrollerPosition();
        }

        public SequenceTrigger GetTriggerByName(string name) =>
            GetComponentsInChildren<SequenceTrigger>().FirstOrDefault(x => x.gameObject.name == name);

        public void Jump(float destination) {
            scroll = destination;
            UpdateScrollerPosition();
            foreach(var sequence in GetComponentsInChildren<SequenceTrigger>()) {
                sequence.ResetTrigger();
            }
        }
        public void Jump(SequenceTrigger trigger) => Jump(GetScrollPosition(trigger.transform.position).x);
        public void Jump(string name) {

            var sequenceTrigger = GetTriggerByName(name);

            if (sequenceTrigger != null) {

                Jump(sequenceTrigger);

            } else {

                Debug.LogFormat("Il n'y a pas d'objet <SequenceTrigger> qui s'appelle \"{0}\".", name);
            }
        }

        void DrawArrow(Vector3 position, float size) {
            Gizmos.DrawLine(position, position + new Vector3(size, size, 0f));
            Gizmos.DrawLine(position, position + new Vector3(size, -size, 0f));
        }

        void OnDrawGizmos() {
            Gizmos.color = gizmosColor;

            switch(direction) {
                case ScrollDirection.RIGHT:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 0f));
                break;
                case ScrollDirection.LEFT:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 180f));
                break;
                case ScrollDirection.UP:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 90f));
                break;
                case ScrollDirection.DOWN:
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));
                break;
            }
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0f, triggerSize + 2f, 2f));
            
            int max = Mathf.FloorToInt(triggerSize / 2f / 2f);
            for (int i = -max; i <= max; i++) {
                DrawArrow(new Vector3(0f, i * 2f, 0f), 0.5f);
            }

            Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, 0.25f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(0f, triggerSize + 2f, 2f));
        }

        Transform scroller;
        void Prepare() {
            if (scroller == null) {
                scroller = transform.Find("Scroller");
                if (scroller == null) {
                    var go = new GameObject("Scroller");
                    scroller = go.transform;
                    scroller.SetParent(transform);
                }
            }
            scroller.transform.localRotation = Quaternion.identity;
            foreach(var sequence in GetComponentsInChildren<Sequence>()) {
                sequence.transform.SetParent(scroller, false);
            }
            UpdateScrollerPosition();
        }

        void JoinSequences() {
            float x = 0f;
            foreach(var sequence in GetComponentsInChildren<Sequence>()) {
                sequence.transform.localPosition = Vector3.right * x;
                sequence.transform.localRotation = Quaternion.identity;
                x += sequence.sequenceLength;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Sequencer))]
        class MyEditor : Editor {
            Sequencer sequencer => target as Sequencer;
            public override void OnInspectorGUI() {
                PropertyField(serializedObject.FindProperty("direction"));
                LabelField("Scroll", EditorStyles.boldLabel);
                var scrollProp = serializedObject.FindProperty("scroll");
                PropertyField(scrollProp);
                if (serializedObject.hasModifiedProperties) {
                    scrollProp.floatValue = Mathf.Max(scrollProp.floatValue, 0f);
                    serializedObject.ApplyModifiedProperties();
                    sequencer.Prepare();
                }

                Space(16);
                LabelField("Properties", EditorStyles.boldLabel);
                PropertyField(serializedObject.FindProperty("triggerSize"));
                PropertyField(serializedObject.FindProperty("velocity"));
                PropertyField(serializedObject.FindProperty("timeScale"));

                Space(16);
                LabelField("Gizmos", EditorStyles.boldLabel);
                PropertyField(serializedObject.FindProperty("gizmosColor"));
                PropertyField(serializedObject.FindProperty("hidePassedTriggers"));

                Space(16);
                LabelField("Sequences", EditorStyles.boldLabel);
                if (GUILayout.Button("Join Sequences")) {
                    sequencer.JoinSequences();
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
