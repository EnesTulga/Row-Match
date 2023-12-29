using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUIScrollbar : GUIObject, IClickable
    {
        //===================================================================================

        private bool _clicked = false;
        
        public float slideSpeed;
        public int minOffset, maxOffset;
        public int currentOffset;
        
        private Vector2 _lastPointerPosition;
        
        //===================================================================================

        private void Update()
        {
            if(_clicked)
            {
                float yOffset = Screen.dpi * (Input.mousePosition.y - _lastPointerPosition.y);
                _lastPointerPosition = Input.mousePosition;

                currentOffset = Mathf.Clamp(currentOffset - (int)(yOffset * Time.deltaTime * slideSpeed), minOffset, maxOffset);
            }    
        }

        //===================================================================================

        public void OnPointerDown()
        {
            if(!_clicked)
            {
                _clicked = true;
                _lastPointerPosition = Input.mousePosition;
            }
            
        }

        //===================================================================================

        public void OnPointerUp()
        {
            if(_clicked)
            {
                _clicked = false;
            }
        }

        //===================================================================================

        public void OnPointerExit()
        {
            if(_clicked)
            {
                _clicked = false;
            }
        }

        //===================================================================================

        public bool IsClickedLastFrame()
        {
            return _clicked;
        }
        
        //===================================================================================
    }
}

