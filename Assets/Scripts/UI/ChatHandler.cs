using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatHandler : MonoBehaviour {

    public InputField ChatInput;
    public Scrollbar Scrollbar;
    public Text ChatWindow;
    
    private List<string> msgBuffer;

    private void Awake() {

        msgBuffer = new List<string>();

    }

    public void SendMessageToServer(string msg) {

        if (msg.Length > 0) {
            
            string jsonData = @"{""id"":""" + SocketManager.SocketID 
                + @""",""type"":""c"",""payload"":{""msg"":""" + msg 
                + @"""}}";
            SocketManager.Socket.Emit("Task", jsonData);

            // empty the input box
            ChatInput.text = "";
            ChatInput.ActivateInputField();
        }

    }

    public void AddMessageToChatBox(string msg) {

        msgBuffer.Add(msg);

    }

    private void Update() {

        if(msgBuffer.Count > 0) {
            foreach(var text in msgBuffer) {
                ChatWindow.text += text + '\n';
                Scrollbar.value = 0.0f;
            }
            msgBuffer.Clear();
        }

    }

}
