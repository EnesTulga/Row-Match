using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class InputHandler : MonoBehaviour
    {
        //===================================================================================

        public static InputHandler Instance;

        //===================================================================================

        public delegate void OnPlayerReleasedClickDelegate();
        public event OnPlayerReleasedClickDelegate OnPlayerReleasedClick;
        
        //===================================================================================

        public Camera mainCamera;

        //===================================================================================

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            
        }

        //===================================================================================

        void Update()
        {
            if(Input.GetMouseButtonUp(0))
            {
                OnPlayerReleasedClick?.Invoke();
            }
        }

        //===================================================================================

        public SlotItem PlayerClickedOnSlotItem()
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hitInfo && hitInfo.collider)
                {
                    if(hitInfo.transform.parent != null)
                    {
                        SlotItem slotItem = hitInfo.transform.parent.GetComponent<SlotItem>();

                        return slotItem;
                    }
                    
                }
            }

            return null;
        }

        //===================================================================================

        public SlotItem PlayerSlideOnSlotItem()
        {
            if(Input.GetMouseButton(0))
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hitInfo && hitInfo.collider)
                {
                    if(hitInfo.transform.parent != null)
                    {
                        SlotItem slotItem = hitInfo.transform.parent.GetComponent<SlotItem>();

                        return slotItem;
                    }
                    
                }
            }

            return null;
        }

        //===================================================================================
    }
}
