using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class LevelController : MonoBehaviour
    {
        //===================================================================================

        public enum LevelState
        {
            PreparingLevel = 0,
            WaitingForPlayerInput = 1,
            WaitingForSlidingSlots = 2,
            WaitingForAligningSlots = 3,
            LevelEnded = 4
        }

        //===================================================================================

        public delegate void OnLevelScoreChangedDelegate(int newScore);
        public event OnLevelScoreChangedDelegate OnLevelScoreChanged;
         public delegate void OnRemainingMoveCountChangedDelegate(int newRemainingMoveCount);
        public event OnRemainingMoveCountChangedDelegate OnRemainingMoveCountChanged;

        //===================================================================================

        public Camera mainCamera;
        public Transform slotItemsParent;
        public SpriteRenderer background;

        public LevelData currentLevelData;

        public int testWidth, testHeight;

        private float _stateWaitTimer = -1f;
        private int _completedRowCount = 0;

        private LevelState _currentLevelState = 0;
        private SlotItem _clickedSlotItem;
        private int _levelScore = 0;
        private int _leftMoveCount;
        private bool breakingHappenedThisSwap = false;
        private int[] leftItemCounts;

        //===================================================================================


        private void Awake()
        {
            GameController.Instance.currentLevelController = this;    
        }

        //===================================================================================

        void Start()
        {
            ChangeLevelState(LevelState.PreparingLevel);

            currentLevelData = (LevelData)GameController.Instance.allLevelDatas[GameController.Instance.chosenLevelIndex].Clone();
            if(currentLevelData == null)
            {
                Debug.LogError("Level can not be loaded!");
            }
            mainCamera = GameController.Instance.mainCamera;

            PrepareBackground();
            SetCameraRatioAccordingToBackground();
            AddItemsToCurrentLevel();

            ChangeLeftMoveCount(currentLevelData.maxMoveCount);

            GameController.Instance.OnScreenResolutionChanged += OnScreenSizeChanged;

            ChangeLevelState(LevelState.WaitingForPlayerInput);
        }

        //===================================================================================

        private void OnDestroy()
        {
            GameController.Instance.OnScreenResolutionChanged -= OnScreenSizeChanged;
        }

        //===================================================================================

        private void Update()
        {
            if(_stateWaitTimer > 0f)
            {
                _stateWaitTimer -= Time.deltaTime;
                
                if(_stateWaitTimer < 0f)
                {
                    if(_currentLevelState == LevelState.WaitingForSlidingSlots)
                    {
                        CheckRowsCompleted();
                    }
                    else if(_currentLevelState == LevelState.WaitingForAligningSlots)
                    {
                        ChangeLevelState(LevelState.WaitingForPlayerInput);
                    }
                }
            }
            else if(_currentLevelState == LevelState.WaitingForPlayerInput)
            {
                if(_clickedSlotItem == null)
                {
                    _clickedSlotItem = InputHandler.Instance.PlayerClickedOnSlotItem();
                }
                else
                {
                    if(Input.GetMouseButtonUp(0))
                    {
                        _clickedSlotItem = null;
                    }
                    else
                    {
                        SlotItem swappingItem = InputHandler.Instance.PlayerSlideOnSlotItem();

                        
                        if(swappingItem && swappingItem != _clickedSlotItem)
                        {
                            int xDistance = Mathf.Abs(_clickedSlotItem.gridPosition.x - swappingItem.gridPosition.x);
                            int yDistance = Mathf.Abs(_clickedSlotItem.gridPosition.y - swappingItem.gridPosition.y);

                            if((xDistance == 1 && yDistance == 0) || (xDistance == 0 && yDistance == 1))
                            {
                                if(SwapTwoItemSlots(_clickedSlotItem.gridPosition, swappingItem.gridPosition))
                                {
                                    ChangeLeftMoveCount(_leftMoveCount-1);
                                }
                                
                                _clickedSlotItem = null;
                            }                            
                            
                        }
                    }

                }
            }    
        }

        //===================================================================================

        public void PrepareBackground()
        {
            background.size = new Vector2(currentLevelData.width, currentLevelData.height);
        }

        //===================================================================================

        public void SetCameraRatioAccordingToBackground()
        {
            mainCamera.orthographicSize = Mathf.Max(3f, (Screen.width > Screen.height ? (currentLevelData.width < currentLevelData.height ? currentLevelData.width : currentLevelData.height) : currentLevelData.width) * 1.2f);
            //mainCamera.orthographicSize = tileRectSize * currentLevelData.width;
        }

        //===================================================================================

        public void OnScreenSizeChanged(Vector2 newResolution)
        {
            SetCameraRatioAccordingToBackground();
        }

        //===================================================================================

        public void ChangeLevelState(LevelState newState)
        {
            if(_currentLevelState != newState)
            {
                if(newState == LevelState.WaitingForPlayerInput)
                {
                    if(_leftMoveCount == 0 || _completedRowCount + 1 >= currentLevelData.height)
                    {
                        _currentLevelState = LevelState.LevelEnded;
                        GameController.Instance.CompleteLevel();
                    }
                    else if(breakingHappenedThisSwap)
                    {
                        bool isThereAnyMoreBreakableRow = false;
                        for(int i = 0; i < leftItemCounts.Length; i++)
                        {
                            if(leftItemCounts[i] >= currentLevelData.width)
                            {
                                isThereAnyMoreBreakableRow = true;
                                break;
                            }
                        }
                        
                        if(!isThereAnyMoreBreakableRow)
                        {
                            _currentLevelState = LevelState.LevelEnded;
                            GameController.Instance.CompleteLevel();
                        }
                    }

                    if(_currentLevelState != LevelState.LevelEnded)
                    {
                        _currentLevelState = newState;
                    }

                    breakingHappenedThisSwap = false;
                }
                else
                {
                    _currentLevelState = newState;
                }
            }
        }

        //===================================================================================

        public void AddItemsToCurrentLevel()
        {
            leftItemCounts = new int[GameController.Instance.itemPrefabs.Count];
            for(int i = 0; i < currentLevelData.width; i++)
            {
                for(int j = currentLevelData.height - 1; j >= 0; j--)
                {
                    int itemIndex = currentLevelData.slotItemIDs[i + ((currentLevelData.height - 1 - j) * currentLevelData.width)];
                    currentLevelData.slotItems[i,j] = Instantiate(GameController.Instance.itemPrefabs[itemIndex], slotItemsParent).GetComponent<SlotItem>();
                    leftItemCounts[itemIndex] = leftItemCounts[itemIndex] + 1;

                    currentLevelData.slotItems[i,j].InitializeItem(i, j);
                }
            }
        }

        //===================================================================================

        public Vector3 GetSlotItemWorldPosition(Vector2Int gridPosition)
        {
            Vector3 startingOffset = new Vector3( 0.5f - (currentLevelData.width / 2f), (currentLevelData.height / 2f) - 0.5f, 0f);

            return startingOffset + new Vector3(gridPosition.x, -gridPosition.y, 0f);
        }

        //===================================================================================

        /// <summary>
        /// Returns true if items have swapped.
        /// </summary>
        public bool SwapTwoItemSlots(Vector2Int swappingItem1, Vector2Int swappingItem2)
        {
            SlotItem item1 = currentLevelData.slotItems[swappingItem1.x, swappingItem1.y];
            SlotItem item2 = currentLevelData.slotItems[swappingItem2.x, swappingItem2.y];

            if(item1 && item2)
            {
                currentLevelData.slotItems[swappingItem1.x, swappingItem1.y] = item2;
                currentLevelData.slotItems[swappingItem2.x, swappingItem2.y] = item1;

                item1.SetNewGridIndex(swappingItem2.x, swappingItem2.y);
                item2.SetNewGridIndex(swappingItem1.x, swappingItem1.y);

                _stateWaitTimer = GameController.Instance.maxSlidingTime;
                ChangeLevelState(LevelState.WaitingForSlidingSlots);

                GameController.Instance.itemVoice.PlayOneShot(GameController.Instance.itemSlideVoice);

                return true;
            }

            return false;
        }

        //===================================================================================

        public void CheckRowsCompleted()
        {
            int completedRows = 0;

            for(int i = _completedRowCount; i < currentLevelData.height; i++)
            {
                if(!currentLevelData.slotItems[0, i])
                {
                    continue;
                }

                bool completed = true;
                int firstItemSlotType = currentLevelData.slotItems[0, i].itemID;
                for(int j = 1; j < currentLevelData.width; j++)
                {
                    if(firstItemSlotType != currentLevelData.slotItems[j, i].itemID)
                    {
                        completed = false;
                        break;
                    }
                }

                if(completed)
                {
                    completedRows++;
                    BreakRow(i);
                }
            }

            _completedRowCount += completedRows;
            if(completedRows > 0)
            {
                breakingHappenedThisSwap = true;
                AlignAllSlots();
            }
            else
            {
                ChangeLevelState(LevelState.WaitingForPlayerInput);
            }

            
        }

        //===================================================================================

        public void BreakRow(int rowIndex)
        {
            int gainedScoreInRow = 0;
            leftItemCounts[currentLevelData.slotItems[0, rowIndex].itemID] -= currentLevelData.width;
            for(int i = 0; i < currentLevelData.width; i++)
            {
                gainedScoreInRow += currentLevelData.slotItems[i, rowIndex].itemScore;
                FXController.Instance.PlayItemBreakingFX(currentLevelData.slotItems[i, rowIndex].transform.position, GameController.Instance.itemBreakingFXColors[currentLevelData.slotItems[i, rowIndex].itemID]);
                Destroy(currentLevelData.slotItems[i, rowIndex].gameObject);
                currentLevelData.slotItems[i, rowIndex] = null;
            }

            GainScore(gainedScoreInRow);
            GameController.Instance.itemVoice.PlayOneShot(GameController.Instance.itemBreakingVoice);
        }

        //===================================================================================

        public void AlignAllSlots()
        {
            bool alignedAny = false;
            for(int j = currentLevelData.height - 1; j >= 0; j--)
            {
                if(currentLevelData.slotItems[0,j])
                {
                    for(int k = currentLevelData.height - 1; k > j; k--)
                    {
                        if(currentLevelData.slotItems[0,k] == null)
                        {
                            alignedAny = true;
                            for(int i = 0; i < currentLevelData.width; i++)
                            {
                                currentLevelData.slotItems[i,j].SetNewGridIndex(i, k);
                                currentLevelData.slotItems[i, k] = currentLevelData.slotItems[i,j];
                                currentLevelData.slotItems[i,j] = null;
                            }
                            break;
                        }
                        
                    }
                }
            }

            if(alignedAny)
            {
                ChangeLevelState(LevelState.WaitingForAligningSlots);
                _stateWaitTimer = GameController.Instance.maxSlidingTime;
            }
            else
            {
               ChangeLevelState(LevelState.WaitingForPlayerInput);
            }
        }

        //===================================================================================

        public void GainScore(int additionalScore)
        {
            _levelScore += additionalScore;
            OnLevelScoreChanged?.Invoke(_levelScore);
        }

        //===================================================================================

        public int GetScore()
        {
            return _levelScore;
        }

        //===================================================================================

        public void ChangeLeftMoveCount(int newLeftMoveCount)
        {
            _leftMoveCount = newLeftMoveCount;
            OnRemainingMoveCountChanged?.Invoke(newLeftMoveCount);
        }

        //===================================================================================

    }
}

