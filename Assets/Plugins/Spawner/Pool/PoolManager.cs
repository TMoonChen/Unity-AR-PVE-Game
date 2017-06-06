using System.Collections;
using System.Collections.Generic;
using TMoonObjectPool;
using UnityEngine;

namespace TMoonObjectPool
{
    /// <summary>
    /// 对象池管理类
    /// </summary>
    public class PoolManager
    {
        private static PoolManager instance = null;
        public static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PoolManager();
                }
                return instance;
            }
        }

        private List<GameObjectPool> pools = null;

        private PoolManager() { pools = new List<GameObjectPool>(); }

        //创建一个对象池
        public GameObjectPool Create(string name, GameObject obj,Transform objParent)
        {
            GameObjectPool pool = new GameObjectPool(name, obj, objParent);
            pools.Add(pool);
            return pool;
        }

        //移除一个对象池
        public void Remove(string name)
        {
            int index = pools.FindIndex((pool) => pool.name == name);
            pools[index].Clear();
            pools.RemoveAt(index);
        }

        //移除所有的对象池
        public void RemoveAll()
        {
            pools.Clear();
        }

        /// <summary>
        /// 得到一个对象池
        /// </summary>
        /// <param name="name">对象池的名字</param>
        /// <returns></returns>
        public GameObjectPool GetPool(string name)
        {
            return pools.Find((pool) => pool.name == name);
        }
    }
}