using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMoonMapManager
{
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        /// MapManager单例
        /// </summary>
        public static MapManager Instance;
        /// <summary>
        /// 实例化在小地图上的图标
        /// </summary>
        public GameObject IconPrefab;
        /// <summary>
        /// 小地图对应的环境
        /// </summary>
        public Transform target;
        /// <summary>
        /// 小地图缩放的比例
        /// </summary>
        public float zoomLevel = 10f;
        /// <summary>
        /// 小地图是否锁定旋转，如果锁定旋转就不会受到target三维目标旋转而导致icon旋转
        /// </summary>
        public bool lockRotation = false;
        /// <summary>
        /// 这个对象是否被销毁
        /// </summary>
        public bool isDestroy = false;
        Vector2 xRotation = Vector2.right;
        Vector2 yRotation = Vector2.up;
        private void Awake()
        {
            Instance = this;
            isDestroy = false;
        }

        private void LateUpdate()
        {
            if (!lockRotation)
            {
                xRotation = new Vector2(target.right.x, -target.right.z);
                yRotation = new Vector2(-target.forward.x, target.forward.z);
            }
        }

        private void OnDestroy()
        {
            isDestroy = true;
        }

        /// <summary>
        /// 地图标识字典，存储已创建的标识
        /// </summary>
        private Dictionary<int, MapIcon> mapIcons = new Dictionary<int, MapIcon>();

        /// <summary>
        /// go要已被实例出来，或者被对象池创建出来
        /// 
        /// 根据传入的go在地图上创建对应的go图标
        /// </summary>
        /// <param name="go">图标对应的对象</param>
        /// <param name="typeName">图标的资源名字</param>
        /// <param name="type">图标类型</param>
        /// <param name="minScale">图标在地图上显示的最小大小</param>
        /// <param name="LockScale">是否锁定图标原有的大小，设置为true则minScale无效</param>
        /// <param name="KeedInBounds">若超出地图范围是否保持在地图边缘</param>
        /// <param name="LockRotation">是否锁定旋转，如果锁定，真实物体旋转而图标不会旋转</param>
        public void AddMapIconByGO(GameObject go, MapIconType type)
        {
            MapIcon icon = GameObject.Instantiate<GameObject>(IconPrefab, this.transform).GetComponent<MapIcon>();
            icon.target = go.transform;
            icon.Id = go.GetInstanceID();
            icon.type = type;
            Image image = icon.GetComponent<Image>();

            switch (type)
            {
                case MapIconType.Player:
                    image.color = Color.red;
                    break;
                case MapIconType.Monster:
                    image.color = Color.black;
                    break;
            }
            mapIcons.Add(icon.Id, icon);
        }

        public void RemoveMapIconById(int id)
        {
            Destroy(mapIcons[id].gameObject);
            mapIcons.Remove(id);
        }

        public MapIcon GetMapIconById(int id)
        {
            return mapIcons[id];
        }

        /// <summary>
        /// 将3d物体坐标转换为2d图标的坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 TransformPosition(Vector3 position)
        {
            Vector3 offset = position - target.position;
            Vector2 newPosition = offset.x * xRotation;
            newPosition += offset.z * yRotation;

            newPosition *= zoomLevel;
            return newPosition;
        }

        /// <summary>
        /// 将3d物体旋转转换为2d图标的旋转
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Vector3 TransformRotation(Vector3 rotation)
        {
            if (!lockRotation)
            {
                return new Vector3(0, 0, -rotation.y);
            }
            else
            {
                return new Vector3(0, 0, target.eulerAngles.y - rotation.y);
            }
        }

        /// <summary>
        /// 限制icon移出地图，如果移出地图，则会卡在地图边缘
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 MoveInside(Vector2 point)
        {
            Rect mapRect = GetComponent<RectTransform>().rect;
            point = Vector2.Max(point, mapRect.min);
            point = Vector2.Min(point, mapRect.max);
            return point;
        }
    }
}


