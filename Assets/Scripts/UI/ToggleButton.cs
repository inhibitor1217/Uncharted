using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {

    public Sprite SpriteImageOn;
    public Sprite SpriteImageOff;

    private bool myFlag;

    public void Toggle(bool newFlag) {
        if(newFlag != myFlag) {
            if(newFlag) {
                myFlag = true;
                GetComponent<Image>().sprite = SpriteImageOn;
            }
            else {
                myFlag = false;
                GetComponent<Image>().sprite = SpriteImageOff;
            }
        }
    }

}
