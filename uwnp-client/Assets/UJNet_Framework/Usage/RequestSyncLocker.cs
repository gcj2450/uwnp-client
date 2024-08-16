using UnityEngine;
using System;
using System.Collections;
using System.Net;

public class RequestSyncLocker : MonoBehaviour
{

    private static RequestSyncLocker mInstance;

    public static RequestSyncLocker Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(RequestSyncLocker)) as RequestSyncLocker;
            }
            return mInstance;
        }
    }

    public int llk = 1000;
    public bool locked = false;
    public int cmd;
    public DateTime startTime;

    void Awake()
    {
        mInstance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        UJNetManager.Instance.Client.AddHttpCloseListener(OnHttpEnd);
        UJNetManager.Instance.Client.AddHttpErrorListener(OnHttpError);
    }

    public int locker
    {
        get { return ++llk; }
    }

    public void FireSync(int cmd)
    {
        locked = true;
        this.cmd = cmd;
        startTime = DateTime.Now;

        WaitPanelCtrl.Instance.ShowPanel();//==============�Զ����޸�=====================
    }

    public void FireSync(int cmd, bool wait)
    {
        locked = true;
        this.cmd = cmd;
        startTime = DateTime.Now;

        if (wait)   //==============�Զ����޸�=====================
            WaitPanelCtrl.Instance.ShowPanel();//==============�Զ����޸�=====================
    }

    public void OnHttpEnd(int lk)
    {
        if (lk != this.llk)
            return;

        locked = false;
        //WaitPanelCtrl.Instance.HidePanel ();//==============�Զ����޸�=====================

        Debug.Log(DateTime.Now + "//REQ ELAPSED: " + cmd + " : " + (DateTime.Now - startTime).TotalSeconds + " s");
    }

    void OnHttpError(Exception e, int lk)
    {
        if (lk != this.llk)
            return;

        //if (e is WebException)
        //{
        //    //==============�Զ����޸�=====================
        //    WebException we = (WebException)e;
        //    if (we.Status == WebExceptionStatus.Timeout)
        //    {
        //        TipBoxCtrl.Instance.Show(I18NManager.Instance.GetProp("net.syn.timeout"));
        //    }
        //    else if (we.Status == WebExceptionStatus.ConnectFailure || we.Status == WebExceptionStatus.NameResolutionFailure || we.Status == WebExceptionStatus.ProtocolError)
        //    {
        //        Debug.LogWarning("web exception " + we.Message);
        //        TipBoxCtrl.Instance.Show(I18NManager.Instance.GetProp("net.syn.connectFailure"));
        //    }
        //    else
        //    {
        //        Debug.LogWarning("web exception " + we.Message);
        //        TipBoxCtrl.Instance.Show(I18NManager.Instance.GetProp("net.syn.webExecption"));
        //    }
        //    //==============�Զ����޸�=====================
        //    return;
        //}

        //TipBoxCtrl.Instance.Show(e.Message);//==============�Զ����޸�=====================
    }

}
