using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PlayerManager;

public class NetworkEntity : NetworkMessageHandler {

    private bool isLerpingPosition;
    private bool isLerpingRotation;
    private Vector3 realPosition;
    private Quaternion realRotation;
    private Vector3 lastRealPosition;
    private Quaternion lastRealRotation;
    private float timeStartedLerping;
    private float timeToLerp;

    private void Start() {

        isLerpingPosition = false;
        isLerpingRotation = false;

        realPosition = transform.position;
        realRotation = transform.rotation;

    }

    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp) {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if (realPosition != transform.position)
            isLerpingPosition = true;
        if (realRotation != transform.rotation)
            isLerpingRotation = true;

        timeStartedLerping = Time.time;
    }

    private void FixedUpdate() {
        NetworkLerp();
    }

    private void NetworkLerp() {

        if(isLerpingPosition) {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            transform.position = Vector3.Lerp(transform.position,
                Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage),
                Time.fixedDeltaTime * 3.0f);
        }
        if(isLerpingRotation) {
            float lerpPercentage = (Time.time  - timeStartedLerping) / timeToLerp;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.Slerp(lastRealRotation, realRotation, lerpPercentage),
                Time.fixedDeltaTime * 3.0f);
        }

    }

}
