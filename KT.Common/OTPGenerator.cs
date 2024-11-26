
namespace KT.Common
{
    public static class OTPGenerator
    {
        public static string GenerateOTP(int otpLength)
        {
            char[] allowedCharacters1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] allowedCharacters2 = { '3', '4', '5', '6', '7', '8', '9', '0', '1', '2' };
            char[] allowedCharacters3 = { '6', '7', '8', '9', '0', '1', '2', '3', '4', '5' };

            string otp = String.Empty;
            char[] tempCharacters1 = new char[otpLength];
            char[] tempCharacters2 = new char[otpLength];
            char[] tempCharacters3 = new char[otpLength];

            Random rand = new Random();

            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters1[i] = allowedCharacters1[rand.Next(0, allowedCharacters1.Length)];
            }
            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters2[i] = allowedCharacters2[rand.Next(0, allowedCharacters2.Length)];
            }
            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters3[i] = allowedCharacters3[rand.Next(0, allowedCharacters3.Length)];
            }

            for (int i = 0; i < otpLength; i++)
            {
                //Pick characters randomly from either of the three arrays
                int array = rand.Next(0, 3);
                if (array == 0)
                    otp += tempCharacters1[i];
                else if (array == 1)
                    otp += tempCharacters2[i];
                else
                    otp += tempCharacters3[i];
            }
            //Console.WriteLine("tempCharacters1:" + new string(tempCharacters1));
            //Console.WriteLine("tempCharacters2:" + new string(tempCharacters2));
            //Console.WriteLine("tempCharacters3:" + new string(tempCharacters3));

            return otp;
        }

        public static string GenerateRequestId(int otpLength)
        {
            char[] allowedCharacters1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] allowedCharacters2 = { '3', '4', '5', '6', '7', '8', '9', '0', '1', '2' };
            char[] allowedCharacters3 = { '6', '7', '8', '9', '0', '1', '2', '3', '4', '5' };
            char[] allowedCharacters4 = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            string otp = String.Empty;
            char[] tempCharacters1 = new char[otpLength];
            char[] tempCharacters2 = new char[otpLength];
            char[] tempCharacters3 = new char[otpLength];
            char[] tempCharacters4 = new char[otpLength];

            Random rand = new Random();

            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters1[i] = allowedCharacters1[rand.Next(0, allowedCharacters1.Length)];
            }
            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters2[i] = allowedCharacters2[rand.Next(0, allowedCharacters2.Length)];
            }
            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters3[i] = allowedCharacters3[rand.Next(0, allowedCharacters3.Length)];
            }
            for (int i = 0; i < otpLength; i++)
            {
                tempCharacters4[i] = allowedCharacters4[rand.Next(0, allowedCharacters4.Length)];
            }

            for (int i = 0; i < otpLength; i++)
            {
                //Pick characters randomly from either of the three arrays
                int array = rand.Next(0, 4);
                if (array == 0)
                    otp += tempCharacters1[i];
                else if (array == 1)
                    otp += tempCharacters2[i];
                else if (array == 2)
                    otp += tempCharacters3[i];
                else
                    otp += tempCharacters4[i];
            }
            //Console.WriteLine("tempCharacters1:" + new string(tempCharacters1));
            //Console.WriteLine("tempCharacters2:" + new string(tempCharacters2));
            //Console.WriteLine("tempCharacters3:" + new string(tempCharacters3));

            return otp;
        }

        //public static string GenerateOTP(int otpLength)
        //{
        //    string[] allowedCharacters = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        //    string otp = String.Empty;
        //    string tempCharacters = String.Empty;
        //    Random rand = new Random();

        //    for (int i = 0; i < otpLength; i++)
        //    {
        //        tempCharacters = allowedCharacters[rand.Next(0, allowedCharacters.Length)];
        //        otp += tempCharacters;
        //    }
        //    return otp;
        //}
    }
}