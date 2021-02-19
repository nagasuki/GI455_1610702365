using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ManoChat : MonoBehaviour
{
    [Header("UserBar")]
    public InputField usernameInputField;

    [Header("RoomManage")]
    public GameObject mainRoom;
    public GameObject lobbyRoom;
    public GameObject chatRoom;
    public List<string> roomList;

    [Header("message")]
    public InputField messageInputField;

    public struct SocketEvent
    {
        public string eventName;
        public string data;

        public SocketEvent(string eventName, string data)
        {
            this.eventName = eventName;
            this.data = data;
        }
    }

    private WebSocket ws;

    private string tempMessageString;

    public delegate void DelegateHandle(SocketEvent result);
    public DelegateHandle OnCreateRoom;
    public DelegateHandle OnJoinRoom;
    public DelegateHandle OnLeaveRoom;

    private void Start()
    {
        mainRoom.SetActive(true);
        lobbyRoom.SetActive(false);
        chatRoom.SetActive(false);
    }

    private void Update()
    {
        UpdateNotifyMessage();
    }

    public void Connect()
    {
        string url = "ws://127.0.0.1:8080/";

        ws = new WebSocket(url);

        ws.OnMessage += OnMessage;

        ws.Connect();

        lobbyRoom.SetActive(true);
        mainRoom.SetActive(false);
    }

    public void CreateRoom(string roomName)
    {
        SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

        string toJsonStr = JsonUtility.ToJson(socketEvent);

        ws.Send(toJsonStr);

        chatRoom.SetActive(true);
        lobbyRoom.SetActive(false);
    }

    public void LeaveRoom()
    {
        SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

        string toJsonStr = JsonUtility.ToJson(socketEvent);

        ws.Send(toJsonStr);
    }

    public void Disconnect()
    {
        if (ws != null)
            ws.Close();

        mainRoom.SetActive(true);
        lobbyRoom.SetActive(false);
        chatRoom.SetActive(false);
    }

    public void _SendMessage(string message)
    {

    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void UpdateNotifyMessage()
    {
        if (string.IsNullOrEmpty(tempMessageString) == false)
        {
            SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

            if (receiveMessageData.eventName == "CreateRoom")
            {
                if (OnCreateRoom != null)
                    OnCreateRoom(receiveMessageData);
            }
            else if (receiveMessageData.eventName == "JoinRoom")
            {
                if (OnJoinRoom != null)
                    OnJoinRoom(receiveMessageData);
            }
            else if (receiveMessageData.eventName == "LeaveRoom")
            {
                if (OnLeaveRoom != null)
                    OnLeaveRoom(receiveMessageData);
            }

            tempMessageString = "";
        }
    }

    private void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        Debug.Log(messageEventArgs.Data);

        tempMessageString = messageEventArgs.Data;
    }
}