using UnityEngine;

namespace PixelCrew.Model.Definitions
{
    public class DefRepository<TDefType> : ScriptableObject where TDefType : IHaveId
    {
        [SerializeField] protected TDefType[] _collection;
        public TDefType Get(string id)
        {
            foreach (var itemDef in _collection)
            {
                if (itemDef.Id == id)
                    return itemDef;
            }

            return default;
        }
    }

    public interface IHaveId
    {
        string Id { get; }
    }
}

