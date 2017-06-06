using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMoonEventSystem;
using SaveGameFree;

public class UIRoot : MonoBehaviour
{
    public static GameSavedData SavedDate = new GameSavedData();

    private void Start()
    {
        // 在UIManager里显示MenuPanel
        UIManager.Instance.InitUIManager();
        UIManager.Instance.PushPanel(UIPanelType.MenuPanel);

        // 加载游戏设置配置文件
        Saver.InitializeDefault();
        SavedDate = Saver.Load<GameSavedData>("GameSavedData");

        // 更新语言选择设置
        if (SavedDate.Language == "中文")
        {
            LocalizationManager.Instance.Language = LocalizationManager.Chinese;
        }
        else
        {
            LocalizationManager.Instance.Language = LocalizationManager.English;
        }

        // 发送语言更新消息
        Message m = Message.Allocate();
        m.Type = "LanguageChange";
        MessageDispatcher.SendMessage(m);
        Message.Release(m);  
    }

    private void OnDestroy()
    {
        UIManager.Instance.Clear();  
        MessageDispatcher.ClearMessages();
        System.GC.Collect();
    }
}
