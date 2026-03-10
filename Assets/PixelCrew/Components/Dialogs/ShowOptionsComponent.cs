using PixelCrew.Model.Definitions.Localization;
using PixelCrew.UI.Hud.Dialogs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Components.Dialogs
{
    public class ShowOptionsComponent : MonoBehaviour
    {
        [SerializeField] private OptionDialogData _data;

        private OptionDialogController _dialogbox;

        private void Awake()
        {
            LocalizationManager.I.OnLocaleChanged += OnLocaleChanged;
            Localize();
        }

        private void OnLocaleChanged()
        {
            Localize();
        }

        private void Localize()
        {
            _data.DialogText = LocalizationManager.I.Localize(_data.LocalizeKey);

            foreach (var option in _data.Options)
            {
                option.Text = LocalizationManager.I.Localize(option.LocalizeKey);
            }
        }

        public void Show()
        {
            if (_dialogbox == null)
            {
                _dialogbox = FindObjectOfType<OptionDialogController>();
            }

            Localize();
            _dialogbox.Show(_data);
        }
    }
}

