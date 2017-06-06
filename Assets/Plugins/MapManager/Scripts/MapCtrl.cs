using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCtrl : MonoBehaviour, IPointerClickHandler
{
    RectTransform myRectTransform;
    public GameObject go;
    public MeshRenderer env;

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float mapX = eventData.pressPosition.x - ((float)Screen.width - myRectTransform.rect.width);
        float mapY = eventData.pressPosition.y - ((float)Screen.height - myRectTransform.rect.height);

        float realX = (mapX / myRectTransform.rect.width) * env.bounds.size.x - env.bounds.size.x / 2;
        float readY = (mapY / myRectTransform.rect.height) * env.bounds.size.z - env.bounds.size.z / 2;

        GameObject.Instantiate(go, new Vector3(realX, 0, readY), Quaternion.identity);
    }
}
