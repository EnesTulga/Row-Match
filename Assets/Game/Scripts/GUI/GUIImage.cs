using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUIImage : GUIObject
    {
        //===================================================================================

        public Color color = Color.white;
        public Texture image;
        public Sprite sprite;

        //===================================================================================

        public override void Draw()
        {
            Rect drawingRect = GetProcessedRect();

            GUI.color = color;

            if(image != null)
            {
                GUI.DrawTexture(drawingRect, image);
            }
            else if(sprite != null)
            {
                Rect spriteRect = sprite.rect;
                Texture2D tex = sprite.texture;
                
                GUI.DrawTextureWithTexCoords(drawingRect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width/ tex.width, spriteRect.height / tex.height));
            }
            
        }

        //===================================================================================

        public override void DrawInScrollbar()
        {
            Rect drawingRect = GetProcessedRect();
            Rect scrollbarRect = followingScrollbar.GetProcessedRect();

            if(scrollbarRect.Contains(drawingRect.max) || scrollbarRect.Contains(drawingRect.min))
            {
                GUI.color = color;
                GUI.DrawTexture(drawingRect, image);
            }
            
        }
    }
}

