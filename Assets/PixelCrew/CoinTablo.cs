using UnityEngine;

namespace PixelCrew
{
    public class CoinTablo : MonoBehaviour
    {
        private const int SilverCoinCost = 1;
        private const int GoldCoinCost = 10;

        private static int _silverCoinCount = 0;
        private static int _goldCoinCount = 0;

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
