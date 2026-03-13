namespace _Project.MiniGame.Core
{
    public readonly struct MiniGameResult
    {
        public MiniGameResult(MiniGameOutcomeType outcome, RewardData reward)
        {
            Outcome = outcome;
            Reward = reward;
        }
        
        public RewardData Reward { get; }
        public MiniGameOutcomeType Outcome { get; }
    }
}