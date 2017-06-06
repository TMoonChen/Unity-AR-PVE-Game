using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMoonMapManager
{
    public enum MapIconType
    {
        Player,
        Monster,
        Map
    }

    public class MapIcon : MonoBehaviour
    {
        /// <summary>
        /// 图标存储在MapManager的唯一标识
        /// </summary>
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 图标类型
        /// </summary>
        public MapIconType type;
        /// <summary>
        /// 图标对应的真实物体
        /// </summary>
        public Transform target;
        /// <summary>
        /// 若超出地图范围是否保持在地图边缘
        /// </summary>
        public bool keedInBounds = true;
        /// <summary>
        /// 是否锁定图标的大小，若锁定则minScale设置会失效
        /// </summary>
        public bool lockScale = false;
        /// <summary>
        /// 是否锁定旋转，如果锁定，真实物体旋转而图标不会旋转
        /// </summary>
        public bool lockRotation = false;
        /// <summary>
        /// 图标最小的显示大小
        /// </summary>
        public float minScale = 1f;

        RectTransform myRectTransform;

        private void Start()
        {
            myRectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            Vector2 newPosition = MapManager.Instance.TransformPosition(target.position);

            if (keedInBounds)
            {
                newPosition = MapManager.Instance.MoveInside(newPosition);
            }

            if (!lockScale)
            {
                float scale = Mathf.Max(minScale, MapManager.Instance.zoomLevel);
                myRectTransform.localScale = new Vector3(scale, scale, 1f);
            }

            if (!lockRotation)
            {
                myRectTransform.localEulerAngles = MapManager.Instance.TransformRotation(target.eulerAngles);
            }

            myRectTransform.localPosition = newPosition;
        }
    }
}


