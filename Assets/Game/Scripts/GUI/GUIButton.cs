using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUIButton : GUIObject , IClickable
    {
        //===================================================================================

        private Color _currentColor;
        public Texture image;

        public Color normalColor = Color.white;
        public Color clickedColor = Color.gray;

        private bool _clicked = false;

        public UnityEngine.Events.UnityEvent onClick;

        //===================================================================================

        private void Start() 
        {
            _currentColor = normalColor;
        }

        //===================================================================================

        public override void Draw()
        {
            Rect drawingRect = GetProcessedRect();

            GUI.color = _currentColor;
            GUI.DrawTexture(drawingRect, image);
        }

        //===================================================================================

        public void OnPointerDown()
        {
            _clicked = true;
            _currentColor = clickedColor;
        }

        //===================================================================================

        public void OnPointerUp()
        {
            _clicked = false;
            _currentColor = normalColor;
            onClick?.Invoke();
        }

        //===================================================================================

        public void OnPointerExit()
        {
            _clicked = false;
            _currentColor = normalColor;
        }

        //===================================================================================

        public bool IsClickedLastFrame()
        {
            return _clicked;
        }
        
        //===================================================================================
    }

}
