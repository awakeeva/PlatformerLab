using PixelCrew.Model.Data;
using PixelCrew.Model.Definitions;
using PixelCrew.Utils.Disposables;
using System;
using UnityEngine;

namespace PixelCrew.Model.Models
{
    public class StatsModel : IDisposable
    {
        private readonly PlayerData _data;

        public StatsModel(PlayerData data)
        {
            _data = data;
        }

        public event Action OnChanged;

        private CompositeDisposable _trash = new CompositeDisposable();
        public IDisposable Subscribe(Action call)
        {
            OnChanged += call;
            return
                new ActionDisposable(() => OnChanged -= call);
        }

        public void LevelUp(StatId id)
        {
            var def = DefsFacade.I.Player.GetStat(id);
            
            var nextLevel = GetLevel(id) + 1;
            if (def.Levels.Length >= nextLevel)
                return;
            
            var price = def.Levels[nextLevel].Price;
            if(!_data.Inventory.IsEnough(price))
                return;

            _data.Inventory.Remove(price.ItemId, price.Count);
            _data.Levels.LevelUp(id);

            OnChanged?.Invoke();
        }

        public float GetValue(StatId id)
        {
            var def = DefsFacade.I.Player.GetStat(id);
            var level = def.Levels[GetLevel(id)];
            return level.Value;
        }

        public int GetLevel(StatId id) => _data.Levels.GetLevel(id);

        public void Dispose()
        {
            _trash.Dispose();
        }
    }
}

