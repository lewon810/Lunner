using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{

    public class SceneManager : ManagerSingleton<SceneManager>
    {
        protected override void Initialize()
        {
            Application.targetFrameRate = Defines.DefaultFps;
        }

    }
}