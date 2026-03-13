namespace _Project.MiniGame.Core
{
    public readonly struct RewardData
    {
        public RewardData(int goldAmount)
        {
            GoldAmount = goldAmount;
        }

        public int GoldAmount { get; }
    }
}