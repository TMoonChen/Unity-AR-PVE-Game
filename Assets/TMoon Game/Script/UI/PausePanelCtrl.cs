using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanelCtrl : BasePanel
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

    private void OpenPanel()
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }

    private void ClosePanel()
    {
        try
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
        }
        catch (System.Exception)
        {

        }

    }

    public void OnContinueBtnClick()
    {
        UIManager.Instance.PopPanel();
        Time.timeScale = 1;
    }

    public void OnExitBtnClick()
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }


}
