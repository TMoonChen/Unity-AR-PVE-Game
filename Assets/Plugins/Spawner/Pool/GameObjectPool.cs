using System.Collections.Generic;
using UnityEngine;

namespace TMoonObjectPool
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class GameObjectPool
    {
        /// <summary>
        /// 对象池的名字
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// 对象池拥有的对象
        /// </summary>
        public GameObject obj = null;

        /// <summary>
        /// 生成对象所在的父对象
        /// </summary>
        public Transform objParent = null;

        /// <summary>
        /// 对有效的对象栈存储
        /// </summary>
        private Stack<GameObject> available;

        /// <summary>
        /// 保存所有的对象
        /// </summary>
        private List<GameObject> all;

        /// <summary>
        /// 构造函数初始化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public GameObjectPool(string name, GameObject obj, Transform objParent)
        {
            this.name = name;
            this.obj = obj;
            this.objParent = objParent;
            available = new Stack<GameObject>();
            all = new List<GameObject>();
        }

        /// <summary>
        /// 从对象池中分配一个对象
        /// </summary>
        /// <returns></returns>
        public GameObject Allocate(Vector3 position, Quaternion rotation)
        {

            GameObject go;

            // 创建对象
            if (available.Count == 0)
            {
                go = GameObject.Instantiate(obj, position, rotation);
                go.transform.parent = objParent;
                go.name = this.name;
                all.Add(go);
            }
            // 使用已存在的
            else
            {
                go = available.Pop();
                go.transform.position = position;
                go.transform.rotation = rotation;
            }

            //激活对象并发送消息给对象已激活执行回调
            go.SetActive(true);
            //MessageDispatcher.SendMessage(go.name or go.tag);

            return go;
        }

        /// <summary>
        /// 从对象池中预先加载对象
        /// </summary>
        /// <param name="count">预先加载数量的数目</param>
        public void Preload(int count)
        {
            GameObject[] gos = new GameObject[count];
            GameObject parent = new GameObject(obj.name);
            // 创建预先加载的对象
            for (int i = 0; i < count; i++)
            {
                gos[i] = Allocate(Vector3.zero, Quaternion.identity);
                gos[i].transform.parent = objParent;
            }

            for (int i = 0; i < count; i++)
            {
                Release(gos[i]);
            }
        }

        /// <summary>
        /// 把对象回收到对象池中去
        /// </summary>
        /// <param name="obj"></param>
        public void Release(GameObject obj)
        {
            if (!available.Contains(obj))
            {
                available.Push(obj);

                //禁用对象并发送消息给对象已禁用执行回调
                //MessageDispatcher.SendMessage(go.name or go.tag);
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// 对象池回收所有的对象
        /// </summary>
        public void ReleaseAll()
        {
            for (int i = 0; i < all.Count; i++)
            {
                GameObject go = all[i];
                if (go != null && go.activeInHierarchy)
                {
                    Release(go);
                }
            }
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void Clear()
        {
            ReleaseAll();
            available.Clear();
            all.Clear();
        }

        /// <summary>
        /// 返回已激活的对象
        /// </summary>
        /// <returns></returns>
        public int GetActiveCount()
        {
            return all.Count - available.Count;
        }

        /// <summary>
        /// 返回可用对象的数目
        /// </summary>
        /// <returns></returns>
        public int GetAvailableCount()
        {
            return available.Count;
        }

        /// <summary>
        /// 返回对象池中的对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            return obj;
        }
    }
}
