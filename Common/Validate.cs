using System;
using System.Text.RegularExpressions;

namespace SWPBirdBoarding.Common
{
    public class Validate 
    {
        //validate name (chỉ được nhập chữ )
        private static Regex regexName = new Regex("^[a-zA-Z ]*$");
        //validate number phone of VN
        private static Regex regexPhone = new Regex(@"(0[2|1|3|5|7|8|9])+([0-9]{8})\b");
        //validate chir được chứa chữ hoặc số
        private static Regex regexSpecialChar = new Regex("[^a-zA-Z0-9]");
        //validate number
        private static Regex hasNumber = new Regex(@"[0-9]+");
        //validate upper 
        private static Regex hasUpperChar = new Regex(@"[A-Z]+");
        //validarte min max
        private static Regex hasMiniMaxChars = new Regex(@".{8,16}");
        //validate lower
        private static Regex hasLowerChar = new Regex(@"[a-z]+");
        //validate symbols
        private static Regex hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
        //validate num symbol
        private static Regex regexNumSymbol = new Regex("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]*$");
        //Validate Email
        private static Regex regexEmail = new Regex("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$");

        public static int totaPage(int totalEle, int ele)
        {
            if ((totalEle % ele) == 0)
            {
                return  (totalEle / ele);
            }
            else
            {
                return  (totalEle / ele) + 1;
            }
        }
        public static bool isName(string s)
        {
            if (regexName.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isPhone(string s)
        {
            if (regexPhone.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isSpecialChar(string s)
        {
            if (regexSpecialChar.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isNumber(string s)
        {
            if (hasNumber.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isUpperChar(string s)
        {
            if (hasUpperChar.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isMiniMaxChars(string s)
        {
            if (hasMiniMaxChars.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isLowerChar(string s)
        {
            if (hasLowerChar.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isSymbols(string s)
        {
            if (hasSymbols.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static bool isNumSymbol(string s)
        {
            if (regexNumSymbol.IsMatch(s))
            {
                return true;
            }
            return false;
        }
        public static bool isEmail(string s)
        {
            if (regexEmail.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        public static int getAge(DateTime bornDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - bornDate.Year;
            if (bornDate > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
