using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    public GameObject Pivot;
    public GameObject Player;
    public float MinDistanceToPivot = 5.0f;
    public float MaxDistanceToPivot = 50.0f;
    public float InitialDistance = 15.0f;
    public float CameraZoomSensitivity = 30.0f;
    public float CameraRotateKeyboardSensitivity = 1.0f;
    public float CameraRotateMouseSensitivity = 5.0f;
    public bool EnableZoom = true;
    public bool SmoothCameraMovement = true;
    public bool ReverseCameraMovementX = false;
    public bool ReverseCameraMovementY = false;
    
    private const float AutoAdjustRate = 3.0f;
    private Vector3 IdleEulerAngles = new Vector3(30.0f, 90.0f, 0.0f);
    private const float CameraFollowRate = 10.0f;

    private bool autoAdjusting = false;

    private float distance;
    private Vector3 front, up, left;

    private void Start() {
        distance = InitialDistance;
        front = Pivot.transform.localRotation * Vector3.forward;
        up = Pivot.transform.localRotation * Vector3.up;
        left = Pivot.transform.localRotation * Vector3.left;
    }

    private void Update() {

        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel") * CameraZoomSensitivity;
        if (EnableZoom) {
            distance -= mouseScrollWheel * distance * Time.deltaTime;
            distance = Mathf.Clamp(distance, MinDistanceToPivot, MaxDistanceToPivot);
        }

        if (autoAdjusting) {
            /* Auto adjustment */
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                Player.transform.rotation * Quaternion.Euler(IdleEulerAngles),
                AutoAdjustRate * Time.deltaTime
            );
        } else {
            /* Manual adjustment */
            Quaternion xRotation = Quaternion.identity;
            Quaternion yRotation = Quaternion.identity;

            if (Input.GetMouseButton(1)) {

                float cameraHorizontal = Input.GetAxis("Mouse X") * CameraRotateMouseSensitivity * Mathf.Sqrt(InitialDistance) / Mathf.Sqrt(distance);
                float cameraVertical = Input.GetAxis("Mouse Y") * CameraRotateMouseSensitivity * Mathf.Sqrt(InitialDistance) / Mathf.Sqrt(distance);

                if (ReverseCameraMovementX)
                    cameraHorizontal = -cameraHorizontal;
                if (ReverseCameraMovementY)
                    cameraVertical = -cameraVertical;

                xRotation = Quaternion.AngleAxis(cameraHorizontal, up);
                yRotation = Quaternion.AngleAxis(cameraVertical, left);

            } else {

                float cameraHorizontal = Input.GetAxis("CameraHorizontal") * CameraRotateKeyboardSensitivity * Mathf.Sqrt(InitialDistance) / Mathf.Sqrt(distance);
                float cameraVertical = Input.GetAxis("CameraVertical") * CameraRotateKeyboardSensitivity * Mathf.Sqrt(InitialDistance) / Mathf.Sqrt(distance);

                if (ReverseCameraMovementX)
                    cameraHorizontal = -cameraHorizontal;
                if (ReverseCameraMovementY)
                    cameraVertical = -cameraVertical;

                xRotation = Quaternion.AngleAxis(cameraHorizontal, up);
                yRotation = Quaternion.AngleAxis(cameraVertical, left);

            }

            transform.localRotation = transform.localRotation * xRotation * yRotation;
        }

        if (SmoothCameraMovement) {
            transform.position = Vector3.Lerp(
                transform.position,
                Pivot.transform.position - distance * transform.forward,
                Time.deltaTime * CameraFollowRate
            );
        } else
            transform.position = Pivot.transform.position - distance * transform.forward;

    }

    public void ToggleAutoAdjust() {

        autoAdjusting = !autoAdjusting;

    }

}
