using System.Collections.Generic;
using UnityEngine;

namespace TMoonEventSystem
{

    /// <summary>
    /// 用来给监听信息者收到信息后执行的处理。
    /// </summary>
    /// <param name="rMessage"></param>
    public delegate void MessageHandler(IMessage rMessage);


    public class MessageDispatcher
    {
        /// <summary>
        /// 创建MessageDispatcher在Unity的存根，专门处理延迟信息的发送
        /// </summary>
        private static MessageDispatcherStub sStub = (new GameObject("MessageDispatcherStub")).AddComponent<MessageDispatcherStub>();

        /// <summary>
        /// 当某些信息没有监听的时候，信息发送的处理由该委托处理。
        /// </summary>
        public static MessageHandler MessageNotHandled = null;

        /// <summary>
        /// 存储标识着延迟发送的信息
        /// </summary>
        private static List<IMessage> mMessages = new List<IMessage>();

        /// <summary>
        /// 主要存储信息对应的处理
        /// 第一个string是信息的标识，第二个string是监听过滤信息的标识，第三个是信息发送成功的处理
        /// </summary>
        private static Dictionary<string, Dictionary<string, MessageHandler>> mMessageHandlers = new Dictionary<string, Dictionary<string, MessageHandler>>();

        /// <summary>
        /// 清除所有延迟信息的列表
        /// </summary>
        public static void ClearMessages()
        {
            mMessages.Clear();
            mMessageHandlers.Clear();
        }

        /// <summary>
        /// 添加一个对信息的监听
        /// </summary>
        /// <param name="rMessageType">所监听信息的标识</param>
        /// <param name="rFilter">所监听信息过滤的标识</param>
        /// <param name="rHandler">所监听信息后的处理，即信息由派送者发出去后的处理</param>
        public static void AddListener(string rMessageType, MessageHandler rHandler, string rFilter = "")
        {
            Dictionary<string, MessageHandler> lRecipientDictionary = null;

            //查找信息对应处理的列表里是否包含了这个信息，若包含取出他的对应的过滤和处理。
            if (mMessageHandlers.ContainsKey(rMessageType))
            {
                lRecipientDictionary = mMessageHandlers[rMessageType];
            }
            //如果没有包含这个信息处理的标识则创建他
            else
            {
                lRecipientDictionary = new Dictionary<string, MessageHandler>();
                mMessageHandlers.Add(rMessageType, lRecipientDictionary);
            }

            //检查信息过滤集合是否包含了过滤信息的标识,""代表没有过滤标识
            if (!lRecipientDictionary.ContainsKey(rFilter))
            {
                lRecipientDictionary.Add(rFilter, null);
            }

            //因为是引用，所以会自动更新到mMessageHandlers里。
            lRecipientDictionary[rFilter] += rHandler;
        }

        /// <summary>
        /// 发送信息出去
        /// </summary>
        /// <param name="rMessage"></param>
        public static void SendMessage(IMessage rMessage)
        {
            //是否丢失接收信息的监听者，默认为true
            bool lReportMissingRecipient = true;

            //如果信息有延迟时间则把该信息加入到延迟信息列表中等待触发。
            if (rMessage.Delay > 0)
            {
                if (!mMessages.Contains(rMessage))
                {
                    mMessages.Add(rMessage);
                }

                //避免触发下面的丢失监听者代码块
                lReportMissingRecipient = false;
            }
            //没有延迟的话直接看看信息相应处理列表是否包含这条信息
            else if (mMessageHandlers.ContainsKey(rMessage.Type))
            {
                //得到相应信息的处理
                Dictionary<string, MessageHandler> lHandlers = mMessageHandlers[rMessage.Type];

                //遍历相信信息的过滤的标识
                foreach (string lFilter in lHandlers.Keys)
                {
                    if (lHandlers[lFilter] == null)
                    {
                        continue;
                    }

                    if (lFilter.Equals(rMessage.Filter))
                    {
                        lHandlers[lFilter](rMessage);
                        rMessage.IsSent = true;
                        lReportMissingRecipient = false;
                    }
                }
            }

            if (lReportMissingRecipient)
            {
                if (MessageNotHandled == null)
                {
                    //Debug.LogWarning(string.Format("MessageDispatcher无法识别Message.Type：{0} 或者是信息过滤指定的Message.Filter", rMessage.Type, rMessage.Filter));
                }
                else
                {
                    MessageNotHandled(rMessage);
                }
            }
        }

        /// <summary>
        /// 在每帧中负责处理延迟信息列表的发送
        /// </summary>
        public static void Update()
        {
            //处理延迟信息列表的信息发送
            for (int i = 0; i < mMessages.Count; i++)
            {
                IMessage lMessage = mMessages[i];

                // 减少延迟时间
                lMessage.Delay -= Time.deltaTime;
                if (lMessage.Delay < 0) { lMessage.Delay = 0; }

                // 如果到时间了就立即发送
                if (!lMessage.IsSent && lMessage.Delay == 0)
                {
                    SendMessage(lMessage);
                    mMessages.RemoveAt(i);
                }
            }
        }
    }

    public sealed class MessageDispatcherStub : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            MessageDispatcher.Update();
        }

        public void OnDisable()
        {
            MessageDispatcher.ClearMessages();
        }
    }
}