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
        [SerializeField] private ProgressBarWidget _perkCooldownProgressBar;

        private GameSession _session;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            _session.Data.Hp.OnChanged += OnHealthChanged;
            OnHealthChanged(_session.Data.Hp.Value, 0);

            _session.PerksModel.OnChanged += OnPerkChanged;
            OnPerkChanged();

            _session.PerksModel.OnCooldownReset += _perkCooldownProgressBar.OnResetCooldown;
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
            var maxHealth = _session.StatsModel.GetValue(StatId.Hp);
            var value = (float)newValue / maxHealth;
            _healthBar.SetProgress(value);
        }

        public void OnSettings()
        {
            WindowUtils.CreateWindow("UI/InGameMenuWindow");
        }

        public void OnStats()
        {
            WindowUtils.CreateWindow("UI/PlayerStatsWindow");
        }

        private void OnDestroy()
        {
            _session.Data.Hp.OnChanged -= OnHealthChanged;
            _session.PerksModel.OnChanged -= OnPerkChanged;
            _session.PerksModel.OnCooldownReset -= _perkCooldownProgressBar.OnResetCooldown;
        }
    }
}

