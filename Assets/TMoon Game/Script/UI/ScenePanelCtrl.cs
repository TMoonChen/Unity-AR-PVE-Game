using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePanelCtrl : BasePanel
{

    public CanvasGroup cg;

    #region Panel Function
    public override void OnEnter()
    {
        OpenPanel();
    }

    public override void OnPause()
    {
        cg.blocksRaycasts = false;
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

    private void OpenPanel()
    {
        try
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
        }
        catch (System.Exception)
        {

        }

    }

    private void ClosePanel()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }

    public void OnPauseBtnClick()
    {
        Time.timeScale = 0;
        UIManager.Instance.PushPanel(UIPanelType.PausePanel);
    }
}
