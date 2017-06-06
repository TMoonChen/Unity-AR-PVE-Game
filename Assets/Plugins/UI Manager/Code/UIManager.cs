using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// UI面板标识
/// </summary>
public enum UIPanelType
{
    AboutPanel,
    MenuPanel,
    SettingPanel,
    LoaderPanel,
    ScenePanel,
    PausePanel,
    ARPanel,
}

public class UIManager
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }

    /// <summary>
    /// 存储所有UI面板Prefab的路径
    /// </summary>
    private Dictionary<UIPanelType, string> panelPathDict = new Dictionary<UIPanelType, string>();

    /// <summary>
    /// 保存所有已实例化的UI面板的BasePanel组件
    /// </summary>
    private Dictionary<UIPanelType, BasePanel> panelDict = new Dictionary<UIPanelType, BasePanel>();

    /// <summary>
    /// UI面板栈管理
    /// </summary>
    private Stack<BasePanel> panelStack = new Stack<BasePanel>();

    private UIManager()
    {
        ParseUIPanelTypeJson();
    }

    /// <summary>
    /// 把某个页面入栈，即把某个页面显示在界面上
    /// </summary>
    public BasePanel PushPanel(UIPanelType panelType)
    {
        //判断一下栈里面是否有页面
        if (panelStack.Count > 0)
        {
            panelStack.Peek().OnPause();
        }

        BasePanel panel = GetPanel(panelType);
        panel.OnEnter();
        panelStack.Push(panel);

        return panel;
    }

    /// <summary>
    /// 把栈顶最上面的页面出栈，即退出栈顶的页面
    /// </summary>
    public void PopPanel()
    {
        if (panelStack.Count <= 0) return;

        //关闭栈顶页面的显示
        panelStack.Pop().OnExit();

        if (panelStack.Count <= 0) return;
        panelStack.Peek().OnResume();

    }

    /// <summary>
    /// 根据UI面板类型得到实例化的UI面板
    /// </summary>
    /// <returns></returns>
    private BasePanel GetPanel(UIPanelType panelType)
    {
        BasePanel panel = panelDict.TryGet(panelType);

        if (Nullable.Equals(panel, null))
        {
            //如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
            string path = panelPathDict.TryGet(panelType);
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform, false);
            panelDict.Add(panelType, instPanel.GetComponent<BasePanel>());
            return instPanel.GetComponent<BasePanel>();
        }
        else
        {
            return panel;
        }

    }

    private void InstantiateAllPanel()
    {
        foreach (var item in Enum.GetValues(typeof(UIPanelType)))
        {
            GetPanel((UIPanelType)Enum.Parse(typeof(UIPanelType), item.ToString()));
        }
    }

    private void ParseUIPanelTypeJson()
    {
        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");

        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text);

        foreach (UIPanelInfo info in jsonObject.infoList)
        {
            panelPathDict.Add(info.panelType, info.path);
        }

    }

    public void Clear()
    {
        panelStack.Clear();
        panelDict.Clear();
    }

    public void InitUIManager()
    {
        InstantiateAllPanel();
    }
}
