using System.Collections;
using UnityEngine;

namespace WCP
{
    public class ChatPanelExample : MonoBehaviour
    {
        public WChatPanel wcp;

        public bool dynamicTest = false;

        private IEnumerator DebugChat()
        {
            wcp.AddChatAndUpdate(true, "i say hello world", 1);
            yield return new WaitForSeconds(2);
            wcp.AddChatAndUpdate(false, "你好世界", 0);
            yield return new WaitForSeconds(2);
            wcp.AddChatAndUpdate(true, "i say hello world hello world hello world hello world ", 2);
            yield return new WaitForSeconds(2);
            wcp.AddChatAndUpdate(false, "你好世界,你好世界,你好世界,你好世界,你好世界,你好世界,你好世界", 0);
        }

        private void Start()
        {
            StartCoroutine("DebugChat");
        }

        string[] lines = new string[]
        {
        "你好世界,你好世界,你好世界,你好世界,你好世界,你好世界,你好世界",
        "新闻，也叫消息、资讯，是通过报纸、电台、广播、电视台等媒体途径所传播信息的一种称谓。是记录社会、传播信息、反映时代的一种文体。新闻概念有广义与狭义之分，就其广义而言，除了发表于报刊、广播、互联网、电视上的评论与专文外的常用文本都属于新闻之列",
        "央视网(cctv.com)新闻频道是面向全球,多语种,多终端的立体化新闻信息共享平台。以视听与互动为核心,24小时不间断提供最快捷,最权威,最全面,最丰富的新闻视听与互动服务",
        "央以视听与互动为核心,24小时不间断提供最快捷,最权威,最全面,最丰富的新闻视听与互动服务",
        "你好世界,你好世界",
        "你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界你好世界,你好世界",
        "你好世界,你好世界",
        "你好",
        "很高兴认识你",
        "我是韩梅梅",
        "你叫什么名字？"
        };

        public void PerformanceTest(int count)
        {
            for (int i = 0; i < count; i++)
                wcp.AddChat(Random.Range(0.0f, 1.0f) > 0.5f, lines[Random.Range(0, lines.Length)] + i.ToString(), Random.Range(0, wcp.configFile.photoSpriteList.Count));

            wcp.Rebuild();
        }

        private void Update()
        {
            if (dynamicTest)
            {
                wcp.configFile.width = 600 + (int)(Mathf.Sin(Time.time) * 100);
                wcp.configFile.height = 700 + (int)(Mathf.Sin(Time.time) * 100);
                wcp.configFile.scrollBarWidth = 25 + (int)(Mathf.Sin(Time.time) * 10);
                wcp.configFile.photoSize = 100 + (int)(Mathf.Sin(Time.time) * 40);
                RectTransform rectTransform = wcp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(Mathf.Sin(Time.time), Mathf.Sin(Time.time)) * 100;
            }
        }
    }
}