using LobbyRoomExample;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace LobbyRoomExample
{
    public class ManoChat : MonoBehaviour
    {
        [Header("InputField")]
        public InputField roomNameInputField;
        public InputField joinRoomInputField;
        public InputField idInputField;
        public InputField idInputFieldSingUpPage;
        public InputField passwordInputField;
        public InputField passwordInputFieldSingUpPage;
        public InputField rePasswordSignUpPage;
        public InputField usernameInputFieldSignUpPage;
        public InputField inputText;

        [Header("RoomManage")]
        public GameObject mainRoom;
        public GameObject lobbyRoom;
        public GameObject chatRoom;
        public GameObject loginPage;
        public GameObject registerPage;
        public GameObject roomNameInput;
        public GameObject joinRoomInput;
        public GameObject activeButtonRoomName;
        public GameObject activeButtonJoinRoom;
        public GameObject connectButton;
         
        [Header("message")]
        public GameObject showPopUp;
        public GameObject showLoginPopUp;
        public Text sendText;
        public Text receiveText;
        public Text messagesPopUp;
        public Text loginMessagePopUp;
        public Text showUser;
        public Text showRoomName;

        [System.Serializable]
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

        [System.Serializable]
        public class HeadData
        {
            public string eventName;
        }

        [System.Serializable]
        public class TempData : HeadData
        {
            public string data;
        }

        [System.Serializable]
        public class MessageData : HeadData
        {
            public string Username;
            public string Message;
        }

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(string result);
        public event DelegateHandle OnCreateRoom;
        public event DelegateHandle OnJoinRoom;
        public event DelegateHandle OnLeaveRoom;
        public event DelegateHandle OnLogin;
        public event DelegateHandle OnRegister;
        public event DelegateHandle OnShowUsername;
        public event DelegateHandle _OnMessage;
        private string messageRoomName;
        private string Username;

        private void Start()
        {
            mainRoom.SetActive(false);
            lobbyRoom.SetActive(false);
            chatRoom.SetActive(false);
            showPopUp.SetActive(false);
        }

        private void Update()
        {
            UpdateNotifyMessage();

            //if(idInputFieldSingUpPage.text == )
            //{

            //}
        }

        public void Connect()
        {
            mainRoom.SetActive(true);

            string url = "ws://25.97.148.109:4624/";

            ws = new WebSocket(url);

            ws.Connect();

            connectButton.SetActive(false);
        }

        public void LoginButton(string LoginData)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                if (idInputField.text == "" || passwordInputField.text == "")
                {
                    showLoginPopUp.SetActive(true);
                    loginMessagePopUp.text = "[ ID or Password is Empty ]";
                }
                else
                {
                    messageRoomName = idInputField.text + ":" + passwordInputField.text;

                    LoginData = messageRoomName;

                    SocketEvent socketEvent = new SocketEvent("Login", LoginData);

                    string toJsonStr = JsonUtility.ToJson(socketEvent);

                    ws.Send(toJsonStr);

                    ws.OnMessage += OnMessage;
                }
            }
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
            usernameInputFieldSignUpPage.text = null;

            mainRoom.SetActive(true);
            loginPage.SetActive(true);
            lobbyRoom.SetActive(false);
            chatRoom.SetActive(false);
            activeButtonJoinRoom.SetActive(true);
            activeButtonRoomName.SetActive(true);
            joinRoomInput.SetActive(false);
            roomNameInput.SetActive(false);
            showPopUp.SetActive(false);
        }

        public void SignUpButton()
        {
            registerPage.SetActive(true);
            loginPage.SetActive(false);
            ws.OnMessage += OnMessage;
        }

        public void SignUpButtonActive(string registerData)
        {
            if (usernameInputFieldSignUpPage.text == "" || idInputFieldSingUpPage.text == "" || passwordInputFieldSingUpPage.text == "")
            {
                showLoginPopUp.SetActive(true);
                loginMessagePopUp.text = "[ Register is Empty ]";
            }
            else
            {
                messageRoomName = idInputFieldSingUpPage.text + ":" + passwordInputFieldSingUpPage.text + ":" + usernameInputFieldSignUpPage.text;

                if (rePasswordSignUpPage.text == passwordInputFieldSingUpPage.text)
                {
                    registerData = messageRoomName;

                    SocketEvent socketEvent = new SocketEvent("Register", registerData);

                    string toJsonStr = JsonUtility.ToJson(socketEvent);

                    ws.Send(toJsonStr);
                }
                else
                {
                    showLoginPopUp.SetActive(true);
                    loginMessagePopUp.text = "[ Password Not Match ]";
                }
            }
        }

        public void OKButton()
        {
            showPopUp.SetActive(false);
            showLoginPopUp.SetActive(false);
            joinRoomInput.SetActive(false);
            roomNameInput.SetActive(false);
            activeButtonJoinRoom.SetActive(true);
            activeButtonRoomName.SetActive(true);
        }

        public void SendMessageButton()
        {
            MessageData newMessageData = new MessageData();
            newMessageData.Username = Username;
            newMessageData.Message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(newMessageData);
            var socketEvent = new SocketEvent("SendMessage", toJsonStr);
            var fromJsonString = JsonUtility.ToJson(socketEvent);

            ws.Send(fromJsonString);

            inputText.text = "";
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
                var receiveMessageData = JsonUtility.FromJson<SocketEvent>(messageRoomName);

                switch (receiveMessageData.eventName)
                {
                    case "CreateRoom":
                        {
                            if (OnCreateRoom != null)
                                OnCreateRoom(receiveMessageData.data);
                            if (receiveMessageData.data != "fail")
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
                            break;
                        }
                    case "JoinRoom":
                        {
                            if (OnJoinRoom != null)
                                OnJoinRoom(receiveMessageData.data);
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
                            break;
                        }
                    case "LeaveRoom":
                        {
                            if (OnLeaveRoom != null)
                                OnLeaveRoom(receiveMessageData.data);
                            sendText.text = "";
                            receiveText.text = "";
                            break;
                        }
                    case "Login":
                        {
                            var receiveUsernameData = JsonUtility.FromJson<MessageData>(messageRoomName);
                            if (OnLogin != null)
                                OnLogin(receiveMessageData.data);
                            if (receiveMessageData.data != "fail")
                            {
                                lobbyRoom.SetActive(true);
                                mainRoom.SetActive(false);
                                loginPage.SetActive(false);
                                Username = receiveUsernameData.Username;
                                showUser.text = Username;
                            }
                            else
                            {
                                showLoginPopUp.SetActive(true);
                                loginMessagePopUp.text = "[ Login Fail ]";
                            }
                            break;
                        }
                    case "Register":
                        {
                            if (OnRegister != null)
                                OnRegister(receiveMessageData.data);
                            if(receiveMessageData.data != "fail")
                            {
                                loginPage.SetActive(true);
                                registerPage.SetActive(false);
                            }
                            else
                            {
                                showLoginPopUp.SetActive(true);
                                loginMessagePopUp.text = "[ Register Fail ]";
                            }
                            break;
                        }
                    case "SendMessage":
                        {
                            var receiveUsernameData = JsonUtility.FromJson<MessageData>(receiveMessageData.data);
                            if(receiveUsernameData.Username == Username)
                            {
                                sendText.text += receiveUsernameData.Username + " : " + receiveUsernameData.Message + "\n";
                                receiveText.text += "\n";
                            }
                            else
                            {
                                sendText.text += "\n";
                                receiveText.text += receiveUsernameData.Username + " : " + receiveUsernameData.Message + "\n";
                            }
                            break;
                        }
                }

                messageRoomName = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            messageEventArgs.Data.Split(':');
            messageRoomName = messageEventArgs.Data;

            //Username = messageEventArgs.Data;
            

            Debug.Log("OnMessage : " + messageEventArgs.Data);
            //tempMessageString = messageEventArgs.Data;
            //Debug.Log(tempMessageString);
        }
    }
}
