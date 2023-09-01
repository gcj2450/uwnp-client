using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//客户端服务端四种沟通方式
//request 客户端发出请求
//response 服务端回复请求
//notify 客户端通知，服务端不必回复
//push 服务端主动发送信息给客户端

namespace UWNP{
    public class TestClient : MonoBehaviour
    {
        public ToggleGroup toggleGroup;
        public InputField ip, port;

        public Button connectBtn;

        public string token, version;
        private string host;
        Client client;
        public Image img;

        [Obsolete]
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            connectBtn.onClick.AddListener(connectBtnClick);
        }

        public void CreateConeccetionBtn() {

            string host = string.Format("ws://{0}:{1}/={2}", this.host, port.text, version);

            client = new Client(host);
            client.OnDisconnect = OnDisconnect;
            client.OnReconected = OnReconected;
            client.OnError = OnError;
            client.OnConnected = OnConnected;
            CreateConeccetion().Forget();
        }

        private void OnConnected()
        {
            Debug.Log("OnConnected");
        }

        private void OnError(string msg)
        {
            Debug.LogError(string.Format("err msg:{0}",msg));
        }

        private void OnReconected()
        {
            Debug.Log("OnReconect");
            img.gameObject.SetActive(true);
        }

        private void OnDisconnect()
        {
            Debug.Log("OnDisconnect");
            img.gameObject.SetActive(false);
        }

        public void connectBtnClick() {

            if (client != null) client.Cancel();

            IEnumerable<Toggle> toggles = toggleGroup.ActiveToggles();

            foreach (Toggle toggle in toggles)
            {
                if (toggle.isOn)
                {
                    host = toggle.GetComponentInChildren<Text>().text;
                    CreateConeccetionBtn();
                    return;
                }
            }
        }

        private async UniTaskVoid CreateConeccetion()
        {
            Debug.Log("BeginConnect..."+ host);

            int count = 3;
            bool isConeccet = false;
            while (count-->0 && !isConeccet)
            {
                Debug.Log(host);
                isConeccet = await client.ConnectAsync("jon");
            }
            
            if (isConeccet)
            {
                img.gameObject.SetActive(true);
                
                // On
                client.On("testOn",(Package pack) => {
                    TestPush info = MessageProtocol.DecodeInfo<TestPush>(pack.buff);
                    Debug.Log(JsonUtility.ToJson(info));
                    //img.gameObject.SetActive(false);
                });

                //请求/响应
                TestRq testRq = new TestRq();
                testRq.packageType = 111;
                Message<TestRp> a = await client.RequestAsync<TestRq, TestRp>("TestController.testA", testRq);
                if (a.err>0)
                {
                    Debug.LogWarning("err:" + a.err);
                    Debug.LogWarning("err msg:" + a.errMsg);
                }
                else
                {
                    Debug.Log("a:" + a.info.packageType);
                }

                //请求/响应
                testRq.packageType = 1985;
                Message<TestRp2> a3 = await client.RequestAsync<TestRq, TestRp2>("TestController.testC", testRq, "custom1");
                if (a3.err > 0)
                {
                    Debug.LogWarning("err:" + a3.err);
                    Debug.LogWarning("err msg:" + a3.errMsg);
                }
                else
                {
                    Debug.Log("a:" + a3.info.info);
                }

                //通知
                TestNotify testRq2 = new TestNotify() { name = "小叮当" };
                client.Notify("TestController.testB", testRq2);
                //*/
            }
            else
                Debug.Log("Connect failed more than three times");
        }

        public async void SendAPI()
        {
            //请求/响应
            TestRq testRq = new TestRq();
            Message<TestRp> a = await client.RequestAsync<TestRq, TestRp>("TestController.testA", testRq);
            if (a.err > 0)
            {
                Debug.LogWarning("err:" + a.err);
                Debug.LogWarning("err msg:" + a.errMsg);
            }
            else
            {
                Debug.Log("a:" + a.info.packageType);
            }
        }

        private void OnDestroy()
        {
            if (client!=null)
            {
                client.Cancel(true);
            }
        }
    }
}

