using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{
    /// <summary>
    /// シーンの基底クラス
    /// </summary>
    public class SceneBase : MonoBehaviour
    {
        /// <summary>
        /// キー入力受付
        /// </summary>
        /// <param name="command">Command.</param>
        public virtual void OnReceiveInputCommand(InputCommand command)
        {
            
        }
    }
}