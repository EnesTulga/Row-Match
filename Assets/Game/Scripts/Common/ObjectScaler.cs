using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class ObjectScaler : MonoBehaviour
    {
        //===================================================================================

        public float timeToScale = 1f;
        public float scaleTimer = 0f;

        //===================================================================================

        private void Update()
        {
            if(scaleTimer < timeToScale)
            {
                scaleTimer += Time.deltaTime;
                
                if(scaleTimer < timeToScale)
                {
                    float _scaleTimeRatio = scaleTimer / timeToScale;
                    transform.localScale = Vector3.one * (2f - _scaleTimeRatio) * Mathf.Sin(_scaleTimeRatio * 90f * Mathf.Deg2Rad);
                }
                else
                {
                    transform.localScale = Vector3.one;
                }
            }    
        }

        //===================================================================================


    }
}

