using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpperCtrl : MonoBehaviour
{
    public Spawner spawner;


    private void OnEnable()
    {
        if (GameManager.Instance.isDestory)
        {
            return;
        }

        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel(UIPanelType.ScenePanel);
        Time.timeScale = 1f;
        spawner.StartSpawn();
    }

    private void OnDisable()
    {

        if (GameManager.Instance.isDestory)
        {
            return;
        }

        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel(UIPanelType.ARPanel);
        string str = "所识别的图片已丢失，请重新识别...";
        GameManager.Instance.arPanelCtrl.ChangeInfomationText(str);
        Time.timeScale = 0f;
    }
}
