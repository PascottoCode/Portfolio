using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : Singleton<SavingSystem>
    {
        [SerializeField] private SOReferenceCache sOReferenceCache;

        public void Save(string saveFile)
        {
            CreateSaveBackup(saveFile);
            
            var state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }
        private void CreateSaveBackup(string saveFile)
        {
            var saveFilePath = GetPathFromSaveFile(saveFile);
            if(!File.Exists(saveFilePath)) { return; }

            var backupFilePath = GetPathFromSaveFile(saveFile + "_Backup");
            File.Copy(saveFilePath,backupFilePath, true);
        }
        public void SaveOver(string save, string saveOver)
        {
            var state = LoadFile(save);
            CaptureState(state);
            SaveFile(saveOver, state);
        }
        public bool Load(string saveFile, bool isBackup = false)
        {
            var stateDictionary = LoadFile(saveFile);
            if (stateDictionary.IsNullOrEmpty())
            {
                return false;
            }
            
            if (RestoreState(stateDictionary))
            {
                return true;
            }
            
            if (!isBackup && File.Exists(GetPathFromSaveFile(saveFile+"_Backup")))
            {
                return Load(saveFile + "_Backup", true);
            }
            else
            {
                Debug.LogError("Failed to load save file, crashing to desktop!");
                Application.Quit();
                return false;
            }
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
            File.Delete(GetPathFromSaveFile(saveFile+"_Backup"));
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            var path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                print("Save file does not exist, creating new dictionary: " + path);
                return new Dictionary<string, object>();
            }
            
            print("Loading from: " + path);

            var bytes = File.ReadAllBytes(path);

            if (bytes.IsNullOrEmpty()) { return new Dictionary<string, object>(); }
        
            //Deserialize byte array
            var dictionary = SerializationUtility.DeserializeValue<Dictionary<string, object>>(bytes, DataFormat.Binary,
                new DeserializationContext { StringReferenceResolver = sOReferenceCache });
            
            return dictionary;
        }

        private void SaveFile(string saveFile, object state)
        {
            var path = GetPathFromSaveFile(saveFile);
            print("Saving to: " + path);
            
            //Serialize byte array
            var serializedData = SerializationUtility.SerializeValue(state, DataFormat.Binary,
                new SerializationContext { StringReferenceResolver = sOReferenceCache });

            File.WriteAllBytes(path, serializedData);
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private bool RestoreState(Dictionary<string, object> state)
        {
            try
            {
                foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
                {
                    var id = saveable.GetUniqueIdentifier();
                    if (state.ContainsKey(id))
                    {
                        saveable.RestoreState(state[id]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Save exception!" + e.Message);
                return false;
            }

            return true;
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}