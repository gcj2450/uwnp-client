[![License](https://img.shields.io/github/license/wsmd/ws-multipath.svg)](https://github.com/wsmd/ws-multipath/blob/master/LICENSE)

# 說明
- [UWNP由來](#UWNP由來)
- [客戶端功能](#客戶端功能)
# UWNP由來
UWNP 全名是 unity+websocket+nodejs+protobuf 輕量級單線程連線框架，目的是讓開發者只專注在開發商業邏輯 API 。
# 客戶端功能
* 斷線重連
* 幾乎零配置，只撰寫 API。
* 客戶端、服務端有四種溝通方法
  * request 客戶端發出請求
  * response 服務端回復請求
  * notify 客戶端通知，服務端不必回復
  * push 服務端主動發送訊息給客戶端
* 服務端從[這裡](https://github.com/IS1103/uwnp-server)點擊.
## 啟動
1. 執行 server
2. 選擇 127.0.0.1，點擊
2. 執行 uwnp-client/UWNP/Semple
![image][link1]

# 與服務端建立連接
```C#
public class TestClient : MonoBehaviour
{
    public async void Start(){

        Client client = new Client("ws://127.0.0.1:3013/=1.0.0", "jon");
        client.OnDisconnect = OnDisconnect;
        client.OnReconect = OnReconect;
        client.OnError = OnError;

        bool isConeccet = await client.ConnectAsync(token);

        if(isConeccet){
            // listen
            client.On("testOn",(Package pack) => {
                TestPush info = MessageProtocol.DecodeInfo<TestPush>(pack.buff);
                Debug.Log(JsonUtility.ToJson(info));
            });

            //request/response
            TestRq testRq = new TestRq();
            Message<TestRp> a = await client.RequestAsync<TestRq, TestRp>("TestController.testA", testRq);
            if (a.err>0)
                Debug.LogWarning("err msg:" + a.errMsg);
            else
                Debug.Log("a:" + a.info.packageType);
            
            //notify mesage
            TestNotify testRq2 = new TestNotify() { name="Lesa" };
            client.Notify("TestController.testB", testRq2);
        }
    }

    private void OnError(uint err,string msg)
    {
        //do something...
    }

    private void OnReconect()
    {
        //do something...
    }

    private void OnDisconnect()
    {
        //do something...
    }
}

```

[link1]:data:image/png;base64,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


