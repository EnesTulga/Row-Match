using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROWMATCH
{
    public class LevelData : System.ICloneable
    {
        //===================================================================================

        public const string levelDataTextFileHeaderMoveCount = "move_count";
        public const string levelDataTextFileHeaderWidth = "grid_width";
        public const string levelDataTextFileHeaderHeight = "grid_height";
        public const string levelDataTextFileHeaderLevelNumber = "level_number";
        public const string levelDataTextFileHeaderGrid = "grid";

        public const char lineBreak = '\n';
        public static readonly char[] headerSeparator = {':', ' '};
        public const char comma = ',';

        //===================================================================================

        public static LevelData CreateLevelDataFromTextFile(TextAsset textFile)
        {
            string levelDataText = textFile.text;
            string[] levelDataTextLines = levelDataText.Split(lineBreak);
            int lineCount = levelDataTextLines.Length;
            
            int levelNumber = 0;
            int levelWidth = 0;
            int levelHeight = 0;
            int levelMoveCount = 0;
            List<int> itemIDs = new List<int>();

            for(int i = 0; i < lineCount; i++)
            {
                if(levelDataTextLines[i].Contains(levelDataTextFileHeaderLevelNumber))
                {
                    string[] levelNumberData = levelDataTextLines[i].Split(headerSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    if(levelNumberData.Length > 1)
                    {
                        levelNumber = int.Parse(levelNumberData[1]);
                    }
                }
                else if(levelDataTextLines[i].Contains(levelDataTextFileHeaderWidth))
                {
                    string[] levelWidthData = levelDataTextLines[i].Split(headerSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    if(levelWidthData.Length > 1)
                    {
                        levelWidth = int.Parse(levelWidthData[1]);
                    }
                }
                else if(levelDataTextLines[i].Contains(levelDataTextFileHeaderHeight))
                {
                    string[] levelHeightData = levelDataTextLines[i].Split(headerSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    if(levelHeightData.Length > 1)
                    {
                        levelHeight = int.Parse(levelHeightData[1]);
                    }
                }
                else if(levelDataTextLines[i].Contains(levelDataTextFileHeaderMoveCount))
                {
                    string[] levelMoveCountData = levelDataTextLines[i].Split(headerSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    if(levelMoveCountData.Length > 1)
                    {
                        levelMoveCount = int.Parse(levelMoveCountData[1]);
                    }
                }
                else if(levelDataTextLines[i].Contains(levelDataTextFileHeaderGrid))
                {
                    string[] levelItemsData = levelDataTextLines[i].Split(headerSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    if(levelItemsData.Length > 1)
                    {
                        string[] ids = levelItemsData[1].Split(comma);
                        int idCount = ids.Length;
                        
                        for(int j = 0; j < idCount; j++)
                        {
                            if(ids[j].Equals("r"))
                            {
                                itemIDs.Add(0);
                            }
                            else if(ids[j].Equals("g"))
                            {
                                itemIDs.Add(1);
                            }
                            else if(ids[j].Equals("y"))
                            {
                                itemIDs.Add(2);
                            }
                            else if(ids[j].Equals("b"))
                            {
                                itemIDs.Add(3);
                            }
                        }
                    }
                }
            }

            if(levelNumber != 0 && levelWidth != 0 && levelHeight != 0 & levelMoveCount != 0 && itemIDs.Count != 0)
            {
                LevelData levelData = new LevelData(levelWidth, levelHeight, levelNumber, levelMoveCount, itemIDs);

                return levelData;
            }

            Debug.Log("LevelNumber : " + levelNumber);
            Debug.Log("levelWidth : " + levelWidth);
            Debug.Log("levelHeight : " + levelHeight);
            Debug.Log("levelMoveCount : " + levelMoveCount);
            Debug.Log("itemIDs : " + itemIDs.Count);

            return null;
        }

        //===================================================================================

        public int width, height;
        public int levelIndex;
        public int maxMoveCount;
        public List<int> slotItemIDs;
        public SlotItem[,] slotItems;

        //===================================================================================

        public LevelData(int width, int height, int levelIndex, int maxMoveCount, List<int> slotItemIDs)
        {
            this.width = width;
            this.height = height;
            this.levelIndex = levelIndex;
            this.maxMoveCount = maxMoveCount;
            this.slotItemIDs = slotItemIDs;

            slotItems = new SlotItem[width, height];

        }

        //===================================================================================

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //===================================================================================
    }
}

