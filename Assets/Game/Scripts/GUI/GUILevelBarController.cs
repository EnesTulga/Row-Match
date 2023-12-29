using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUILevelBarController : MonoBehaviour
    {
        public GUILabel levelAndMoveCountText, bestScoreText;
        public GUIButton playButton, lockedButton;
        public GUIImage backgroundImage;
        public int levelBarLevelIndex;

        public void OnPlayButtonClicked()
        {
            GameController.Instance.LoadLevelFromMenu(levelBarLevelIndex);
        }
    }
}

