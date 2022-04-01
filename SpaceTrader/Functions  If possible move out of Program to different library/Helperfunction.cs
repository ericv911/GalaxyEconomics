namespace SpaceTrader
{
    public static class Helperfunction
    {
        public static string IntToLetters(int value)
        {
            string result = string.Empty;
            while (--value >= 0)
            {
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }
            return result;
        }
        //public static string RandomString(FastRandom rand, int size, bool lowerCase)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    char ch;
        //    for (int i = 1; i < size + 1; i++)
        //    {
        //        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rand.NextDouble() + 65)));
        //        builder.Append(ch);
        //    }
        //    if (lowerCase)
        //        return builder.ToString().ToLower();
        //    else
        //        return builder.ToString();
        //}
    }
}
