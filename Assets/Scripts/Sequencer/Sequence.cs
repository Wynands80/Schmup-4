using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequencer {

    public class Sequence : MonoBehaviour {

        public float sequenceLength = 20f;
        public Vector3 initialPosition;

        void Start() {
            initialPosition = transform.localPosition;
        }





        static void GizmosAlpha(float value) {
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, value);
        }

        void DrawRight(float triggerSize) {
            Gizmos.DrawWireCube(Vector3.right * sequenceLength / 2f, new Vector3(sequenceLength, triggerSize, 0));
            GizmosAlpha(0.25f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(0f, i, 0f), new Vector3(sequenceLength, i, 0f));
            }
        }

        void DrawLeft(float triggerSize) {
            Gizmos.DrawWireCube(Vector3.left * sequenceLength / 2f, new Vector3(sequenceLength, triggerSize, 0));
            GizmosAlpha(0.25f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(-sequenceLength, i, 0f), new Vector3(0, i, 0f));
            }
        }

        void DrawUp(float triggerSize) {
            Gizmos.DrawWireCube(Vector3.up * sequenceLength / 2f, new Vector3(triggerSize, sequenceLength, 0));
            GizmosAlpha(0.25f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(i, 0f, 0f), new Vector3(i, sequenceLength, 0f));
            }
        }

        void DrawDown(float triggerSize) {
            Gizmos.DrawWireCube(Vector3.down * sequenceLength / 2f, new Vector3(triggerSize, sequenceLength, 0));
            GizmosAlpha(0.25f);
            int max = Mathf.FloorToInt(triggerSize / 2f);
            for (int i = -max; i <= max; i++) {
                Gizmos.DrawLine(new Vector3(i, -sequenceLength, 0f), new Vector3(i, 0f, 0f));
            }
        }

        void OnDrawGizmos() {
            
            Sequencer sequencer = GetComponentInParent<Sequencer>();
            float triggerSize = sequencer?.triggerSize ?? 10f;
            ScrollDirection direction = sequencer?.direction ?? ScrollDirection.RIGHT;
            Gizmos.color = sequencer?.gizmosColor ?? Color.red;

            Gizmos.matrix = transform.localToWorldMatrix;
            switch (direction) {

                case ScrollDirection.RIGHT:
                DrawRight(triggerSize);
                break;

                case ScrollDirection.LEFT:
                DrawLeft(triggerSize);
                break;

                case ScrollDirection.UP:
                DrawUp(triggerSize);
                break;

                case ScrollDirection.DOWN:
                DrawDown(triggerSize);
                break;
            }
        }
    }
}
