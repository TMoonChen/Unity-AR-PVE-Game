using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARPanelCtrl : BasePanel
{
    public CanvasGroup cg;

    public Text InfomationText;

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

    public void ChangeInfomationText(string str)
    {
        InfomationText.text = str;
    }

    public void OnExitBtnClickEvent()
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    private void OpenPanel()
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }

    private void ClosePanel()
    {
        if (GameManager.Instance.isDestory)
        {
            return;
        }

        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
}
