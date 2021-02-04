using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ManoChat : MonoBehaviour
{
    [Header("WebControl")]
    private WebSocket webSocket;
    public InputField yourIP;
    public InputField yourPort;
    public InputField yourMessage;
    public InputField yourUsername;
    public GameObject textBoxFromClient;
    public GameObject textBoxFromServer;
    public Transform posTextBox;
    public TextBoxChatFromServer fromServer;
    public List<string> usernameList;

    [Header("InterfaceManoChat")]
    public GameObject loginPage;
    public GameObject chatPage;
    bool checkUsername;

    [Header("TextToString")]
    private string ip;
    private string port;
    public string message;
    public string messageServer;
    public string username;
    public List<string> messageHistory;
    public string[] _messagesUser;

    // Start is called before the first frame update
    void Start()
    {
        loginPage.SetActive(true);
        chatPage.SetActive(false);

        fromServer = GameObject.FindGameObjectWithTag("ChatServer").GetComponent<TextBoxChatFromServer>();
    }       

    // Update is called once per frame
    void Update()
    {
        ip = yourIP.text;
        port = yourPort.text;
        username = yourUsername.text;
    }

    public void SignInToManoChat()
    {
        webSocket = new WebSocket("ws://" + ip + ":" + port + "/");

        loginPage.SetActive(false);
        chatPage.SetActive(true);
        webSocket.Connect();
        username = yourUsername.text;
        webSocket.OnMessage += OnMessage;
        checkUsername = true;

        if (port != "8080" && yourIP.text != "127.0.0.1" && yourUsername.text == null)
        {
            loginPage.SetActive(true);
        }
    }

    public void RememberInfo()
    {
        yourIP.text = null;
        yourPort.text = null;
        yourUsername.text = null;
        print("Delete");
    }

    public void SendMessage()
    {
        if (webSocket.ReadyState == WebSocketState.Open)
        {
            message = yourMessage.text;
            webSocket.Send(username + "@" + message);
            yourMessage.text = null;
            var textboxServer = GetComponent<TextBoxChatFromServer>();
            messageHistory.Add(username + "@" + textboxServer.usernameFromServer.text);
        }
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {        
        _messagesUser = messageEventArgs.Data.Split('@');
        message = _messagesUser[1];

        usernameList.Add(username);

        Debug.Log(_messagesUser[0]);
        if (username != _messagesUser[0])
        {
            for (int i = 0; i < messageHistory.Count; i++)
            {
                messageHistory[i] = message;
            }

            var messageFromServer = true;
            if (messageFromServer)
            {
                var newTextBox = Instantiate(textBoxFromServer, posTextBox.position, Quaternion.identity);
                newTextBox.transform.parent = posTextBox;

                messageFromServer = false;
            }
        }
        else
        {
            var newTextBox = Instantiate(textBoxFromClient, posTextBox.position, Quaternion.identity);
            newTextBox.transform.parent = posTextBox;
        }
    }

    public void LogOutManoChat()
    {
        webSocket.Close();
        loginPage.SetActive(true);
        chatPage.SetActive(false);
    }

    public void OnDestroy()
    {
        if (webSocket != null)
        {
            webSocket.Close();
        }
    }
}