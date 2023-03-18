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

            long xId = x.CustomerId;
            long yId = y.CustomerId;
            int xDigit, yDigit;
            for (int i = 0; i < sizeof(long); i++)
            {
                xDigit = GetDigit(xId, i);
                yDigit = GetDigit(yId, i);

                if (xDigit < yDigit)
                {
                    return -1;
                }
                else if (xDigit > yDigit)
                {
                    return 1;
                }
            }

            return 0;
        }

        private static int GetDigit(long num, int digitIndex)
        {
            long mask = (long)Math.Pow(10, digitIndex + 1);
            long digit = num % mask;
            digit /= (long)Math.Pow(10, digitIndex);
            return (int)digit;
        }
    }
}
