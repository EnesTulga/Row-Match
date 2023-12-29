using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUILabel : GUIObject
    {
        //===================================================================================

        public string text;
        public float textSize;
        public GUIStyle guiStyle;
        
        //===================================================================================

        public override void Draw()
        {
            Rect drawingRect = GetProcessedRect();
            
            guiStyle.fontSize = (int)(transform.lossyScale.x * Mathf.Max(1f, textSize * drawingRect.width) * 0.01f);

            GUI.color = guiStyle.normal.textColor;

            GUI.Label(drawingRect, text, guiStyle);
        }

        //===================================================================================
    }
}

