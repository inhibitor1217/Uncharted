using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;

public class SocketManager : MonoBehaviour {

    private const string url = "http://127.0.0.1:2000/";

    [HideInInspector]
    public static Client Socket { get; private set; }
    [HideInInspector]
    public static string SocketID;

    public ChatHandler Console;

    private void Awake() {

        Socket = new Client(url);

        Socket.Opened += SocketOpened;

        Socket.On("Configure", (data) => {

            ConfigureSchema parsedData = JsonUtility.FromJson<ConfigureSchema>(data.Json.args[0].ToString());

            SocketID = parsedData.id;

            Console.AddMessageToChatBox("SERVER: Connected to Chat Server.");
            Console.AddMessageToChatBox("SERVER: Configured identification: " + SocketID);

        });

        Socket.On("Update", (data) => {
            
            DataSchema parsedData = JsonUtility.FromJson<DataSchema>(data.Json.args[0].ToString());

            /* Process Chat Data */
            foreach(ChatDataSchema chat in parsedData.chat) {
                if (chat.id == SocketID)
                    Console.AddMessageToChatBox("<color=#00ffffff>[" + chat.id + "]: " + chat.msg + "</color>");
                else
                    Console.AddMessageToChatBox("[" + chat.id + "]: " + chat.msg);
            }
            /* Process Chat Data End */

        });

        Socket.Connect();

    }

    private void SocketOpened(object sender, System.EventArgs e) {
       //  Debug.Log("Client: CONNECTED TO SERVER");
    }

    private void OnDisable() {
        Socket.Close();
    }

    [System.Serializable]
    class ConfigureSchema {
        public string id;
    }

    [System.Serializable]
    class ChatDataSchema {
        public string id, msg;
    }

    [System.Serializable]
    class DataSchema {
        public ChatDataSchema[] chat;
    }

}
