using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxChatFromClient : MonoBehaviour
{
    ManoChat manoChat;
    public Text usernameFromClient;
    public Text messageFromClient;

    // Start is called before the first frame update
    void Start()
    {
        manoChat = GameObject.FindGameObjectWithTag("ProgramManeger").GetComponent<ManoChat>();
        messageFromClient.text = manoChat.message;
        usernameFromClient.text = manoChat.yourUsername.text;
        manoChat.yourMessage.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
