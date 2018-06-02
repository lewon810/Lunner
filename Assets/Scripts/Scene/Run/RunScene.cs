using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{
    /// <summary>
    /// 走りシーンのメインクラス
    /// ** 走りキャラとステージを一元で管理する
    /// </summary>
    public class RunScene : SceneBase
    {
        public RunGameState CurrentState { get; private set; }

        private static int RailPointLayer = 0;

        [SerializeField]
        private RunSceneParamsHolder _params;

        [SerializeField]
        private Rail _rail = null;
        [SerializeField]
        private RailCharacterController _character = null;
        [SerializeField]
        private RunnerCamera _mainCamera = null;

        private Rail.CalculatePositionInfo _calcPositionInfo;
        private Vector3 _prevPos;
        private bool _isInitialized = false;

        private int _lap;
        private int _lapMax;

        /// <summary>
        /// ゲームの状態を設定
        /// </summary>
        /// <param name="nextState">Next state.</param>
        public void SetGameState(RunGameState nextState)
        {
            //状態の終了・開始処理が必要な場合はこの辺に記述する

            Debug.Log("set state : " + nextState);
            CurrentState = nextState;
            switch (nextState)
            {
                case RunGameState.None:
                    break;
                case RunGameState.Start:
                    Initialize();
                    break;
                case RunGameState.Runnning:
                    break;
                case RunGameState.Goal:
                    _character.RunGoal();
                    break;
            }
        }

        /// <summary>
        /// ゲームの更新
        /// </summary>
        private void UpdateInternal()
        {
            switch (CurrentState)
            {
                case RunGameState.None:
                    break;
                case RunGameState.Start:
                    break;
                case RunGameState.Runnning:
                    UpdateRunning();
                    break;
                case RunGameState.Goal:
                    break;
            }
        }

        #region MonoBehaviour

        private void Start()
        {
            SetGameState(RunGameState.Start);
        }

        private void Update()
        {
            UpdateInternal();
        }

        #endregion

        #region SceneBase

        public override void OnReceiveInputCommand(InputCommand command)
        {
            //走ってる最中だけキー入力を受け付ける
            //if (CurrentState != RunGameState.Runnning) return;

            switch (command)
            {
                case InputCommand.Left:
                    _character.MoveToLeft();
                    break;
                case InputCommand.Right:
                    _character.MoveToRight();
                    break;
                case InputCommand.Up:
                    _character.Jump();
                    break;
                case InputCommand.Down:
                    _character.Slide();
                    break;
            }
            base.OnReceiveInputCommand(command);
        }

        #endregion

        private void Initialize()
        {
            _rail.Initialize();

            _calcPositionInfo = new Rail.CalculatePositionInfo();
            _calcPositionInfo._velocity = _params.CharacterDefaultVelocity;
            _calcPositionInfo._railIndex = 0;

            RailPointLayer = UnityEngine.LayerMask.GetMask("RailPoint");

            var startRail = _rail.GetRailPoint(0);
            _character.Initialize(_params, startRail.transform.position, new Vector3(0, startRail.ZAngle, 0));

            _mainCamera.SetPositionForce();

            _lap = 1;
            _lapMax = 3;    //TODO ステージごとにかえれるように

            _isInitialized = true;

            //出走
            RunStart();
        }

        private void RunStart()
        {
            _character.RunStart();
            SetGameState(RunGameState.Runnning);
        }

        /// <summary>
        /// 現在のキャラの情報を収集する
        /// </summary>
        private void GatherCalcInfo()
        {
            _calcPositionInfo._position = _character.GetPosition();
            _calcPositionInfo._velocity = _params.CharacterDefaultVelocity;
            _prevPos = _calcPositionInfo._position;
        }

        /// <summary>
        /// キャラの走り状態を更新する
        /// </summary>
        private void UpdateRunning()
        {
            if (_isInitialized == false) return;

            //計算の元になる情報を詰める
            GatherCalcInfo();

            //収集した情報を元に次の地点を計算
            _rail.CalcNext(_calcPositionInfo);

            //行き過ぎチェック
            CheckRailPointThrow();

            //計算された結果を元にキャラクターの姿勢を確定する
            _character.SetCalculatedTransform(_calcPositionInfo._position, _calcPositionInfo._rotation);

            //TODO:初回のスムーシングを切る処理がうまくいかないので更新時に元に戻している
            //原因わかるまではひとまずこれで
            if (_calcPositionInfo._railIndex > 0) _mainCamera.ResetSmooth();

            //ゴールチェック
            CheckGoal();
        }

        private void CheckGoal()
        {
            //ゴール判定
            if (_rail.IsGoalNode(_calcPositionInfo._railIndex))
            {
                _lap++;
                //最終ラップ到達
                if (_lap == _lapMax)
                {
                    SetGameState(RunGameState.Goal);
                }
                else
                {
                    //TODO 何かがおかしい
                    _calcPositionInfo._railIndex = 0;
                    _rail.ThrowReset();
                }
            }
        }

        /// <summary>
        /// RailPointを通り過ぎているかチェック
        /// </summary>
        private void CheckRailPointThrow()
        {
            var currentPoint = _rail.GetRailPoint(_calcPositionInfo._railIndex);
            var nextPoint = currentPoint.Next;
            if (nextPoint == null) return;

            var targetDirection = (_prevPos - _calcPositionInfo._position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(_calcPositionInfo._position, targetDirection, out hit, RailPointLayer))
            {
                if (hit.transform.gameObject.name == nextPoint.name && !nextPoint.IsThrow)
                {
                    //行き過ぎた量の補正をかける
                    var prevDistance = Vector3.Distance(_prevPos, nextPoint.transform.position);
                    var currentDistance = Vector3.Distance(_prevPos, _calcPositionInfo._position);
                    //どれだけ行き過ぎてるか割合を算出し再計算
                    var fixedValue = (currentDistance - prevDistance) / currentDistance;
                    _calcPositionInfo._position = nextPoint.transform.position;
                    _rail.CalcNext(_calcPositionInfo, fixedValue);
                    _calcPositionInfo._railIndex++;
                    nextPoint.Throw();

                    Debug.Log("Next -> " + _calcPositionInfo._railIndex);
                }
                //Debug.Log("_prevpos" + _prevPos + " , " + hit.transform.name);
            }
        }
    }
}