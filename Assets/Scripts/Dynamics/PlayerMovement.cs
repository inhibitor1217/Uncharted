using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    public Text StateDisplayText;
    public Text SubstateDisplayText;

    private Vector3 playerPosition = Vector3.zero;
    private Quaternion playerRotation = Quaternion.identity;

    private bool isAccelerating = false;
    public bool IsAccelerating {
        get {
            return isAccelerating;
        }
        set {
            if(isAccelerating != value) {
                ToggleAcceleration();
            }
        }
    }

    private bool isDecelerating = false;
    public bool IsDecelerating {
        get {
            return isDecelerating;
        }
        set {
            if(isDecelerating != value) {
                ToggleDeceleration();
            }
        }
    }

    private bool canHandleInput = true;
    public bool CanHandleInput {
        get {
            return canHandleInput;
        }
    }

    private float speed;
    public bool IsStationary {
        get {
            return speed < 1e-2;
        }
    }
    private float acceleration;

    private const float ACCELERATION = 0.5f;
    private const float DECELERATION = -0.5f;
    private const float MAX_SPEED = 20.0f;

    private bool canStartAcceleration;
    private bool canStartDeceleration;

    public RectTransform SpeedBar;
    private const float SPEEDBAR_MAX_WIDTH = 200.0f;

    public Text SpeedDisplayText;
    private const float SPEED_MODIFIER = 300.0f;
    private const float SPEED_UI_CHANGE_COOLDOWN = 0.5f;
    private bool canChangeSpeedUI = true;
    private Vector3 SPEEDBAR_COLOR_IDLE = new Vector3(0.000f, 0.784f, 1.000f);
    private Vector3 SPEEDBAR_COLOR_ACCEL = new Vector3(1.000f, 0.235f, 0.000f);

    private void Update() {

        /* Player Update */
        speed += acceleration * Time.deltaTime;
        if (speed < 0.0f) {
            speed = 0.0f;
            if (isDecelerating)
                GetComponent<InputManager>().MoveNeutral();
        }
        if(speed > MAX_SPEED) {
            speed = MAX_SPEED;
            if (isAccelerating)
                GetComponent<InputManager>().MoveNeutral();
        }

        playerPosition += speed * (playerRotation * Vector3.right) * Time.deltaTime;

        if (NetworkPlayer.Player != null) {
            NetworkPlayer.Player.transform.position = playerPosition;
            NetworkPlayer.Player.transform.rotation = playerRotation;
        }
        /* Player Update End */

        /* UI Update */
        if (canChangeSpeedUI) {
            canChangeSpeedUI = false;
            StartCoroutine(WaitForSpeedUIChange());
        }
        SpeedBar.sizeDelta = new Vector2(SPEEDBAR_MAX_WIDTH * speed / MAX_SPEED, 0.0f);
        Vector3 curColor = Vector3.Lerp(SPEEDBAR_COLOR_IDLE,
            Vector3.Lerp(SPEEDBAR_COLOR_IDLE, SPEEDBAR_COLOR_ACCEL, speed / MAX_SPEED),
            speed / MAX_SPEED);
        SpeedBar.GetComponent<Image>().color = new Color(curColor.x, curColor.y, curColor.z);
        /* UI Update End */

        /* Animation Handling */
        if (canStartAcceleration) {
            canStartAcceleration = false;
            StartCoroutine(WaitForEngineMovement(ACCELERATION));
        }
        if(canStartDeceleration) {
            canStartDeceleration = false;
            StartCoroutine(WaitForEngineMovement(DECELERATION));
        }
        /* Animation Handling End */

    }

    private IEnumerator WaitForSpeedUIChange() {
        yield return new WaitForSeconds(SPEED_UI_CHANGE_COOLDOWN);
        SpeedDisplayText.text = (speed * SPEED_MODIFIER).ToString("F1") + " m/s";
        canChangeSpeedUI = true;
    }

    private IEnumerator WaitForEngineMovement(float _acceleration) {
        yield return new WaitForSeconds(ShipEngineControl.EngineRotationRequiredTime);
        acceleration = _acceleration;
        canHandleInput = true;
        StateDisplayText.text = "Engine ON";
        setEngineEffect(true);
    }

    private void ToggleAcceleration() {

        if(isAccelerating) {
            isAccelerating = false;
            acceleration = 0.0f;
            if(!isDecelerating) {
                StateDisplayText.text = "Engine OFF";
                SubstateDisplayText.text = "On Cruise";
                setEngineEffect(false);
            }
        } else {
            isAccelerating = true;
            SubstateDisplayText.text = "ACCELERATING";
            if (!NetworkPlayer.Player.GetComponentInChildren<ShipEngineControl>().FrontHead) {
                acceleration = 0.0f;
                canHandleInput = false;
                StateDisplayText.text = "Adjusting Module";
                NetworkPlayer.Player.GetComponentInChildren<ShipEngineControl>().setFrontHead(true);
                canStartAcceleration = true;
                setEngineEffect(false);
            } else {
                acceleration = 0.5f;
                StateDisplayText.text = "Engine ON";
                setEngineEffect(true);
            }
        }

    }

    private void ToggleDeceleration() {

        if(isDecelerating) {
            isDecelerating = false;
            acceleration = 0.0f;
            if (!isAccelerating) {
                StateDisplayText.text = "Engine OFF";
                SubstateDisplayText.text = "On Cruise";
                setEngineEffect(false);
            }
        } else {
            isDecelerating = true;
            SubstateDisplayText.text = "DECELERATING";
            if (NetworkPlayer.Player.GetComponentInChildren<ShipEngineControl>().FrontHead) {
                acceleration = 0.0f;
                canHandleInput = false;
                StateDisplayText.text = "Adjusting Module";
                NetworkPlayer.Player.GetComponentInChildren<ShipEngineControl>().setFrontHead(false);
                canStartDeceleration = true;
                setEngineEffect(false);
            } else {
                acceleration = -0.5f;
                StateDisplayText.text = "Engine ON";
                setEngineEffect(true);
            }
        }

    }

    private void setEngineEffect(bool flag) {

        var renderers = NetworkPlayer.Player.GetComponentsInChildren<ParticleRenderer>();
        foreach(var renderer in renderers) {
            renderer.enabled = flag;
        }

    }

}
