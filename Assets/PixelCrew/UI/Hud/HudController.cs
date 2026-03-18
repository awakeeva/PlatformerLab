using PixelCrew.Model;
using PixelCrew.Model.Definitions;
using PixelCrew.UI.Widgets;
using PixelCrew.UI.Windows.Perks;
using PixelCrew.Utils;
using System;
using UnityEngine;

namespace PixelCrew.UI.Hud
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private ProgressBarWidget _healthBar;
        [SerializeField] private PerkBar _perkBar;

        private GameSession _session;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            _session.Data.Hp.OnChanged += OnHealthChanged;
            OnHealthChanged(_session.Data.Hp.Value, 0);

            _session.PerksModel.OnChanged += OnPerkChanged;
            OnPerkChanged();
        }

        private void OnPerkChanged()
        {
            if (!string.IsNullOrEmpty(_session.PerksModel.Used))
            {
                _perkBar.gameObject.SetActive(true);
                var def = DefsFacade.I.Perks.Get(_session.PerksModel.Used);
                _perkBar.SetData(def, default);
            }
            else
            {
                _perkBar.gameObject.SetActive(false);
            }
        }

        private void OnHealthChanged(int newValue, int oldValue)
        {
            var maxHealth = DefsFacade.I.Player.MaxHealth;
            var value = (float)newValue / maxHealth;
            _healthBar.SetProgress(value);
        }

        public void OnSettings()
        {
            WindowUtils.CreateWindow("UI/InGameMenuWindow");
        }

        private void OnDestroy()
        {
            _session.Data.Hp.OnChanged -= OnHealthChanged;
            _session.PerksModel.OnChanged -= OnPerkChanged;
        }
    }
}

