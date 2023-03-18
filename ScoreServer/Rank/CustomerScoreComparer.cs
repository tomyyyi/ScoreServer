using ScoreServer.Model;

namespace ScoreServer.Rank
{
    class CustomerScoreComparer : IComparer<CustomerScore>
    {
        public int Compare(CustomerScore x, CustomerScore y)
        {
            if (x.Score > y.Score)
            {
                return -1;
            }
            else if (x.Score < y.Score)
            {
                return 1;
            }

            if (x.CustomerId > y.CustomerId)
            {
                return 1;
            }
            else if (x.CustomerId < y.CustomerId)
            {
                return -1;
            }

            return 0;
        }
    }
}
