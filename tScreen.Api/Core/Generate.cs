using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Generate
    {
        protected static readonly char[] UppercaseCharList = { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        protected static readonly char[] LowercaseCharList = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        protected static readonly char[] DigitCharList = {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        protected static readonly char[] SymbolCharList = { 
            '!', '@', '#', '$', '%', '^', '&', '*', '_', '-' 
        };

        public static char RandomCharacter(IRandom rand, char[] value) => 
            value[rand.Next(0, value.Length)];

        public static char RandomCharacter(Random rand, char[] value) => 
            value[rand.Next(0, value.Length)];

        public static char RandomCharacter(CryptoRandom rand, char[] value) => 
            value[rand.Next(0, value.Length)];

        public static char RandomCharacter(IRandom rand, string value) => 
            value[rand.Next(0, value.Length)];

        public static char RandomCharacter(Random rand, string value) => 
            value[rand.Next(0, value.Length)];

        public static char RandomCharacter(CryptoRandom rand, string value) => 
            value[rand.Next(0, value.Length)];
        
        public static string RandomPassword(
            int requiredLength = 4, 
            int requiredUniqueChars = 4, 
            bool requireUppercase = true,
            bool requireLowercase = true,
            bool requireDigit = true,
            bool requireNonAlphanumeric = true
        )
        {
            return RandomStringWithSpecification(
                new Random(),
                requiredLength, 
                requiredUniqueChars, 
                requireUppercase, 
                requireLowercase, 
                requireDigit, 
                requireNonAlphanumeric);
        }

        public static string CryptographicRandomString(
            int requiredLength = 4, 
            int requiredUniqueChars = 4, 
            bool requireUppercase = true,
            bool requireLowercase = true,
            bool requireDigit = true,
            bool requireNonAlphanumeric = true
        )
        {
            return RandomStringWithSpecification(
                new CryptoRandom(),
                requiredLength, 
                requiredUniqueChars, 
                requireUppercase, 
                requireLowercase, 
                requireDigit, 
                requireNonAlphanumeric);
        }
        
        public static string CryptoRandomPassword(
            int requiredLength = 4, 
            int requiredUniqueChars = 4, 
            bool requireUppercase = true,
            bool requireLowercase = true,
            bool requireDigit = true,
            bool requireNonAlphanumeric = true
        )
        {
            return RandomStringWithSpecification(
                new CryptoRandom(),
                requiredLength, 
                requiredUniqueChars, 
                requireUppercase, 
                requireLowercase, 
                requireDigit, 
                requireNonAlphanumeric);
        }

        public static string RandomStringWithSpecification(
            IRandom rand,
            int requiredLength = 4, 
            int requiredUniqueChars = 4, 
            bool requireUppercase = true,
            bool requireLowercase = true,
            bool requireDigit = true,
            bool requireNonAlphanumeric = true
        )
        {
            var temp = new List<char>();

            if (requireUppercase)
            {
                temp.Insert(rand.Next(0, temp.Count), 
                    RandomCharacter(rand, UppercaseCharList));
            }
            
            if (requireLowercase)
            {
                temp.Insert(rand.Next(0, temp.Count), 
                    RandomCharacter(rand, LowercaseCharList));
            }

            if (requireDigit)
            {
                temp.Insert(rand.Next(0, temp.Count), 
                    RandomCharacter(rand, DigitCharList));
            }

            if (requireNonAlphanumeric)
            {
                temp.Insert(rand.Next(0, temp.Count), 
                    RandomCharacter(rand, SymbolCharList));
            }


            for (var i = temp.Count; i < requiredLength 
                    || temp.Distinct().Count() < requiredUniqueChars; i++)
            {
                var charType = rand.Next(0, 3);
                
                temp.Insert(rand.Next(0, temp.Count), 
                    RandomCharacter(rand, charType switch
                    {
                        0 => UppercaseCharList,
                        1 => LowercaseCharList,
                        2 => DigitCharList,
                        _ => SymbolCharList
                    }));
            }

            return new string(temp.ToArray());
        }
    }
}