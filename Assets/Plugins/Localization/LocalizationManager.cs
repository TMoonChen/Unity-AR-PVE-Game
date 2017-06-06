using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalizationManager
{
    private static LocalizationManager _instance;
    public static LocalizationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocalizationManager();
            }
            return _instance;
        }
    }

    //可选语言
    public const string Chinese = "Localization/Chinese";
    public const string English = "Localization/English";
    //已选语言
    private string _language = Chinese;
    public string Language
    {
        get { return _language; }
        set
        {
            if (_language != value)
            {
                _language = value;
                UpdateLanguageToDict();
            }
        }
    }
    //存储语言映射
    private Dictionary<string, string> dict = new Dictionary<string, string>();

    private LocalizationManager()
    {
        UpdateLanguageToDict();
    }

    private void UpdateLanguageToDict()
    {
        dict.Clear();
        TextAsset ta = Resources.Load<TextAsset>(Language);
        string[] lines = ta.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line) == false)
            {
                string[] keyvalues = line.Split('=');
                dict.Add(keyvalues[0], keyvalues[1]);
            }
        }
    }

    public string GetValue(string key)
    {
        string value;
        dict.TryGetValue(key, out value);
        return value;
    }
}