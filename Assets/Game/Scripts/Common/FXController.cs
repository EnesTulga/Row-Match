using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class FXController : MonoBehaviour
    {
        //===================================================================================

        public static FXController Instance;

        //===================================================================================

        public int fxCount;

        public GameObject itemBreakingFXPrefab;
        private List<ParticleSystem> _itemBreakingFXPool;

        //===================================================================================

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            _itemBreakingFXPool = new List<ParticleSystem>();

            for(int i = 0; i < fxCount; i++)
            {
                ParticleSystem particle = Instantiate(itemBreakingFXPrefab, transform).GetComponent<ParticleSystem>();
                _itemBreakingFXPool.Add(particle);
            }
        }

        //===================================================================================

        public void PlayItemBreakingFX(Vector3 position, Color color)
        {
            ParticleSystem particle = null;

            for(int i = 0; i < _itemBreakingFXPool.Count; i++)
            {
                if(!_itemBreakingFXPool[i].gameObject.activeInHierarchy)
                {
                    particle = _itemBreakingFXPool[i];
                    break;
                }
            }

            if(particle != null)
            {
                ParticleSystem.MainModule main = particle.main;
                main.startColor = color;
                particle.transform.position = position;

                particle.gameObject.SetActive(true);

                particle.Clear();
                particle.Play();
            }
        }

        //===================================================================================
    }
}

