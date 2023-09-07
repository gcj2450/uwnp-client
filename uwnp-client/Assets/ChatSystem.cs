using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatSystem : MonoBehaviour
{

    /// <summary>
    /// 发送或打开聊天
    /// </summary>
    bool typing = false;
    /// <summary>
    /// 输入框
    /// </summary>
    [SerializeField] InputField speachPlace;
    /// <summary>
    /// 文本显示
    /// </summary>
    [SerializeField] Text conversation; 

    public static ChatSystem instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return) &&
        //PlayerManager.instance.LocalPlayer.GetComponent<PlayerController>().IslocalPlayer)
        //{
        //    Debug.Log("Enter");
        //    if (typing)
        //    {
        //        //Envia msg ao servidor
        //        SendMsg();
        //        typing = false;
        //    }
        //    else
        //    {
        //        //abre chat
        //        speachPlace.gameObject.SetActive(true);
        //        speachPlace.Select();
        //        typing = true;
        //    }
        //}
    }

    //Enviar msg servidor
    void SendMsg()
    {
        //speachPlace.gameObject.SetActive(false);

        //if (PlayerManager.instance.GetPlayerController().IslocalPlayer)
        //{
        //    string msg = speachPlace.text;
        //    //Avisa ao connectionManager
        //    ConnectionManager.instance.EmitMsg(msg);
        //}
    }

    public void MsgReceived(string newMsg, string playerName)
    {
        ////Seta mensagem na janela de chat
        //string msg = playerName + ": " + newMsg + "\n";
        //conversation.text += msg;
    }

}
