using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDText : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private RectTransform r_Transform;

    private void Awake()
    {
        r_Transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 temp = target.TransformPoint(offset);
            Vector3 ScenePos = Camera.main.WorldToScreenPoint(temp);
            r_Transform.anchoredPosition3D = ScenePos;
        }
    }
}
