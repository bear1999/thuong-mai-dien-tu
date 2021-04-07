using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TMDT.Function
{
    public class _Function
    {
        public static String md5(string text)
        {
            String str = "";
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            foreach (Byte b in buffer)
                str += b.ToString("x2");
            return str;
        }
        public static string RandomOTP()
        {
            int dodaithe = 6;
            string allowednumber = "0123456789";
            char[] chars = new char[dodaithe];
            Random rd = new Random();

            bool usenumber = true;
            for (int i = 0; i < dodaithe; i++)
            {
                if (usenumber)
                {
                    chars[i] = allowednumber[rd.Next(0, allowednumber.Length)];
                    usenumber = false;
                }
                else
                {
                    chars[i] = allowednumber[rd.Next(0, allowednumber.Length)];
                    usenumber = true;
                }
            }
            return new string(chars);
        }
        public static string convertKhongDau(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}