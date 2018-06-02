using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{
    /// <summary>
    /// レール上で移動するキャラを表現するクラス
    /// </summary>
    public class RailCharacterController : MonoBehaviour
    {
        private enum AnimationState
        {
            Default = 0,
            Run,
            Jump,
            Slide,

            Max,
        }
        private AnimationState _currentAnim;
        private Dictionary<AnimationState, string> _stateNameDict = new Dictionary<AnimationState, string>();

        [SerializeField]
        private Transform _charaRootTrans;

        [SerializeField]
        private Animator _animator = null;

        private float _sideMoveCoolTime = 0.0f;

        private Transform _cachedTrans;

        private RailPositionIndex _positionIndex;

        //位置
        private Vector3 _positionBuff;
        private float _targetX = 0;
        private float _fromX = 0;
        private bool _validMove;

        //回転
        private Vector3 _prevRotation;
        private Vector3 _targetRotation;
        private float _rotationLerpTimeCurrent = 0.0f;

        //パラメータ
        private RunSceneParamsHolder _params;

        //ジャンプ
        private bool _validJump;
        private float _jumpCoolTime;

        //スライディング
        private bool _validSlide;
        private float _slideCoolTime;

        public void Initialize(RunSceneParamsHolder paramsHolder, Vector3 startPosition, Vector3 startRotation)
        {
            _params = paramsHolder;

            _cachedTrans = transform;
            _cachedTrans.position = startPosition;
            _cachedTrans.rotation = Quaternion.Euler(startRotation);
            _positionIndex = RailPositionIndex.Center;

            _prevRotation = Utils.GetRotationIn180(startRotation);
            _targetRotation = Utils.GetRotationIn180(startRotation);
            _charaRootTrans.transform.rotation = Quaternion.Euler(startRotation);

            _validJump = true;
            _validSlide = true;
            _validMove = true;

            _stateNameDict.Clear();
            for (int i = 0; i < (int)AnimationState.Max; i++)
            {
                var eState = (AnimationState)i;
                _stateNameDict.Add(eState, eState.ToString());
            }
        }

        /// <summary>
        /// モーション再生
        /// </summary>
        /// <param name="state">State.</param>
        private void PlayMotion(AnimationState state)
        {
            string stateName = null;
            if (_stateNameDict.TryGetValue(state, out stateName))
            {
                _animator.Play(stateName);
            }
        }

        //左
        public void MoveToLeft()
        {
            if (_validMove == false) return;

            _fromX = _charaRootTrans.localPosition.x;
            switch (_positionIndex)
            {
                case RailPositionIndex.Center:
                    _targetX = _charaRootTrans.localPosition.x - _params.CharacterSideMoveValue;
                    _positionIndex = RailPositionIndex.Left;
                    break;
                case RailPositionIndex.Right:
                    _targetX = _charaRootTrans.localPosition.x - _params.CharacterSideMoveValue;
                    _positionIndex = RailPositionIndex.Center;
                    break;
                default: break;
            }
            _sideMoveCoolTime = _params.MoveCoolTime;
            _validMove = false;
        }

        //右
        public void MoveToRight()
        {
            if (_validMove == false) return;

            _fromX = _charaRootTrans.localPosition.x;
            switch (_positionIndex)
            {
                case RailPositionIndex.Center:
                    _targetX = _charaRootTrans.localPosition.x + _params.CharacterSideMoveValue;
                    _positionIndex = RailPositionIndex.Right;
                    break;
                case RailPositionIndex.Left:
                    _targetX = _charaRootTrans.localPosition.x + _params.CharacterSideMoveValue;
                    _positionIndex = RailPositionIndex.Center;
                    break;
                default: break;
            }
            _sideMoveCoolTime = _params.MoveCoolTime;
            _validMove = false;
        }

        //ジャンプ
        public void Jump()
        {
            if (_validJump)
            {
                PlayMotion(AnimationState.Jump);
                _jumpCoolTime = _params.JumpCoolTime;
                _validJump = false;
                _validSlide = false;
            }
        }

        //スライディング
        public void Slide()
        {
            if (_validSlide)
            {
                PlayMotion(AnimationState.Slide);
                _slideCoolTime = _params.SlideCoolTime;
                _validSlide = false;
                _validJump = false;
            }
        }

        /// <summary>
        /// 走りアニメと位置移動の開始
        /// </summary>
        public void RunStart()
        {
            PlayMotion(AnimationState.Run);
        }

        public void RunGoal()
        {
            PlayMotion(AnimationState.Default);
        }

        /// <summary>
        /// Transformを設定する
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        public void SetCalculatedTransform(Vector3 position, Vector3 rotation)
        {
            _cachedTrans.position = position;

            _prevRotation = Utils.GetRotationIn180(_cachedTrans.localRotation.eulerAngles);
            _targetRotation = Utils.GetRotationIn180(rotation);

            _rotationLerpTimeCurrent = _params.CharaRotationLerpTime;
            //TODO:回転の補間がうまくいかんので直すこと
#if false
            _rotationLerpTimeCurrent = 0.0f;
#endif
        }

        public Vector3 GetPosition()
        {
            return _cachedTrans.position;
        }

        private void CalcCoolTime()
        {
            var deltaTime = Time.deltaTime;
            //ジャンプ
            if(_jumpCoolTime > 0)
            {
                _jumpCoolTime -= deltaTime;
                if(_jumpCoolTime < 0)
                {
                    _jumpCoolTime = 0.0f;
                    _validJump = true;
                    _validSlide = true;
                }
            }
            //スライディング
            if (_slideCoolTime > 0)
            {
                _slideCoolTime -= deltaTime;
                if (_slideCoolTime < 0)
                {
                    _slideCoolTime = 0.0f;
                    _validSlide = true;
                    _validJump = true;
                }
            }
            //横移動
            if(_sideMoveCoolTime > 0)
            {
                _sideMoveCoolTime -= deltaTime;
                if(_sideMoveCoolTime < 0)
                {
                    _sideMoveCoolTime = 0.0f;
                    _validMove = true;
                }
            }
        }

        /// <summary>
        /// 横移動の更新
        /// </summary>
        private void UpdateSideMove()
        {
            if (_params == null) return;
            if (_sideMoveCoolTime <= 0.0f) return;

            var rate = _sideMoveCoolTime / _params.MoveCoolTime;
            _positionBuff = _charaRootTrans.localPosition;
            _positionBuff.x = Mathf.Lerp(_targetX,_fromX, rate);

            _charaRootTrans.localPosition = _positionBuff;
        }

        /// <summary>
        /// 回転更新
        /// </summary>
        private void UpdateRotation()
        {
            if (_params == null) return;

            _rotationLerpTimeCurrent += Time.deltaTime;
            if(_rotationLerpTimeCurrent >= _params.CharaRotationLerpTime)
            {
                _cachedTrans.localRotation = Quaternion.Euler(_targetRotation);
                return;
            }

            var rate = _rotationLerpTimeCurrent / _params.CharaRotationLerpTime;
            var rotation = Vector3.Lerp(_prevRotation, _targetRotation,rate);
            _cachedTrans.localRotation = Quaternion.Euler(rotation);
        }

#region MonoBehaviour

		private void Update()
		{
            //位置の更新
            UpdateSideMove();
            //回転の更新
            UpdateRotation();
            //クールタイムの更新
            CalcCoolTime();
		}

#endregion
	}
}