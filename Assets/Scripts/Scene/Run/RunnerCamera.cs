using UnityEngine;
using System.Collections;

namespace Lunner
{
	public class RunnerCamera : MonoBehaviour
	{
		public float smooth = 3f;		// カメラモーションのスムーズ化用変数
		
        [SerializeField]
        private Transform standardPos = null;			// the usual position for the camera, specified by a transform in the game
        [SerializeField]
		private Transform frontPos = null;			// Front Camera locater
		[SerializeField]
        private Transform jumpPos = null;			// Jump Camera locater
	
		// スムーズに繋がない時（クイック切り替え）用のブーリアンフラグ
		bool bQuickSwitch = false;	//Change Camera Position Quickly
        private float _defaultSmooth;

		private void Awake()
		{
            _defaultSmooth = smooth;
		}

		void Start ()
		{
			//カメラをスタートする
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;
		}
	
		void FixedUpdate ()	// このカメラ切り替えはFixedUpdate()内でないと正常に動かない
		{
		
			if (Input.GetButton ("Fire1")) {	// left Ctlr	
				// Change Front Camera
				setCameraPositionFrontView ();
			} else if (Input.GetButton ("Fire2")) {	//Alt	
				// Change Jump Camera
				setCameraPositionJumpView ();
			} else {	
				// return the camera to standard position and direction
				setCameraPositionNormalView ();
			}
		}

        /// <summary>
        /// 強制的にカメラをキャラの背後にセットする
        /// </summary>
        public void SetPositionForce()
        {
            bQuickSwitch = true;
            smooth = 100;
        }

        public void ResetSmooth()
        {
            smooth = _defaultSmooth;
        }

		void setCameraPositionNormalView ()
		{
			if (bQuickSwitch == false) {
				// the camera to standard position and direction
				transform.position = Vector3.Lerp (transform.position, standardPos.position, Time.fixedDeltaTime * smooth);	
				transform.forward = Vector3.Lerp (transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
			} else {
				// the camera to standard position and direction / Quick Change
				transform.position = standardPos.position;	
				transform.forward = standardPos.forward;
				bQuickSwitch = false;
			}
		}

		void setCameraPositionFrontView ()
		{
			// Change Front Camera
			bQuickSwitch = true;
            smooth = 100;
			transform.position = frontPos.position;	
			transform.forward = frontPos.forward;
		}

		void setCameraPositionJumpView ()
		{
			// Change Jump Camera
			bQuickSwitch = false;
			transform.position = Vector3.Lerp (transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);	
			transform.forward = Vector3.Lerp (transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);		
		}
	}
}