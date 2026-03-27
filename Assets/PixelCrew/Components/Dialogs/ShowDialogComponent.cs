using PixelCrew.Model.Data;
using PixelCrew.Model.Definitions;
using PixelCrew.Model.Definitions.Localization;
using PixelCrew.UI.Hud.Dialogs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Components.Dialogs
{
    public class ShowDialogComponent : MonoBehaviour
    {
        [SerializeField] private Mode _mode;
        [SerializeField] private DialogData _bound;
        [SerializeField] private DialogDef _external;

        private string[] _localized;

        private DialogBoxController _dialogbox;

        private void Awake()
        {
            LocalizationManager.I.OnLocaleChanged += OnLocaleChanged;
            Localize();
        }

        private void Localize()
        {
            string[] separator = {"##"};

            _localized = LocalizationManager.I.Localize(Data.LocalizeKey)
                .Split(separator, StringSplitOptions.RemoveEmptyEntries);

            switch (_mode)
            {
                case Mode.Bound:
                    //_bound.Sentences = _localized;
                    break;

                case Mode.External:
                    //_external.Data.Sentences = _localized;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnLocaleChanged()
        {
            Localize();
        }

        private void OnDestroy()
        {
            LocalizationManager.I.OnLocaleChanged -= OnLocaleChanged;
        }

        public void Show()
        {
            _dialogbox = FindDialogController();

            _dialogbox.ShowDialog(Data);
        }

        private DialogBoxController FindDialogController()
        {
            if (_dialogbox != null) return _dialogbox;

            GameObject controllerGo = null;

            switch (Data.Type)
            {
                case DialogType.Simple:
                    controllerGo = GameObject.FindWithTag("SimpleDialog");
                    break;
                case DialogType.Personalized:
                    controllerGo = GameObject.FindWithTag("PersonalizedDialog");
                    break;
                default:
                    throw new ArgumentException("Undefined dialog type");
            }

            return controllerGo.GetComponent<DialogBoxController>(); ;
        }

        public void Show(DialogDef def)
        {
            _external = def;
            Localize();
            Show();
        }

        public DialogData Data
        {
            get
            {
                switch (_mode)
                {
                    case Mode.Bound:
                        return _bound;

                    case Mode.External:
                        return _external.Data;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public enum Mode
        {
            Bound,
            External
        }
    }
}

