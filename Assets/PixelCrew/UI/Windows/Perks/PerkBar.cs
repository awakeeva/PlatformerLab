using PixelCrew.Model;
using PixelCrew.Model.Definitions;
using PixelCrew.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.UI.Windows.Perks
{
    public class PerkBar : MonoBehaviour, IItemRenderer<PerkDef>
    {
        [SerializeField] private Image _icon;

        private GameSession _session;
        private PerkDef _data;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            UpdateView();
        }

        public void SetData(PerkDef data, int index)
        {
            _data = data;

            if (_session != null)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            _icon.sprite = _data.Icon;
        }
    }
}

