using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour {

    private GameObject player;

    private void Update() {
        if (player == null) {
            player = NetworkPlayer.Player;
        } else {
            transform.position = player.transform.position;
            transform.rotation = player.transform.rotation;
        }
    }

}
