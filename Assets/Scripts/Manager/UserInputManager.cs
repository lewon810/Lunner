using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{

    /// <summary>
    /// ユーザー入力の通知
    /// </summary>
    public class UserInputManager : MonoBehaviour
    {
        [SerializeField]
        private SceneBase _scene = null;

        //InputCommandに対応するキー入力
        private Dictionary<InputCommand, KeyCode> InputKeyDict = new Dictionary<InputCommand, KeyCode>();

        private void Awake()
        {
            InputKeyDict.Clear();
            var tempList = new List<string>();
            for (int i = 0; i < (int)InputCommand.Max; i++)
            {
                var eCommand = (InputCommand)i;
                if (eCommand == InputCommand.None) continue;
                if (eCommand == InputCommand.Max) continue;

                var keycode = KeyCode.None;
                switch(eCommand)
                {
                    case InputCommand.Left:
                        keycode = KeyCode.LeftArrow;
                        break;
                    case InputCommand.Right:
                        keycode = KeyCode.RightArrow;
                        break;
                    case InputCommand.Up:
                        keycode = KeyCode.UpArrow;
                        break;
                    case InputCommand.Down:
                        keycode = KeyCode.DownArrow;
                        break;
                }
                InputKeyDict.Add(eCommand, keycode);
            }
        }

		private void Update()
		{
            UpdateInput();
		}

        /// <summary>
        /// キー入力の検知
        /// シングル入力のみ検知する
        /// </summary>
        private void UpdateInput()
        {
            foreach(var kv in InputKeyDict)
            {
                if(Input.GetKeyDown(kv.Value))
                {
                    _scene.OnReceiveInputCommand(kv.Key);
                    break;
                }
            }
        }
	}
}