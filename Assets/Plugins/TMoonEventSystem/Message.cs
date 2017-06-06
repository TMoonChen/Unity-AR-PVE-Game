using TMoonCommon;
using System;

namespace TMoonEventSystem
{

    public class Message : IMessage
    {
        protected object mData = null;
        public object Data
        {
            get { return mData; }
            set { mData = value; }
        }

        protected float mDelay = 0;
        public float Delay
        {
            get { return mDelay; }
            set { mDelay = value; }
        }

        protected bool mIsSent = false;
        public bool IsSent
        {
            get { return mIsSent; }
            set { mIsSent = value; }
        }

        protected string mType = String.Empty;
        public string Type
        {
            get { return mType; }
            set { mType = value; }
        }

        protected string mFilter = String.Empty;
        public string Filter
        {
            get { return mFilter; }
            set { mFilter = value; }
        }

        public void Reset()
        {
            mType = String.Empty;
            mData = null;
            mIsSent = false;
            mFilter = string.Empty;
            mDelay = 0.0f;
        }

        // ******************************** OBJECT POOL ********************************

        /// <summary>
        /// Allows us to reuse objects without having to reallocate them over and over
        /// </summary>
        private static ObjectPool<Message> sPool = new ObjectPool<Message>(40, 10);

        /// <summary>
        /// Pulls an object from the pool.
        /// </summary>
        /// <returns></returns>
        public static Message Allocate()
        {
            // Grab the next available object
            Message lInstance = sPool.Allocate();

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // by default.
            lInstance.Reset();

            // For this type, guarentee we have something
            // to hand back tot he caller
            if (lInstance == null) { lInstance = new Message(); }
            return lInstance;
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public static void Release(Message rInstance)
        {
            if (rInstance == null) { return; }

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // and removed by default.
            rInstance.Reset();

            // Make it available to others.
            sPool.Release(rInstance);
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public static void Release(IMessage rInstance)
        {
            if (rInstance == null) { return; }

            // We should never release an instance unless we're
            // sure we're done with it. So clearing here is fine
            rInstance.Reset();

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // and removed by default.
            //rInstance.IsSent = true;

            // Make it available to others.
            if (rInstance is Message)
            {
                sPool.Release((Message)rInstance);
            }
        }
    }
}