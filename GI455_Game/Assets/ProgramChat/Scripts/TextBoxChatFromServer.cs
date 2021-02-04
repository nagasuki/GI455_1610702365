using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxChatFromServer : MonoBehaviour
{
    ManoChat manoChat;
    public Text usernameFromServer;
    public Text messageFromServer;

    // Start is called before the first frame update
    void Start()
    {
        manoChat = GameObject.FindGameObjectWithTag("ProgramManeger").GetComponent<ManoChat>();
        messageFromServer.text = manoChat.message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
