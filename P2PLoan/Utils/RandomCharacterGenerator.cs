using System;
using System.Text;

namespace P2PLoan.Utils;

public static class RandomCharacterGenerator
{
    private static readonly char[] Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()".ToCharArray();

    public static int GenerateRandomNumber(int numberOfDigits)
    {
        if (numberOfDigits < 1)
        {
            throw new ArgumentException("Number of digits must be at least 1.");
        }

        int minValue = (int)Math.Pow(10, numberOfDigits - 1);
        int maxValue = (int)Math.Pow(10, numberOfDigits) - 1;

        Random random = new Random();
        return random.Next(minValue, maxValue + 1);
    }


    public static string GenerateRandomString(int length)
    {
        if (length < 1)
        {
            throw new ArgumentException("Length must be at least 1.");
        }

        Random random = new Random();
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(Characters[random.Next(Characters.Length)]);
        }

        return result.ToString();
    }
}
