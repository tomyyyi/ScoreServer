namespace ScoreServer.Model
{
    public class CustomerScore
    {
        public CustomerScore(long customerId, decimal score)
        {
            CustomerId = customerId;
            Score = score;
        }

        public long CustomerId { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }
    }
}
