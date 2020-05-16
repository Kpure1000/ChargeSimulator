using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(RectTransform))]
public class ScrollMsgController : MonoBehaviour
{
    public enum PartType
    {
        Company,
        Government
    }

    public PartType partType = PartType.Company;

    public Vector2 startPosition;

    public Vector2 endPosition;

    public float closeSpeed;

    RectTransform rt;

    Text _msgText;

    Vector2 curDirection
    {
        get
        {
            return endPosition - startPosition;
        }
    }

    /// <summary>
    /// 靠近终点距离判定
    /// </summary>
    public float closeDistance;

    /// <summary>
    /// 消息队列
    /// </summary>
    Queue<string> _scrollMsgs = new Queue<string>();

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        _msgText = GetComponent<Text>();
    }

    private void Update()
    {
        if (_scrollMsgs.Count == 0) return;
        //到达
        if(Vector3.Distance(rt.anchoredPosition, endPosition) < closeDistance)
        {
            _scrollMsgs.Dequeue();
            rt.anchoredPosition = startPosition;
        }
        else
        {
            _msgText.text = _scrollMsgs.Peek();

            rt.anchoredPosition += curDirection.normalized * closeSpeed * Time.unscaledDeltaTime;

        }

    }

    public void SandMsg(UserInfo.chargeDeltaRecord record)
    {
        Debug.Log("发送消息！");

        string msgString = "";
        switch (partType)
        {
            case PartType.Company:
                if (record.deltaPower < 0)
                {
                    Debug.Log("获取消息：充电！");
                    msgString =
                    string.Format("{0}:{1}: U0001 给 {2}-充电:{3}-奖励:{4}",
                    record.endTime.x,
                    record.endTime.y,
                    record.targetBar,
                    record.deltaPower,
                    record.rewards);
                }
                //被充电的记录
                else if (record.deltaPower > 0)
                {
                    Debug.Log("获取消息：被充电！");
                    msgString =
                    string.Format("{0}:{1}: {2} 给 U0001-充电:{3}-实际支付:{4}",
                    record.endTime.x,
                    record.endTime.y,
                    record.targetBar,
                    record.deltaPower,
                    record.realPay);
                }
                //加入队列
                _scrollMsgs.Enqueue(msgString);

                break;
            case PartType.Government:
                if (record.deltaPower < 0)
                {
                    msgString = string.Format("{0}:{1}: U0001 给 {2} 充电.", record.endTime.x, record.endTime.y, record.targetBar);
                }
                //被充电的记录
                else if (record.deltaPower > 0)
                {
                    msgString = string.Format("{0}:{1}: U0001 从 {2} 获取电量.", record.endTime.x, record.endTime.y, record.targetBar);
                    if (record.realPay == 0)
                        msgString += " 未付款，失信.";
                }
                //加入队列
                _scrollMsgs.Enqueue(msgString);
                break;
            default:
                break;
        }
    }

}
