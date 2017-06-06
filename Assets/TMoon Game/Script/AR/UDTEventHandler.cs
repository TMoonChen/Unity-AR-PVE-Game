using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Linq;
using UnityEngine.UI;

public class UDTEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
{
    #region PUBLIC_MEMBERS
    public ImageTargetBehaviour ImageTargetTemplate;

    public static UDTEventHandler Instance;

    public int LastTargetIndex
    {
        get { return (Instance.mTargetCounter - 1) % MAX_TARGETS; }
    }
    #endregion

    #region PRIVATE_MEMBERS
    public const int MAX_TARGETS = 1;
    public UserDefinedTargetBuildingBehaviour mTargetBuildingBehaviour;
    public ObjectTracker mObjectTracker;

    public DataSet mBuiltDataSet;

    public ImageTargetBuilder.FrameQuality mFrameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;

    public int mTargetCounter;

    public TrackableSettings mTrackableSettings;
    #endregion

    #region MONOBEHAVIOUR_METHODS
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Instance.mTargetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();
        if (Instance.mTargetBuildingBehaviour)
        {
            Instance.mTargetBuildingBehaviour.RegisterEventHandler(this);
        }

    }

    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion

    #region IUserDefinedTargetEventHandler implementation   
    public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
    {
        Instance.mFrameQuality = frameQuality;
        if (Instance.mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_LOW)
        {
            string str = "扫描的图片识别度不够高,请更换图片...";
            GameManager.Instance.arPanelCtrl.ChangeInfomationText(str);
        }
        else
        {
            string str = "扫描的图片识别度及格，请按下识别按钮...";
            GameManager.Instance.arPanelCtrl.ChangeInfomationText(str);
        }
    }

    public void OnInitialized()
    {
        Instance.mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (Instance.mObjectTracker != null)
        {
            // Create a new dataset
            Instance.mBuiltDataSet = mObjectTracker.CreateDataSet();
            Instance.mObjectTracker.ActivateDataSet(Instance.mBuiltDataSet);
        }
    }

    public void OnNewTrackableSource(TrackableSource trackableSource)
    {
        Instance.mTargetCounter++;

        // Deactivates the dataset first
        Instance.mObjectTracker.DeactivateDataSet(Instance.mBuiltDataSet);

        // Destroy the oldest target if the dataset is full or the dataset 
        // already contains five user-defined targets.
        if (Instance.mBuiltDataSet.HasReachedTrackableLimit() || Instance.mBuiltDataSet.GetTrackables().Count() >= MAX_TARGETS)
        {
            IEnumerable<Trackable> trackables = Instance.mBuiltDataSet.GetTrackables();
            Trackable oldest = null;
            foreach (Trackable trackable in trackables)
            {
                if (oldest == null || trackable.ID < oldest.ID)
                    oldest = trackable;
            }

            if (oldest != null)
            {
                //Debug.Log("Destroying oldest trackable in UDT dataset: " + oldest.Name);
                Instance.mBuiltDataSet.Destroy(oldest, true);
            }
        }

        // Get predefined trackable and instantiate it
        ImageTargetBehaviour imageTargetCopy = (ImageTargetBehaviour)Instantiate(Instance.ImageTargetTemplate);
        imageTargetCopy.gameObject.name = "UserDefinedTarget-" + Instance.mTargetCounter;

        // Add the duplicated trackable to the data set and activate it
        Instance.mBuiltDataSet.CreateTrackable(trackableSource, imageTargetCopy.gameObject);

        // Activate the dataset again
        Instance.mObjectTracker.ActivateDataSet(Instance.mBuiltDataSet);

        // Extended Tracking with user defined targets only works with the most recently defined target.
        // If tracking is enabled on previous target, it will not work on newly defined target.
        // Don't need to call this if you don't care about extended tracking.
        StopExtendedTracking();
        Instance.mObjectTracker.Stop();
        Instance.mObjectTracker.ResetExtendedTracking();
        Instance.mObjectTracker.Start();

        // Make sure TargetBuildingBehaviour keeps scanning...
        Instance.mTargetBuildingBehaviour.StartScanning();
    }
    #endregion

    #region PUBLIC_METHODS
    public void BuildNewTarget()
    {
        if (Instance.mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM ||
            Instance.mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH)
        {
            // create the name of the next target.
            // the TrackableName of the original, linked ImageTargetBehaviour is extended with a continuous number to ensure unique names
            string targetName = string.Format("{0}-{1}", Instance.ImageTargetTemplate.TrackableName, Instance.mTargetCounter);

            // generate a new target:
            Instance.mTargetBuildingBehaviour.BuildNewTarget(targetName, Instance.ImageTargetTemplate.GetSize().x);

        }
        else
        {
            string str = "扫描的图片识别度不够高，识别失败，请更换图片...";
            GameManager.Instance.arPanelCtrl.ChangeInfomationText(str);
        }
    }
    #endregion

    #region PRIVATE_METHODS

    private void StopExtendedTracking()
    {
        // If Extended Tracking is enabled, we first disable it for all the trackables
        // and then enable it only for the newly created target
        bool extTrackingEnabled = Instance.mTrackableSettings && Instance.mTrackableSettings.IsExtendedTrackingEnabled();
        if (extTrackingEnabled)
        {
            StateManager stateManager = TrackerManager.Instance.GetStateManager();

            // 1. Stop extended tracking on all the trackables
            foreach (var tb in stateManager.GetTrackableBehaviours())
            {
                var itb = tb as ImageTargetBehaviour;
                if (itb != null)
                {
                    itb.ImageTarget.StopExtendedTracking();
                }
            }

            // 2. Start Extended Tracking on the most recently added target
            List<TrackableBehaviour> trackableList = stateManager.GetTrackableBehaviours().ToList();
            ImageTargetBehaviour lastItb = trackableList[Instance.LastTargetIndex] as ImageTargetBehaviour;
            if (lastItb != null)
            {
                if (lastItb.ImageTarget.StartExtendedTracking())  
                    Debug.Log("Extended Tracking successfully enabled for " + lastItb.name);
            }
        }
    }

    #endregion
}
