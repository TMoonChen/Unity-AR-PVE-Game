using SaveGameFree;
using System.Collections;
using System.Collections.Generic;
using TMoonEventSystem;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelCtrl : BasePanel
{
    public CanvasGroup cg;
    public Slider VolumeSlider;
    public Dropdown LanguageDropdown;

    #region Panel Function
    public override void OnEnter()
    {
        OpenPanel();

        VolumeSlider.value = UIRoot.SavedDate.Volume;
        LanguageDropdown.captionText.text = UIRoot.SavedDate.Language;

        if (LanguageDropdown.captionText.text == "中文")
        {
            LanguageDropdown.value = 0;  
        }
        else  
        {
            LanguageDropdown.value = 1;
        }
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

    #region Button Event

    public void ChangePanel(string type)
    {
        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel((UIPanelType)System.Enum.Parse(typeof(UIPanelType), type));
    }

    public void OnSaveBtnClickEvent()
    {
        string languageSelected = LanguageDropdown.options[LanguageDropdown.value].text;

        UIRoot.SavedDate.Volume = VolumeSlider.value;

        if (UIRoot.SavedDate.Language != languageSelected)
        {
            UIRoot.SavedDate.Language = languageSelected;
            if (languageSelected == "English")
            {
                LocalizationManager.Instance.Language = LocalizationManager.English;
            }
            else
            {
                LocalizationManager.Instance.Language = LocalizationManager.Chinese;
            }

            Message m = Message.Allocate();
            m.Type = "LanguageChange";
            MessageDispatcher.SendMessage(m);
            Message.Release(m);

        }

        Saver.Save(UIRoot.SavedDate, "GameSavedData");
      
    }

    #endregion

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
