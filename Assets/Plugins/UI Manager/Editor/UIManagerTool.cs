using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UIManagerTool
{
    [MenuItem("Tools/UI Manager/UIPanelToJson")]
    private static void WriteUIPanelToJson()
    {
        UIPanelTypeJson list = new UIPanelTypeJson();
        GameObject[] allPanel = Resources.LoadAll<GameObject>("UIPanel");
        for (int i = 0; i < allPanel.Length; i++)
        {
            UIPanelInfo temp = new UIPanelInfo();
            temp.panelTypeString = allPanel[i].name;
            temp.path = "UIPanel/" + allPanel[i].name;
            list.infoList.Add(temp);
        }
        
        WriteJsonToAsset(JsonUtility.ToJson(list));
    }

    private static void WriteJsonToAsset(string json)
    {
        using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/UIPanelType.json", false, Encoding.UTF8))
        {
            sw.WriteLine(json);
        }
    }

}
