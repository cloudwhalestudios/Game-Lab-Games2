public class TopScoreCounter : ScoreCounter
{
    protected override int Value
    {
        get { return UserProgress.Current.TopScore; }
    }
}