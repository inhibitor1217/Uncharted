using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    /* Key Inputs */
    private const KeyCode TOGGLE_CAMERA_ANCHOR = KeyCode.Y;
    private const KeyCode SET_MOVE_ACCELERATE = KeyCode.Q;
    private const KeyCode SET_MOVE_NEUTRAL = KeyCode.E;
    private const KeyCode SET_MOVE_DECELERATE = KeyCode.R;

    private const float MovementUpdateRate = 0.100f;
    private float movementUpdateTimeCount = 0.0f;

    public ThirdPersonCamera MainCamera;
    public ThirdPersonCamera GlobeUIRenderCamera;
    public PlayerMovement Movement;
    public Button AccelerateButton;
    public Button NeutralButton;
    public Button DecelerateButton;

    private void Start() {
        MoveNeutral();
    }

    private void Update() {
        
        if(Input.GetKeyDown(TOGGLE_CAMERA_ANCHOR)) {
            MainCamera.ToggleAutoAdjust();
            GlobeUIRenderCamera.ToggleAutoAdjust();
        }

        if(Movement.CanHandleInput) {

            if (Input.GetKeyDown(SET_MOVE_ACCELERATE)) {
                MoveAccelerate();
            }

            if (Input.GetKeyDown(SET_MOVE_NEUTRAL)) {
                MoveNeutral();
            }

            if (Input.GetKeyDown(SET_MOVE_DECELERATE)) {
                MoveDecelerate();
            }

        }

    }

    public void MoveAccelerate() {

        Movement.IsAccelerating = true;
        Movement.IsDecelerating = false;

        AccelerateButton.GetComponent<ToggleButton>().Toggle(true);
        NeutralButton.GetComponent<ToggleButton>().Toggle(false);
        DecelerateButton.GetComponent<ToggleButton>().Toggle(false);

    }

    public void MoveNeutral() {

        Movement.IsAccelerating = false;
        Movement.IsDecelerating = false;
        
        AccelerateButton.GetComponent<ToggleButton>().Toggle(false);
        NeutralButton.GetComponent<ToggleButton>().Toggle(true);
        DecelerateButton.GetComponent<ToggleButton>().Toggle(false);

    }

    public void MoveDecelerate() {

        Movement.IsAccelerating = false;
        Movement.IsDecelerating = true;

        AccelerateButton.GetComponent<ToggleButton>().Toggle(false);
        NeutralButton.GetComponent<ToggleButton>().Toggle(false);
        DecelerateButton.GetComponent<ToggleButton>().Toggle(true);

    }

}
