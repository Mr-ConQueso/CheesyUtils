using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace CheesyUtils.Localization
{
    public class LocaleSelector : MonoBehaviour
    {
        private bool _isActive = false;

        public void OnClick_SelectLocale(int localeIndex)
        {
            if (_isActive) return;
            StartCoroutine(SetLocale(localeIndex));
        }
        
        IEnumerator SetLocale(int localeIndex)
        {
            _isActive = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
            _isActive = false;
        }
    }
}