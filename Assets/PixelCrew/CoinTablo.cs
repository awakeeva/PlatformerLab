using UnityEngine;

namespace PixelCrew
{
    public class CoinTablo : MonoBehaviour
    {
        [SerializeField] private int SilverCoinCost;
        [SerializeField] private int GoldCoinCost;

        private int _silverCoinCount = 0;
        private int _goldCoinCount = 0;

        public void CollectSilverCoin()
        {
            _silverCoinCount++;
            ShowTablo();
        }
        public void CollectGoldCoin()
        {
            _goldCoinCount++;
            ShowTablo();
        }

        private void ShowTablo()
        {
            var total = _silverCoinCount * SilverCoinCost + _goldCoinCount * GoldCoinCost;
            Debug.Log($"SilverCount ={_silverCoinCount} GoldCount ={_goldCoinCount} TotalMoney ={total}");
        }
    }
}
