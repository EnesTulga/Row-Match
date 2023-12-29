using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GUIController : MonoBehaviour
    {
        //===================================================================================

        public static Vector2 nativeScreenSize = new Vector2(640, 480);

        public const string noScoreText = "No Score";
        public const string highestScoreText = "Highest Score : ";
        public const string BestScoreText = "BEST SCORE : ";

        //===================================================================================

        public LevelController levelController;
        public GUILabel scoreText;
        public GUILabel remainingMoveCountText;

        public GUIButton levelsButton;
        public GUIScrollbar levelsScrollbar;
        public List<GUILevelBarController> levelBars;
        public GUIImage bestScoreTab;
        public GUILabel bestScoreText;
        public GUIObject bestScoreButton;
        public GUIObject downloadNewLevelBar;
        public GUIObject applyDownloadResultButton;
        public GUILabel downloadResultText;

        private int _levelCount;

        private bool _canPlayerClickSomething = true;

        //===================================================================================

        private void Awake()
        {
            if(levelController)
            {
                if(scoreText != null)
                {
                    levelController.OnLevelScoreChanged += OnLevelScoreChanged;
                }

                if(remainingMoveCountText != null)
                {
                    levelController.OnRemainingMoveCountChanged += OnRemainingMoveCountChanged;
                }
            }
            
            if(levelsScrollbar != null)
            {
                _levelCount = 0;

                for(int i = 0; i < GameController.Instance.allLevelDatas.Count; i++)
                {
                    _levelCount++;
                    AddLevelBarToMenu(GameController.Instance.allLevelDatas[i]);
                }

                levelsScrollbar.minOffset = (int)(-1f * (_levelCount - 1) * 0.15f * Screen.height);
                levelsScrollbar.currentOffset = Mathf.Clamp((int)(-1f * (GameController.Instance.maxAchievedLevel - 1) * 0.15f * Screen.height), levelsScrollbar.minOffset, levelsScrollbar.maxOffset);
                
                GameController.Instance.OnNewLevelAddedToAllLevelDatas += OnNewLevelAddedToAllLevelDatas;
            }
            

            GameController.Instance.OnScreenResolutionChanged += OnScreenResolutionChanged;

            if(levelsScrollbar != null)
            {
                if(!GameController.Instance.gameStartedNew)
                {
                    ShowLevelScrollbar();
                }
                
                if(GameController.Instance.lastLevelWasBestScore)
                {
                    _canPlayerClickSomething = false;
                    bestScoreText.text = BestScoreText + GameController.Instance.lastlevelBestScore;
                    bestScoreTab.gameObject.SetActive(true);
                    levelsScrollbar.gameObject.SetActive(false);
                }

            }

            if(downloadNewLevelBar != null)
            {
                GameController.Instance.OnDownloadNewLevelResult += OnDownloadNewLevelResult;
            }

            
        }

        //===================================================================================

        private void OnDestroy()
        {
            if(levelController)
            {
                if(scoreText != null)
                {
                    levelController.OnLevelScoreChanged -= OnLevelScoreChanged;
                }

                if(remainingMoveCountText != null)
                {
                    levelController.OnRemainingMoveCountChanged -= OnRemainingMoveCountChanged;
                }
            }
            if(levelsScrollbar != null)
            {
                GameController.Instance.OnNewLevelAddedToAllLevelDatas -= OnNewLevelAddedToAllLevelDatas;
            }

            if(downloadNewLevelBar != null)
            {
                GameController.Instance.OnDownloadNewLevelResult -= OnDownloadNewLevelResult;
            }

            GameController.Instance.OnScreenResolutionChanged -= OnScreenResolutionChanged;
        }

        //===================================================================================

        private void Update()
        {
            
            bool pointerDown = Input.GetMouseButtonDown(0);
            bool pointerHold = Input.GetMouseButton(0);
            bool pointerUp = Input.GetMouseButtonUp(0);

            if(pointerDown || pointerHold || pointerUp)
            {
                GUIObject[] guiObjects = GetComponentsInChildren<GUIObject>();
                foreach(GUIObject guiObject in guiObjects)
                {
                    if(guiObject.isClickable && (_canPlayerClickSomething || guiObject == bestScoreButton || guiObject == applyDownloadResultButton))
                    {
                        IClickable clickable = guiObject.GetComponent<IClickable>();
                        if(clickable != null)
                        {
                            Rect realRect = guiObject.GetProcessedRect();
                            Vector2 pointerPosition = Input.mousePosition;
                            pointerPosition.y = Screen.height - pointerPosition.y;
                            if(realRect.Contains(pointerPosition))
                            {
                                if(pointerDown)
                                {
                                    clickable.OnPointerDown();
                                }
                                else if(pointerUp)
                                {
                                    if(clickable.IsClickedLastFrame())
                                    {
                                        clickable.OnPointerUp();
                                    }
                                }
                            }
                            else
                            {
                                if(pointerHold)
                                {
                                    if(clickable.IsClickedLastFrame())
                                    {
                                        clickable.OnPointerExit();
                                    }
                                }
                            }
                        }
                    }
                    
                }  
            }
        }

        //===================================================================================

        private void OnGUI()
        {

            GUIObject[] guiObjects = GetComponentsInChildren<GUIObject>();
            
            foreach(GUIObject guiObject in guiObjects)
            {
                if(guiObject.followingScrollbar == null)
                {
                    guiObject.Draw();
                }
                else
                {
                    guiObject.DrawInScrollbar();
                }
                
            }    
        }

        //===================================================================================

        private void OnLevelScoreChanged(int newScore)
        {
            scoreText.text = newScore.ToString();
        }

        //===================================================================================

        private void OnRemainingMoveCountChanged(int newRemainingMoveCount)
        {
            remainingMoveCountText.text = newRemainingMoveCount.ToString();
        }

        //===================================================================================

        public void OnStartButtonClicked()
        {
            GameController.Instance.LoadLevelFromMenu(5);
        }

        //===================================================================================

        public void OnLevelsButtonClicked()
        {
            ShowLevelScrollbar();
        }

        //===================================================================================

        public void ShowLevelScrollbar()
        {
            levelsButton.gameObject.SetActive(false);
            levelsScrollbar.gameObject.SetActive(true);
        }

        //===================================================================================

        private void OnScreenResolutionChanged(Vector2 newResolution)
        {
            if(levelsScrollbar != null)
            {
                levelsScrollbar.minOffset = (int)(-1f * (_levelCount - 1) * 0.15f * Screen.height);

                levelsScrollbar.currentOffset = Mathf.Clamp(levelsScrollbar.currentOffset, levelsScrollbar.minOffset, levelsScrollbar.maxOffset);
            }
        }

        //===================================================================================

        public void HideBestScoreTab()
        {
            _canPlayerClickSomething = true;
            bestScoreTab.gameObject.SetActive(false);
            levelsScrollbar.gameObject.SetActive(true);
        }

        //===================================================================================

        public void AddLevelBarToMenu(LevelData levelDataToAdd)
        {
            GUILevelBarController levelBar = Instantiate(GameController.Instance.levelBarGUIPrefab, levelsScrollbar.transform).GetComponent<GUILevelBarController>();
            levelBar.backgroundImage.anchoredPosition.y = 0.45f - ((levelDataToAdd.levelIndex - 1) * 0.15f);
            levelBar.backgroundImage.followingScrollbar = levelsScrollbar;
            levelBar.levelBarLevelIndex = (levelDataToAdd.levelIndex - 1);
            levelBar.levelAndMoveCountText.text = "Level " + levelDataToAdd.levelIndex + " - " + levelDataToAdd.maxMoveCount + " Moves";
            bool isLevelPlayable = GameController.Instance.maxAchievedLevel >= levelDataToAdd.levelIndex - 1;
            if(isLevelPlayable)
            {
                levelBar.backgroundImage.color = GameController.Instance.playableLevelBarColor;
                levelBar.playButton.gameObject.SetActive(true);
                levelBar.lockedButton.gameObject.SetActive(false);

                int levelBestScore = PlayerPrefs.GetInt(GameController.PlayerPrefsBS + (levelBar.levelBarLevelIndex + 1));
                if(levelBestScore == 0)
                {
                    levelBar.bestScoreText.text = noScoreText;
                }
                else
                {
                    levelBar.bestScoreText.text = highestScoreText + levelBestScore;
                }
            }
            else
            {
                levelBar.backgroundImage.color = GameController.Instance.unplayableLevelBarColor;
                levelBar.playButton.gameObject.SetActive(false);
                levelBar.lockedButton.gameObject.SetActive(true);
                levelBar.bestScoreText.text = noScoreText;
            }
        }

        //===================================================================================

        private void OnNewLevelAddedToAllLevelDatas(LevelData newLevelData)
        {
            AddLevelBarToMenu(newLevelData);
        }

        //===================================================================================

        public void OnClickedOnDownloadNewLevelsButton()
        {
            _canPlayerClickSomething = false;
            applyDownloadResultButton.gameObject.SetActive(false);
            downloadNewLevelBar.gameObject.SetActive(true);
            downloadResultText.text = "Checking for updates..";

            GameController.Instance.DownloadNewLevel();
        }

        //===================================================================================

        public void OnClickedApplyDownloadResultButton()
        {
            _canPlayerClickSomething = true;
            downloadNewLevelBar.gameObject.SetActive(false);
        }

        //===================================================================================

        public void OnDownloadNewLevelResult(string resultText)
        {
            applyDownloadResultButton.gameObject.SetActive(true);
            downloadResultText.text = resultText;
        }

        //===================================================================================

    }
}

