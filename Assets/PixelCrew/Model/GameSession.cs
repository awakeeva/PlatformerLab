using UnityEngine;
using PixelCrew.Model.Data;
using UnityEngine.SceneManagement;
using PixelCrew.Utils.Disposables;

namespace PixelCrew.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;

        public PlayerData Data => _data;
        private PlayerData _save;
        private readonly CompositeDisposable _trash = new CompositeDisposable();

        public QuickInventoryModel QuickInventory { get; private set; }

        public void Save()
        {
            _save = _data.Clone();
        }

        public void LoadLastSave()
        {
            _data = _save.Clone();
        }

        private void Awake()
        {
            LoadHud();

            if (IsSessionExist())
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                Save();
                InitModels();
            }
        }

        private void InitModels()
        {
            QuickInventory = new QuickInventoryModel(Data);
            _trash.Retain(QuickInventory);
        }

        private void LoadHud()
        {
            SceneManager.LoadScene("Hud", LoadSceneMode.Additive);
        }

        private bool IsSessionExist()
        {
            var sessions = FindObjectsOfType<GameSession>();

            foreach (var gameSession in sessions)
            {
                if (gameSession != this)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}

