using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEngineControl : MonoBehaviour {

    public static float EngineRotationRequiredTime = 5.0f;

    private bool frontHead;
    public bool FrontHead {
        get {
            return frontHead;
        }
    }
    private bool adjusting;
    private float adjustTimeCount;

    private void Awake() {
        frontHead = true;
        adjusting = false;
        adjustTimeCount = 0.0f;
    }

    public void setFrontHead(bool frontHead) {
        if (this.frontHead != frontHead) {
            adjusting = true;
            this.frontHead = frontHead;
            adjustTimeCount = 0.0f;
        }
    }

    private void Update() {

        if (adjusting) {

            float strt = frontHead ? 180.0f : 0.0f;
            float dest = frontHead ? 0.0f : 180.0f;
            Vector3 angles = Vector3.Lerp(
                strt * Vector3.forward, 
                dest * Vector3.forward, 
                Mathf.SmoothStep(0.0f, 1.0f, adjustTimeCount / EngineRotationRequiredTime)
            );
            Quaternion rotation = Quaternion.Euler(angles);

            foreach(Transform childTransform in transform) {
                childTransform.localRotation = rotation;
            }
            
            adjustTimeCount += Time.deltaTime;
            if (adjustTimeCount > EngineRotationRequiredTime)
                adjusting = false;

        }
    }

}
