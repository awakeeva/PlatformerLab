using PixelCrew.Utils;
using PixelCrew.Utils.Disposables;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.UI.Widgets
{
    public class ProgressBarWidget : MonoBehaviour
    {
        [SerializeField] private Image _bar;

        private readonly CompositeDisposable _cooldownTrash = new CompositeDisposable();

        private IEnumerator TrackingProgress(Cooldown cooldown)
        {
            while (cooldown.TimeLasts >0)
            {
                this.SetProgress(cooldown.TimeLasts / cooldown.Value);
                yield return new WaitForSeconds(0.02f);
            };

            this.SetProgress(0f);
        }

        [ContextMenu("TestProgress")]
        public void TestProgress()
        {
            Cooldown cooldown = new Cooldown();
            cooldown.Value = 5f;
            cooldown.Reset();
            OnResetCooldown(cooldown);
        }

        public void OnResetCooldown(Cooldown cooldown)
        {
            StartCoroutine(TrackingProgress(cooldown));
        }

        public void SetProgress(float progress)
        {
            _bar.fillAmount = progress;
        }
    }
}

