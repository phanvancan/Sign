using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SiginBS.Common
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static string EmptyNull(this string str)
        {
            return str ?? "";
        }

        public static bool IsEquals(this string str, string value)
        {
            return str.EmptyNull().Equals(value);
        }

        public static bool IsMinLength(this string str, int minLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            else
            {
                return str.Length >= minLength;
            }
        }

        public static bool IsOverLength(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }
            else
            {
                return str.Length <= maxLength;
            }
        }

        public static int ToInt(this string value, int valueDefault)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return valueDefault;
            }

            int outValue = 0;
            int.TryParse(value.ToString(), out outValue);
            return outValue;
        }

        public static int NumberLine(this string value, int maxlength, double ratioConvertUpperCase)
        {
            int numberLine = 0;
            if (value.IsNullOrEmpty())
            {
                return numberLine;
            }

            int maxlenghtCharacter = maxlength;
            string[] array = SplitLineFeed(value);
            foreach (var item in array)
            {
                if (item.NumberCharactorUpdateCase() > (maxlength / 2))
                {
                    maxlenghtCharacter = (int)(maxlength / ratioConvertUpperCase);
                }

                numberLine = numberLine + RoundNumber(item.Length, maxlenghtCharacter);
            }

            return numberLine;
        }

        public static string[] SplitLineFeed(this string value)
        {
            return value.Split(new string[] { "\n" }, StringSplitOptions.None);
        }

        private static int NumberCharactorUpdateCase(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return 0;
            }

            return str.ToCharArray().Where(c => char.IsUpper(c)).Count();
        }
        private static int RoundNumber(int divisor, int divided)
        {
            if (divisor == 0)
            {
                return 1;
            }

            int result = (divisor / divided);
            int overbalance = divisor % divided;
            if (overbalance > 0)
            {
                result = result + 1;
            }

            return result;
        }

        public static List<string> ConvertToList(this string value, char character)
        {
            if (value.IsNullOrEmpty())
            {
                return new List<string>();
            }

            List<string> listData = value.Split(character).ToList();
            return listData;
        }

        public static string DecodeUrl(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }

            return HttpUtility.UrlDecode(value, Encoding.UTF8);
        }

        public static string ToAscii(this string unicode)
        {
            if (string.IsNullOrEmpty(unicode)) return "";
            string result = unicode.ToLower().Trim();
            string[] arrSrc = new string[] { " ", "&", "'", ">","<","!",":","#",".","+",
                "~","@","$","%","^","*","(",")",",","}","{","]","[",";","?","/","\\","\"","“","”",
                "Đ","đ", "ê", "â", "ư", "ơ", "ă","ô",
                    "ế", "ấ", "ứ", "ớ", "ắ","á","ú","ó","ố","í","ý","é",
                    "ề", "ầ", "ừ", "ờ", "ằ","à","ù","ò","ồ","ì","ỳ","è",
                    "ể", "ẩ", "ử", "ở", "ẳ","ả","ủ","ỏ","ổ","ỉ","ỷ","ẻ",
                    "ễ", "ẫ", "ữ", "ỡ", "ẵ","ã","ũ","õ","ỗ","ĩ","ỹ","ẽ",
                    "ệ", "ậ", "ự", "ợ", "ặ","ạ","ụ","ọ","ộ","ị","ỵ","ẹ"};
            string[] arrDest = new string[] { "-", "", "", "", "", "","","","","","","",
                "","","","","","","","","","","","","","","","","","",
                "D","d", "e", "a", "u", "o", "a","o",
                    "e", "a", "u", "o", "a","a","u","o","o","i","y","e",
                    "e", "a", "u", "o", "a","a","u","o","o","i","y","e",
                    "e", "a", "u", "o", "a","a","u","o","o","i","y","e",
                    "e", "a", "u", "o", "a","a","u","o","o","i","y","e",
                    "e", "a", "u", "o", "a","a","u","o","o","i","y","e"};
            for (int ct = 0; ct < arrSrc.Length; ct++)
            {
                result = result.Replace(arrSrc[ct].ToString(), arrDest[ct].ToString());
            }
            return result;
        }

        public static string ToHexString(this byte[] hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        public static byte[] ToHexBytes(this string hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return new byte[0];

            int l = hex.Length / 2;
            var b = new byte[l];
            for (int i = 0; i < l; ++i)
            {
                b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return b;
        }

    }
}
