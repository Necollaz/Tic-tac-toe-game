namespace _Project.MiniGame.Core
{
    public sealed class RewardService
    {
        private const int WIN_GOLD_AMOUNT = 100;
        private const int DRAW_GOLD_AMOUNT = 50;
        private const int LOSE_GOLD_AMOUNT = 0;

        public RewardData GenerateReward(MiniGameOutcomeType outcome)
        {
            switch (outcome)
            {
                case MiniGameOutcomeType.Win:
                    return new RewardData(WIN_GOLD_AMOUNT);

                case MiniGameOutcomeType.Draw:
                    return new RewardData(DRAW_GOLD_AMOUNT);

                default:
                    return new RewardData(LOSE_GOLD_AMOUNT);
            }
        }
    }
}