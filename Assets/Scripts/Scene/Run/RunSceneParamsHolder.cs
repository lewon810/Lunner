using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{
    /// <summary>
    /// レベルデザイン用パラメータ
    /// </summary>
    public class RunSceneParamsHolder : ScriptableObject
    {
        [Header("キャラクター")]

        [SerializeField]
        private float _characterSideMoveValue = 0.5f;
        public float CharacterSideMoveValue
        { get { return _characterSideMoveValue; }}

        [SerializeField]
        private float _characterDefaultVelocity = 0.1f;
        public float CharacterDefaultVelocity
        { get { return _characterDefaultVelocity; } }

        [SerializeField]
        private float _jumpCoolTime = 0.5f;
        public float JumpCoolTime
        { get { return _jumpCoolTime; }}

        [SerializeField]
        private float _slideCoolTime = 0.5f;
        public float SlideCoolTime
        { get { return _slideCoolTime; } }

        [SerializeField]
        private float _moveCoolTime = 0.5f;
        public float MoveCoolTime
        { get { return _moveCoolTime; } }

        [SerializeField]
        private float _charaRotationLerpTime = 0.5f;
        public float CharaRotationLerpTime
        { get { return _charaRotationLerpTime; } }
    }
}