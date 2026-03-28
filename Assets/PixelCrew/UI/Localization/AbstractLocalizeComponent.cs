using PixelCrew.Model.Definitions.Localization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.UI.Localization
{
    public abstract class AbstractLocalizeComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            LocalizationManager.I.OnLocaleChanged += OnLocaleChanged;
            Localize();
        }

        private void OnLocaleChanged()
        {
            Localize();
        }

        protected abstract void Localize();

        private void OnDestroy()
        {
            LocalizationManager.I.OnLocaleChanged -= OnLocaleChanged;
        }
    }
}

