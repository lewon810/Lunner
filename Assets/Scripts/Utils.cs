using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{
    /// <summary>
    /// よく使う関数たち
    /// </summary>
    public class Utils
    {
        public static Vector3 GetRotationIn180(Vector3 origin)
        {
            var result = origin;
            if (origin.x > 180) result.x -= 180;
            if (origin.x < -180) result.x += 180;
            if (origin.y > 180) result.y -= 180;
            if (origin.y < -180) result.y += 180;
            if (origin.z > 180) result.z -= 180;
            if (origin.z < -180) result.z += 180;
            return result;
        }

    }
}
