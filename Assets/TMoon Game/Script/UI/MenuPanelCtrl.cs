using System;
using System.Collections;
using System.Collections.Generic;
using TMoonEventSystem;
using UnityEngine;

public class MenuPanelCtrl : BasePanel
{
    public CanvasGroup cg;

    #region Panel Function
    public override void OnEnter()
    {
        OpenPanel();
    }

    public override void OnPause()
    {
        ClosePanel();
    }

    public override void OnResume()
    {
        OpenPanel();
    }

    public override void OnExit()
    {
        ClosePanel();
    }
    #endregion

    public void ChangePanel(string type)
    {
        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel((UIPanelType)System.Enum.Parse(typeof(UIPanelType), type));
    }

    public void OnExitBtnClickEvent()
    {
        Application.Quit();
    }

    public void OnBeginGameClickEvent()
    {
        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel(UIPanelType.LoaderPanel);
    }

    private void OpenPanel()
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }

    private void ClosePanel()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }

}



