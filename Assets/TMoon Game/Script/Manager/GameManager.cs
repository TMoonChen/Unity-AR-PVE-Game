using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMoonEventSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public Transform mainCanvas;
    [HideInInspector]
    public PlayerManager playerManager;

    public List<Vector3> EnemyAttackPositions;
    public ARPanelCtrl arPanelCtrl;
    public ScoreViewModel ScoreModel = new ScoreViewModel();
    public PlayerViewModel PlayerModel = new PlayerViewModel();

    public bool isDestory = false;

    private int _index;
    public int Index
    {
        get
        {
            return ++_index % EnemyAttackPositions.Count;
        }
    }

    private void Awake()
    {
        _index = EnemyAttackPositions.Count;
        ScoreModel.Score.Value = 0;
        PlayerModel.HP.Value = 10000;
        mainCanvas = GameObject.Find("Canvas").transform;
        Instance = this;
    }

    private void Start()
    {
        arPanelCtrl = (ARPanelCtrl)UIManager.Instance.PushPanel(UIPanelType.ARPanel);
    }

    private void OnDisable()
    {
        isDestory = true;
    }

    private void OnDestroy()
    {
        UIManager.Instance.Clear();
        MessageDispatcher.ClearMessages();
        System.GC.Collect();
    }

}
