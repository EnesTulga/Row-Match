using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public interface IClickable
    {
        void OnPointerDown();
        void OnPointerUp();
        void OnPointerExit();
        bool IsClickedLastFrame();
    }
}

