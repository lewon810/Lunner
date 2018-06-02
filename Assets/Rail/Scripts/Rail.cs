using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEditorInternal;
using System.Linq;

/// <summary>
/// ノードを連結したレール
/// </summary>
public class Rail : MonoBehaviour
{
    /// <summary>
    /// 計算結果を格納するためのクラス
    /// </summary>
    public class CalculatePositionInfo
    {
        public float _velocity;
        public Vector3 _position;
        public Vector3 _rotation;
        public int _railIndex;
        public int _nextRailIndex;
        public float _currentFramePassDistance; //このフレームで進んだ距離
    }

    // 閉じる
    public bool isClose = false;

    // レール点のリスト
    public List<GameObject> railPoints = new List<GameObject>();

    /// <summary>
    /// 初回アクセス時にレールの初期化を行い全てのポイントを取得する
    /// </summary>
    /// <value>The rail point list.</value>
    public List<RailPoint> RailPointList
    {
        get
        {
            if (_railPointListCache == null)
            {
                Initialize();
            }
            return _railPointListCache;
        }
    }
    private List<RailPoint> _railPointListCache = null;

    public bool IsGoalNode(int index)
    {
        return index == RailPointList.Count - 1;
    }

    public void ThrowReset()
    {
        for (int i = 0; i < _railPointListCache.Count;i++)
        {
            _railPointListCache[i].ThrowReset();
        }
    }

    public RailPoint GetRailPoint(int index)
    {
        if (index < RailPointList.Count) return RailPointList[index];
        return null;
    }

    /// <summary>
    /// 全てのレールポイントのNextの設定などの初期化を行う
    /// </summary>
    public void Initialize()
    {
        _railPointListCache = new List<RailPoint>();
        foreach (var p in railPoints)
        {
            _railPointListCache.Add(p.GetComponent<RailPoint>());
            if (_railPointListCache.Count > 1)
            {
                int indexPrev = _railPointListCache.Count - 2;
                int indexCurrent = _railPointListCache.Count - 1;
                InitializeNode(indexPrev, indexCurrent);

                //終端ノードの向き先を開始ノードに向ける
                if(indexCurrent == railPoints.Count - 1)
                {
                    InitializeNode(indexCurrent, 0);
                }
            }
        }
        Dump();
    }

    private void InitializeNode(int indexPrev, int indexCurrent)
    {
        var prevNode = _railPointListCache[indexPrev];
        var currentNode = _railPointListCache[indexCurrent];
        prevNode.Initialize(indexPrev, currentNode);
        prevNode.gameObject.name = indexPrev.ToString();
        currentNode.gameObject.name = indexCurrent.ToString();
    }

    public void Dump()
    {
        for (int i = 0; i < RailPointList.Count; i++)
        {
            Debug.Log("index = " + i);
            GetRailPoint(i).Dump();
        }
    }

    /// <summary>
    /// 現在の位置情報を元に次の位置情報を計算して返す
    /// ** クラスをnewしないために引数のinfoを直接書き換える
    /// </summary>
    /// <returns>The next position.</returns>
    /// <param name="calcInfo">Calculate info.</param>
    /// <param name="distanceFixed">ノードを通過した際に行き過ぎた割合</param>
    public void CalcNext(CalculatePositionInfo calcInfo, float distanceFixed = 1.0f)
    {
        if (calcInfo._railIndex >= RailPointList.Count)
        {
            Debug.LogError("なんかおかしい");
            return;
        }

        var currentPoint = GetRailPoint(calcInfo._railIndex);
        var nextPoint = GetRailPoint(calcInfo._railIndex + 1);

        //次位置 = 現在位置 + 向いている方向 * 速さ
        var tempNextDiff = currentPoint.NormalizedVelocity * calcInfo._velocity * distanceFixed;
        var tempNext = calcInfo._position + tempNextDiff;
        var tempRotation = new Vector3(0, -currentPoint.ZAngle, 0);

        //計算結果を格納して返す
        calcInfo._position = tempNext;
        calcInfo._rotation = tempRotation;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        for (int i = 0, n = railPoints.Count; i < n; ++i)
        {
            if (i == n - 1 && !isClose)
            {
                break;
            }

            GameObject from = railPoints[i];
            GameObject to = railPoints[(i + 1) % n];
            Gizmos.color = Color.green;
            Gizmos.DrawLine(from.transform.position, to.transform.position);
        }
    }

    // レールのカスタムエディタ
    [CustomEditor(typeof(Rail))]
    public class RailEditor : Editor
    {
        ReorderableList railPointsReorderableList;

        private Rail rail
        {
            get
            {
                return target as Rail;
            }
        }

        void OnEnable()
        {
            railPointsReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("railPoints"));

            // コールバック設定
            railPointsReorderableList.onAddCallback += AddRailPoint;
            railPointsReorderableList.onRemoveCallback += RemoveRailPoint;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            railPointsReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        // レール点を追加する
        private void AddRailPoint(ReorderableList list)
        {
            // レール点の生成位置
            Vector3 position;
            {
                // 最初のレール点 ⇒ レールの位置
                if (rail.railPoints.Count == 0)
                {
                    position = rail.transform.position;
                }
                // 既にレール点がある ⇒ 最後のレール点の位置
                else
                {
                    GameObject railPoint_Last = rail.railPoints[rail.railPoints.Count - 1];
                    position = railPoint_Last.transform.position;
                }
            }

            // レール点生成
            GameObject prefab = (GameObject)Resources.Load("Prefabs/RailPoint");
            GameObject railPoint_New = Instantiate(prefab, position, Quaternion.identity);

            // リストに追加
            rail.railPoints.Add(railPoint_New);

            // レールの子にする
            railPoint_New.transform.parent = rail.transform;
        }

        // レール点を削除する
        private void RemoveRailPoint(ReorderableList list)
        {
            GameObject railPoint = rail.railPoints[list.index];
            rail.railPoints.Remove(railPoint);
            DestroyImmediate(railPoint);
        }
    }
#endif
}
