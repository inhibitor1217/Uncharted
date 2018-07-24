using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;

public class NetworkPlayer : NetworkMessageHandler {

    private const float NETWORK_SEND_RATE = 5.0f;

    private string playerID;
    private bool canSendNetworkMovement = false;
    private float timeBetweenMovementStart;
    private float timeBetweenMovementEnd;

    private bool isLocalInitialized = false;

    [HideInInspector]
    public static GameObject Player;

    private void Start() {

        playerID = "player" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        Manager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);

    }

    private void Update() {
        
        if(isLocalPlayer && !isLocalInitialized) {
            isLocalInitialized = true;
            InitializeLocalPlayerSettings();
        }

        if (!canSendNetworkMovement) {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }

    }

    private void OnDestroy() {
        Manager.Instance.RemovePlayerFromConnectedPlayers(playerID);
    }

    private void InitializeLocalPlayerSettings() {
        Manager.Instance.SetLocalPlayerID(playerID);
        canSendNetworkMovement = false;
        NetworkManager.singleton.client.RegisterHandler(movement_msg, OnReceiveMovementMessage);
        Player = gameObject;
    }

    private void OnReceiveMovementMessage(NetworkMessage _message) {
        var _msg = _message.ReadMessage<PlayerMovementMessage>();

        if (_msg.objectTransformName != transform.name) {
            var entity = Manager.Instance.ConnectedPlayers[_msg.objectTransformName];
            if(entity != null) {
                entity.GetComponent<NetworkEntity>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
            }
        }
    }

    private IEnumerator StartNetworkSendCooldown() {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds(1.0f / NETWORK_SEND_RATE);
        SendNetworkMovement();
    }

    private void SendNetworkMovement() {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID, transform.position, transform.rotation, timeBetweenMovementEnd - timeBetweenMovementStart);
        canSendNetworkMovement = false;
    }

    private void SendMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, float _time) {
        var _msg = new PlayerMovementMessage() {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _playerID,
            time = _time
        };
        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

}
