using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;

namespace CheesyUtils.SavingLoading
{
    public class SavingSystem : Singleton<SavingSystem>
    {
        private const string SaveFolder = "Saves";
        private const string SettingsFileExtension = ".json";
        
        [SerializeField] private string saveFileName = "SaveData";
        [SerializeField] private string saveFileExtension = ".sav";
        [SerializeField] private string settingsFileName = "Settings";
        
        [SerializeField] private SavedSettings _settings;
        [SerializeField] private SavedVariables _variables;

        private string _savePath;
        private string _settingsSavePath;

        protected override void Awake()
        {
            base.Awake();
            _savePath = Path.Combine(
                Application.persistentDataPath,
                SaveFolder
            );
            Directory.CreateDirectory(_savePath);
            
            _settingsSavePath = Path.Combine(
                Application.persistentDataPath,
                settingsFileName + SettingsFileExtension
            );
        }

        public void Save(int slot)
        {
            var state = LoadFile(GetSavePath(slot));
            CaptureState(state);
            SaveFile(state, GetSavePath(slot));
            SaveVariables();
            Debug.Log("Saved at Slot: " + slot, DLogType.System);
        }

        public void Load(int slot)
        {
            RestoreState(LoadFile(GetSavePath(slot)));
            Debug.Log("Loaded from Slot: " + slot, DLogType.System);
        }
        
        public bool DoesSaveExist(int slot)
        {
            return File.Exists(GetSavePath(slot));
        }
        
        public void DeleteSave(int slot)
        {
            if (!File.Exists(GetSavePath(slot))) return;
            File.Delete(GetSavePath(slot));
        }

        #region Settings
        public void SaveSettings()
        {
            SaveVariables();
            string settings = JsonConvert.SerializeObject(_settings.CaptureState(), Formatting.Indented);
            object state;
    
            if (File.Exists(_settingsSavePath))
            {
                state = File.ReadAllText(_settingsSavePath);
            }
            else
            {
                state = _settings.CaptureState();
            }
    
            // Merge new settings with existing settings if necessary
            // Assuming 'json' is already a JSON string, otherwise deserialize and merge here
    
            using (var streamWriter = new StreamWriter(_settingsSavePath))
            {
                streamWriter.Write(settings);
            }
        }
        public void LoadSettings()
        {
            if (!File.Exists(_settingsSavePath))
            {
                _settings.RestoreState(_settings.CaptureState());
                return;
            }

            string settings = File.ReadAllText(_settingsSavePath);
            _settings.RestoreState(settings);
        }
        #endregion
        
        #region Variables
        public void SaveVariables()
        {
            var state = LoadFile(GetSavePath(0));
            CaptureVariableState(state);
            SaveFile(state, GetSavePath(0));
        }
        public void LoadVariables()
        {
            RestoreVariableState(LoadFile(GetSavePath(0)));
        }
        #endregion
        
        #region Save Slots
        private Dictionary<string, object> LoadFile(string savePath)
        {
            if (!File.Exists(savePath))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(savePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(object state, string savePath)
        {
            using (var stream = File.Open(savePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }
        #endregion

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
            {
                state[saveable.Id] = saveable.CaptureState();
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
            {
                if (state.TryGetValue(saveable.Id, out object value))
                {
                    saveable.RestoreState(value);
                }
            }
        }
        
        private void CaptureVariableState(object state)
        {
            state = _variables.CaptureState();
        }

        private void RestoreVariableState(object state)
        {
            _variables.RestoreState(state);
        }

        private string GetSavePath(int slot)
        {
            string fileName = string.Concat(saveFileName, "_", slot.ToString("D2"), saveFileExtension);
            return Path.Combine(_savePath, fileName);
        }
    }
}
