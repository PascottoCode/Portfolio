using RPG.Control;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace RPG.Localization
{
    public class LocalizedText : SerializedMonoBehaviour
    {
        [SerializeField] private ILocalizationTable localizationTable = new LocalizationTable();
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            GameSession.onLanguageChanged += LocalizeText;
            LocalizeText(GameSession.Inst.Language);
        }

        private void OnDisable()
        {
            GameSession.onLanguageChanged -= LocalizeText;
        }

        private void LocalizeText(Language language)
        {
            if (_text == null) { return; }
            
            var localization = localizationTable.GetLocalization(language);

            if (!string.IsNullOrEmpty(localization))
            {
                _text.text = localization;
            }
        }

        [Button]
        private void LocalizeButton(Language language)
        {
            //editor only
            var localization = localizationTable.GetLocalization(language);

            if (!string.IsNullOrEmpty(localization))
            {
                GetComponent<TextMeshProUGUI>().text = localization;
            }
        }
    }
}