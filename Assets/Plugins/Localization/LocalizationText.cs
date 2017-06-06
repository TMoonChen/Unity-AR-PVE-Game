using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMoonEventSystem;

public class LocalizationText : MonoBehaviour
{
    public string key;
    private Text text;
    
    private void Awake()
    {
        text = GetComponent<Text>();
        MessageDispatcher.AddListener("LanguageChange", LanguageChage);
    }

    private void LanguageChage(IMessage m)
    {
        text.text = LocalizationManager.Instance.GetValue(key);
    }

}
