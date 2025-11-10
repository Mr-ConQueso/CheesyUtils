using UnityEngine;
using UnityEngine.SceneManagement;

namespace CheesyUtils.SavingLoading
{
    public class SavedVariables : MonoBehaviour, ISaveable
    {
        public static string consoleHistory;
        public static bool isFirstTimePlaying = true;
        public static string previousScene;
        public static bool wasGamePaused = false;
        public static int currentSaveSlot = 1;
        public static int lastSceneBuildIndex = 0;
        
        public object CaptureState()
        {
            return new SaveData()
            {
                consoleHistory = consoleHistory,
                isFirstTimePlaying = isFirstTimePlaying,
                previousScene = previousScene,
                wasGamePaused = wasGamePaused,
                saveSlot = currentSaveSlot,
                lastSceneBuildIndex = SceneManager.GetActiveScene().buildIndex
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            
            // Misc
            consoleHistory = saveData.consoleHistory;
            isFirstTimePlaying = saveData.isFirstTimePlaying;
            previousScene = saveData.previousScene;
            wasGamePaused = saveData.wasGamePaused;
            currentSaveSlot = saveData.saveSlot;
            lastSceneBuildIndex = saveData.lastSceneBuildIndex;
        }
        
        private struct SaveData
        {
            public string consoleHistory;
            public bool isFirstTimePlaying;
            public string previousScene;
            public bool wasGamePaused;
            public int saveSlot;
            public int lastSceneBuildIndex;
        }
    }
}