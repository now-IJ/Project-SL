using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RS
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        /// <summary>
        /// Before we create a new save file, we must check to see if one of this character slots alreadys exists (max 10 slots)
        /// </summary>
        /// <returns></returns>
        public bool CheckToSeeIfFileExists()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Used to delete character save files
        /// </summary>
        public void DeleteSaveFile()
        {
            Debug.Log(Path.Combine(saveDataDirectoryPath, saveFileName));
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        /// <summary>
        /// Used to create a save file upon starting a new game
        /// </summary>
        /// <param name="characterSaveData">Data that will be saved</param>
        public void CreateNewCharacterSaveFile(CharacterSaveData characterSaveData)
        {
            // Make a Path to sace the file on the machine
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // create the directory the file will be written to, if it does not already exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating Save file, at save path: " + savePath);

                // serialize the C# game data into JSON
                string dataToStore = JsonUtility.ToJson(characterSaveData, true);

                // Write the file to the system
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to save character data, GAME NOT SAVED!" + savePath + "\n" + ex);
            }
        }

        /// <summary>
        /// Used to load a save file upon loading a previous game
        /// </summary>
        /// <returns></returns>
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;
            
            // Make a Path to sace the file on the machine
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Deserialize the data from JSON back to Unity
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    
                }
            }

            return characterData;

        }
    }
}