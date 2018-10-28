using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lunner
{

    public class MotionTest : MonoBehaviour
    {
        public RailCharacterController _character;

        private void Start()
        {
            _character.CreateMotionStateDict();
        }

        public void PlayMotion()
        {
            _character.RunStart();
        }
    }
}