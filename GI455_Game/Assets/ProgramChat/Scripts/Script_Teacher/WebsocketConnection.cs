using LobbyRoomExample;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace LobbyRoomExample
{
    public class WebsocketConnection : MonoBehaviour
    {
        [Header("InputField")]
        public InputField usernameInputField;
        public InputField roomNameInputField;
        public InputField joinRoomInputField;

        [Header("RoomManage")]
        public GameObject mainRoom;
        public GameObject lobbyRoom;
        public GameObject chatRoom;
        public GameObject roomNameInput;
        public GameObject joinRoomInput;
        public GameObject activeButtonRoomName;
        public GameObject activeButtonJoinRoom;
        [SerializeField] private List<string> roomList = new List<string>();
        bool isCreate = false;
        bool isJoin = false;
        bool isLeave = false;

        [Header("message")]
        public InputField messageInputField;
        public GameObject showPopUp;
        public Text messagesPopUp;
        public Text showUser;
        public Text showRoomName;

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
        private string messageRoomName;

        private void Start()
        {
            mainRoom.SetActive(true);
            lobbyRoom.SetActive(false);
            chatRoom.SetActive(false);
            showPopUp.SetActive(false);
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

            showUser.text = usernameInputField.text;

            lobbyRoom.SetActive(true);
            mainRoom.SetActive(false);
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            messageRoomName = messageEventArgs.Data;
            tempMessageString = messageEventArgs.Data;
        }

        public void CreateRoomButton()
        {
            roomNameInput.SetActive(true);
            activeButtonJoinRoom.SetActive(false);
        }

        public void CreateRoom(string roomName)
        {
            messageRoomName = roomNameInputField.text;

            roomName = messageRoomName;

            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

            showRoomName.text = roomName;

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            roomList.Add(messageRoomName);

            roomNameInputField.text = null;
            joinRoomInputField.text = null;            
        }

        public void JoinRoomButton()
        {
            joinRoomInput.SetActive(true);
            activeButtonRoomName.SetActive(false);
        }

        public void JoinRoom(string roomName)
        {
            messageRoomName = joinRoomInputField.text;

            roomName = messageRoomName;

            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

            showRoomName.text = roomName;

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            roomNameInputField.text = null;
            joinRoomInputField.text = null;            
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            roomList.Remove(messageRoomName);

            roomNameInputField.text = null;
            joinRoomInputField.text = null;

            roomNameInput.SetActive(false);
            joinRoomInput.SetActive(false);
            chatRoom.SetActive(false);
            lobbyRoom.SetActive(true);
            activeButtonJoinRoom.SetActive(true);
            activeButtonRoomName.SetActive(true);
            joinRoomInput.SetActive(false);
            roomNameInput.SetActive(false);
            showPopUp.SetActive(false);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();

            roomNameInputField.text = null;
            joinRoomInputField.text = null;
            usernameInputField.text = null;

            mainRoom.SetActive(true);
            lobbyRoom.SetActive(false);
            chatRoom.SetActive(false);
            activeButtonJoinRoom.SetActive(true);
            activeButtonRoomName.SetActive(true);
            joinRoomInput.SetActive(false);
            roomNameInput.SetActive(false);
            showPopUp.SetActive(false);
        }

        public void OKButton()
        {
            showPopUp.SetActive(false);
            joinRoomInput.SetActive(false);
            roomNameInput.SetActive(false);
            activeButtonJoinRoom.SetActive(true);
            activeButtonRoomName.SetActive(true);
        }

        public void _SendMessage(string message)
        {

        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(messageRoomName) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(messageRoomName);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                    if(receiveMessageData.data != "fail")
                    {
                        showPopUp.SetActive(false);
                        chatRoom.SetActive(true);
                        lobbyRoom.SetActive(false);
                    }
                    else
                    {
                        showPopUp.SetActive(true);
                        messagesPopUp.text = "[ Create Room Fail ]";
                    }
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                    if (receiveMessageData.data != "fail")
                    {
                        chatRoom.SetActive(true);
                        lobbyRoom.SetActive(false);
                        showPopUp.SetActive(false);
                    }
                    else
                    {
                        showPopUp.SetActive(true);
                        messagesPopUp.text = "[ Join Room Fail ]";
                    }
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }

                messageRoomName = "";
            }
        }        
    }
}
