using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class SlotItem : MonoBehaviour
    {
        //===================================================================================

        public int itemID;
        public Vector2Int gridPosition;
        public Vector3 gridWorldPosition;
        private Vector3 _lastWorldPositionBeforeGetBack;
        private float _movingTimer = -1f;
        public int itemScore;

        //===================================================================================

        public void InitializeItem(int x, int y)
        {
            gridPosition.x = x;
            gridPosition.y = y;

            gridWorldPosition = GameController.Instance.currentLevelController.GetSlotItemWorldPosition(gridPosition);

            transform.position = gridWorldPosition;
        }

        //===================================================================================

        public void SetNewGridIndex(int x, int y)
        {
            gridPosition.x = x;
            gridPosition.y = y;

            _lastWorldPositionBeforeGetBack = transform.position;
            gridWorldPosition = GameController.Instance.currentLevelController.GetSlotItemWorldPosition(gridPosition);
            _movingTimer = GameController.Instance.maxSlidingTime;
        }

        //===================================================================================

        private void Update()
        {
            if(_movingTimer > 0f)
            {
                _movingTimer -= Time.deltaTime;
                if(_movingTimer >= 0f)
                {
                    float movingTimeRatio = (GameController.Instance.maxSlidingTime - _movingTimer) / GameController.Instance.maxSlidingTime;

                    transform.position = Vector3.Lerp(_lastWorldPositionBeforeGetBack, gridWorldPosition, Mathf.Cos((270f + movingTimeRatio * 90f) * Mathf.Deg2Rad));
                    transform.localScale = Vector3.Lerp(Vector3.one , Vector3.one * 0.75f, Mathf.Sin(Mathf.Deg2Rad * 180f * movingTimeRatio));
                }
                else
                {
                    transform.position = gridWorldPosition;
                    transform.localScale = Vector3.one;
                }
            }    
        }

        //===================================================================================
        
    }
}

