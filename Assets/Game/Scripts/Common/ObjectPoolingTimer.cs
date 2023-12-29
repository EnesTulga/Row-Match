using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class ObjectPoolingTimer : MonoBehaviour
    {
        //===================================================================================

        public float poolingTime = 3f;
        private float _poolingTimer = -1f;

        //===================================================================================

        private void OnEnable()
        {
            _poolingTimer = poolingTime;
        }

        //===================================================================================
        
        void Update()
        {
            if(_poolingTimer > 0f)
            {
                _poolingTimer -= Time.deltaTime;
                if(_poolingTimer <= 0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        //===================================================================================
    }
}

