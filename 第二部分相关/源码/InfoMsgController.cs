using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMsgController : MonoBehaviour
{
    Text msgText;

    Dictionary<string, UserInfo> userinfoDic = new Dictionary<string, UserInfo>();

    private void Awake()
    {
        msgText = GetComponent<Text>();
        for (int i = 0; i < 40; i++)
        {
            userinfoDic[string.Format("User0 ({0})", i)] = null;
        }
    }

    private void Update()
    {
        string msgString = "";
        int i = 0;
        foreach (var item in userinfoDic)
        {
            if (item.Value)
            {
                msgString += string.Format("User:ID:{0}, Power:{1}({2:P}), State:{3}\r\n",
                   i,
                   item.Value.curPower,
                   (float)item.Value.curPower / UserInfo.maxPower,
                   item.Value.userController._curState.statettype.ToString());
            }
            if ((i + 1) % 3 == 0) msgString += "\r\n";
            i++;
        }
        msgText.text = msgString;
    }

    public void SandMsg(UserInfo userinfo)
    {
        if (userinfoDic.ContainsKey(userinfo.gameObject.name))
        {
            userinfoDic[userinfo.gameObject.name] = userinfo;
        }
    }
}
