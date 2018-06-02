using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPoint : MonoBehaviour {

    public Transform CachedTrans{
        get{
            if (_cachedTransform == null) _cachedTransform = transform;
            return _cachedTransform;
        }
    }
    private Transform _cachedTransform;

    public RailPoint Next { get { return _next; } }
    [SerializeField]
    private RailPoint _next;
    public Vector3 NormalizedVelocity { get { return _normalizedVelocity; } }
    [SerializeField]
    private Vector3 _normalizedVelocity;
    public float ZAngle { get { return _zAngle; }}
    [SerializeField]
    private float _zAngle;
    public float Distance { get { return _distance; } }    //次ノードまでの距離
    [SerializeField]
    private float _distance;
    public bool IsThrow { get { return _isThrow; }}
    [SerializeField]
    private bool _isThrow;
    public int Index { get { return _index; } }
    [SerializeField]
    private int _index;

    /// <summary>
    /// 次のノードへの参照と次のノードへの方向の初期化を行う
    /// </summary>
    /// <param name="next">Next.</param>
    public void Initialize(int index,RailPoint next){
        _isThrow = false;
        _index = index;
        _next = next;
        _normalizedVelocity = (next.CachedTrans.position - CachedTrans.position).normalized;
        _zAngle = Vector3.Angle(NormalizedVelocity, Vector3.forward);
        if (NormalizedVelocity.x > 0) _zAngle *= -1;
        _distance = Vector3.Distance(CachedTrans.position,next.CachedTrans.position);
    }

    /// <summary>
    /// 通り過ぎたら呼ぶこと
    /// </summary>
    public void Throw() { _isThrow = true; }
    /// <summary>
    /// 周回時に呼ぶこと
    /// </summary>
    public void ThrowReset() { _isThrow = false; }

    public void Dump(){
        string log = "distance: " + Distance + ", normalizedVelocity: " + NormalizedVelocity + ", ZAngle : " + ZAngle;
        Debug.Log(log);
    }

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawCube (transform.position, Vector3.one);
	}
}
