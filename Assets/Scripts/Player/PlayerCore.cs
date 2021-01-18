using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerCore : MonoBehaviour {

    void OnTriggerEnter(Collider other) {

        string layerName = LayerMask.LayerToName(other.gameObject.layer);
    
        if (layerName == "AI" || layerName == "AIBullet") {

            if (Player.player.IsInvincible()) {
                Debug.Log("Haha ! Je suis invincible !!!");
                return;
            }

            Destroy(transform.parent.gameObject);
            
            Player.player.RemoveOneHp();
        }
    }
}
