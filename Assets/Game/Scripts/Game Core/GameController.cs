using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class GameController : MonoBehaviour
    {
        //===================================================================================

        public static GameController Instance;

        public const string MenuSceneName = "MenuScene";
        public const string LevelSceneName = "LevelScene";
        public const string PlayerPrefsLevel = "Level";
        public const string PlayerPrefsBS = "BS";

        //===================================================================================

        public delegate void OnScreenResolutionChangedDelegate(Vector2 newResolution);
        public event OnScreenResolutionChangedDelegate OnScreenResolutionChanged;

        public delegate void OnNewLevelAddedToAllLevelDatasDelegate(LevelData newLevelData);
        public event OnNewLevelAddedToAllLevelDatasDelegate OnNewLevelAddedToAllLevelDatas;

        public delegate void OnDownloadNewLevelResultDelegate(string resultText);
        public event OnDownloadNewLevelResultDelegate OnDownloadNewLevelResult;

        //===================================================================================
        
        public Camera mainCamera;
        private Vector2Int _currentResolution;
        public List<GameObject> itemPrefabs;
        public LevelController currentLevelController;
        public float maxSlidingTime = 0.5f;
        public float waitingToCompleteLevelTime = 1f;

        public List<Color> itemBreakingFXColors;
        public Color playableLevelBarColor, unplayableLevelBarColor;
        public AudioSource itemVoice;
        public AudioClip itemBreakingVoice, itemSlideVoice;

        public int maxAchievedLevel;
        public int chosenLevelIndex;

        public List<TextAsset> levelTextAssets;
        public List<LevelData> allLevelDatas;

        public GameObject levelBarGUIPrefab;

        public bool gameStartedNew = true;
        public bool lastLevelWasBestScore = false;
        public int lastlevelBestScore = 0;

        public string downloadLevelURL;
        

        //===================================================================================

        void Awake()
        {
            if(!Instance)
            {
                Instance = this;
            }

            _currentResolution = new Vector2Int(Screen.width, Screen.height);

            maxAchievedLevel = PlayerPrefs.GetInt(PlayerPrefsLevel);

            allLevelDatas = new List<LevelData>();

            for(int i = 0; i < levelTextAssets.Count; i++)
            {
                allLevelDatas.Add(LevelData.CreateLevelDataFromTextFile(levelTextAssets[i]));
            }

            int index = levelTextAssets.Count + 1;

            while(System.IO.File.Exists(Application.persistentDataPath + "/" + index + ".txt"))
            {
                string levelAllText = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + index + ".txt");
                LevelData levelData = LevelData.CreateLevelDataFromTextFile(new TextAsset(levelAllText));

                if(levelData != null)
                {
                    allLevelDatas.Add(levelData);
                    index++;
                }
                else
                {
                    break;
                }
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(MenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        }

        //===================================================================================

        private void Update()
        {
            if(_currentResolution.x != Screen.width && _currentResolution.y != Screen.currentResolution.height)
            {
                _currentResolution = new Vector2Int(Screen.width, Screen.height);
                OnScreenResolutionChanged?.Invoke(_currentResolution);
            } 
        }

        //===================================================================================

        public void LoadMenuFromLevel(int levelScore, bool bestScore = false)
        {
            gameStartedNew = false;
            lastlevelBestScore = levelScore;
            lastLevelWasBestScore = bestScore;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(LevelSceneName);
            UnityEngine.SceneManagement.SceneManager.LoadScene(MenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        //===================================================================================

        public void LoadLevelFromMenu(int levelIndex)
        {
            if(levelIndex <= allLevelDatas.Count)
            {
                chosenLevelIndex = levelIndex;
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(MenuSceneName);
                UnityEngine.SceneManagement.SceneManager.LoadScene(LevelSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
        }

        //===================================================================================

        public void CompleteLevel()
        {
            int currentScore = currentLevelController.GetScore();
            
            if(currentScore != 0 && currentLevelController.currentLevelData.levelIndex > maxAchievedLevel)
            {
                maxAchievedLevel = currentLevelController.currentLevelData.levelIndex;
                PlayerPrefs.SetInt(PlayerPrefsLevel, maxAchievedLevel);
                
            }
            int levelBestScore = PlayerPrefs.GetInt(PlayerPrefsBS + currentLevelController.currentLevelData.levelIndex);
            
            bool bestScore = levelBestScore < currentScore;
            if(bestScore)
            {
                PlayerPrefs.SetInt(PlayerPrefsBS + currentLevelController.currentLevelData.levelIndex, currentScore);
            }

            StartCoroutine(CompleteLevelIEnumerator(bestScore));
        }

        //===================================================================================

        public IEnumerator CompleteLevelIEnumerator(bool bestScore = false)
        {
            yield return new WaitForSeconds(waitingToCompleteLevelTime);

            LoadMenuFromLevel(currentLevelController.GetScore(), bestScore);
            
        }

        //===================================================================================

        IEnumerator StartToDownloadingLevel(string url)
        {
            using (UnityEngine.Networking.UnityWebRequest downloadingWebRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
            {
                yield return downloadingWebRequest.SendWebRequest();
                string resultText;
                if (downloadingWebRequest.isNetworkError || downloadingWebRequest.isHttpError)
                {
                    //Debug.Log(downloadingWebRequest.error);
                    resultText = "Connection error!";
                }
                else
                {
                    LevelData levelData = LevelData.CreateLevelDataFromTextFile(new TextAsset(downloadingWebRequest.downloadHandler.text));
                    if(levelData != null)
                    {
                        int nextLevelIndex = allLevelDatas[allLevelDatas.Count - 1].levelIndex + 1;
                        if(nextLevelIndex == levelData.levelIndex)
                        {
                            string savePath = string.Format("{0}/{1}.txt", Application.persistentDataPath, levelData.levelIndex.ToString());        
                            System.IO.File.WriteAllText(savePath, downloadingWebRequest.downloadHandler.text);
                            allLevelDatas.Add(levelData);
                            OnNewLevelAddedToAllLevelDatas?.Invoke(levelData);

                            resultText = "Level " + nextLevelIndex + " is downloaded!";
                            
                        }
                        else
                        {
                            resultText = "You should download level " + nextLevelIndex + " first!";
                        }
                        
                    }
                    else
                    {
                        resultText = "File corrupted!";
                    }
                }

                OnDownloadNewLevelResult?.Invoke(resultText);
            }
        }

        //===================================================================================

        public void DownloadNewLevel()
        {
            StartCoroutine(StartToDownloadingLevel(downloadLevelURL));
        }

        //===================================================================================


    }
}

