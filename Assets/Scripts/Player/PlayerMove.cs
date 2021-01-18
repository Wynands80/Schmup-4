using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    public float velocity = 10f;

    public bool autoPilot = false;
    float autoPilotTime = 0f;

    void Update() {

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, y, 0f);
        move *= velocity * Time.deltaTime * Item.timeScale;

        if (autoPilot) {
            if (autoPilotTime > 2f) {
                move.x += Mathf.Sin(Time.time * 2f) * 2f * Time.deltaTime;
            }
            autoPilotTime += Time.deltaTime;
            if (Mathf.Abs(x) > 0.01f || Mathf.Abs(y) > 0.01f) {
                autoPilotTime = 0f;
            }
        }

        transform.position += move;

        if (Stage.instance != null) {
            transform.position = Stage.instance.Clamp(transform.position, (0.8f, 0.6f));
        }
    }
}
