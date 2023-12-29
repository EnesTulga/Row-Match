using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUIObject : MonoBehaviour
    {
        //===================================================================================

        public Rect rect;
        public Vector2 anchoredPosition;

        public Vector2 screenDependentScale = new Vector2(-1f, -1f);

        public bool isClickable = false;
        public bool equalWithHeightRatio = false;
        public bool equalWithWidthRatio = false;

        public GUIObject rectFollowingObject;
        public GUIScrollbar followingScrollbar;
        public GUIObject offsetFollowingObject;
        
        //===================================================================================

        public virtual void Draw()
        {

        }

        //===================================================================================

        public virtual void DrawInScrollbar()
        {
            
        }

        //===================================================================================

        public virtual Rect GetProcessedRect()
        {
            float scaleRatio = (GUIController.nativeScreenSize.x / GUIController.nativeScreenSize.y) * (Screen.width / GUIController.nativeScreenSize.x);
            
            GUIObject guiObject = this;

            while(guiObject.rectFollowingObject != null)
            {
                guiObject = guiObject.rectFollowingObject;
            }

            Rect processed = guiObject.rect;

            if(screenDependentScale.x < 0 && screenDependentScale.y < 0)
            {
                processed.width *= guiObject.transform.lossyScale.x * scaleRatio;
                processed.height *= guiObject.transform.lossyScale.y * scaleRatio;
            }
            else
            {
                if(equalWithWidthRatio)
                {
                    processed.width = guiObject.transform.lossyScale.x * Screen.width * screenDependentScale.x;
                
                    processed.height = processed.width;
                }
                else
                {
                    processed.height = guiObject.transform.lossyScale.y * Screen.height * screenDependentScale.y;
                
                    if(!equalWithHeightRatio)
                    {
                        processed.width = guiObject.transform.lossyScale.x * Screen.width * screenDependentScale.x;
                    }
                    else
                    {
                        processed.width = processed.height;
                    }
                }
                
                
            }

            if(offsetFollowingObject == null)
            {
                processed.x = (processed.x * guiObject.transform.lossyScale.x * scaleRatio) + (Screen.width * 0.5f) + (Screen.width * guiObject.anchoredPosition.x) - processed.width * 0.5f;
                processed.y = -(processed.y * guiObject.transform.lossyScale.y * scaleRatio) + (Screen.height * 0.5f) - (Screen.height * guiObject.anchoredPosition.y) - processed.height * 0.5f;
            }
            else
            {
                Rect offsetFollowingObjectRect = offsetFollowingObject.GetProcessedRect();
                processed.x = (processed.x * guiObject.transform.lossyScale.x * scaleRatio) + (offsetFollowingObjectRect.x + offsetFollowingObjectRect.width * 0.5f) + (offsetFollowingObjectRect.width * guiObject.anchoredPosition.x) - processed.width * 0.5f;
                processed.y = -(processed.y * guiObject.transform.lossyScale.y * scaleRatio) + (offsetFollowingObjectRect.y + offsetFollowingObjectRect.height * 0.5f) - (offsetFollowingObjectRect.height * guiObject.anchoredPosition.y) - processed.height * 0.5f;
            }

            

            if(followingScrollbar != null)
            {
                processed.y += followingScrollbar.currentOffset;
            }

            return processed;
        }

        //===================================================================================
    }
}

