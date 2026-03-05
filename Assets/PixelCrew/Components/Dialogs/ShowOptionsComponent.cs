using PixelCrew.UI.Hud.Dialogs;
using UnityEngine;

namespace PixelCrew.Components.Dialogs
{
    public class ShowOptionsComponent : MonoBehaviour
    {
        [SerializeField] private OptionDialogData _data;

        private OptionDialogController _dialogbox;


        public void Show()
        {
            if (_dialogbox == null)
            {
                _dialogbox = FindObjectOfType<OptionDialogController>();
            }

            _dialogbox.Show(_data);
        }
    }
}

