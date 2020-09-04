using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FateUtilCmd
{
    class Program
    {
        private static StringBuilder _sb;
        private static string _str;
        private static string _jassFilePath;
        private static string _exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        private static string _directoryPath = Path.GetDirectoryName(_exePath);
        //This is unix new line format.
        private static string _newLine = "\r";

        static void Main(string[] args)
        {
            //Debug Used

            _jassFilePath = $"{_directoryPath}/war3map.j";

            if (!File.Exists(_jassFilePath))
            {
                Console.WriteLine("J檔案不存在開啟，請按任意鍵關閉。");
                return;
            }
            //初始化誤刪
            _str = File.ReadAllText(_jassFilePath);
            _sb = new StringBuilder(_str);
            

            AddRefeshScoreBoard();
            AddSkin();
            /*string rgx1 = "Table__ht";
            string rgx2 = "Table___ht";
            AdjustPrefix(rgx1, rgx2);*/


        }

        static void AddRefeshScoreBoard()
        {
            string stringPath = $"{_directoryPath}/AddRefeshScoreBoard.txt";

            if (!File.Exists(stringPath))
            {
                Console.WriteLine("AddRefeshScoreBoard.txt 不存在，所以不修改");
                return;
            }

            TextForPosition obj = new TextForPosition();
            obj.CompareStringFirst = "s__BloodBath__DoT_periodic";
            obj.CompareStringSecond = "s__BloodBath___DoT_periodic";
            obj.ClearAtStringBefore = "";
            obj.InsertString =File.ReadAllText(stringPath);
            AddCodeAfterSearchText(obj, true);
        }

        static void AddSkin()
        {
            string stringPath = $"{_directoryPath}/AddSkin.txt";

            if (!File.Exists(stringPath))
            {
                Console.WriteLine("AddSkin.txt 不存在，所以不修改");
                return;
            }

            string compStr1 = "FreeSkin__OnExpiration";
            string compStr2 = "FreeSkin___OnExpiration";
            string clearFrontStr = "call ReleaseTimer(GetExpiredTimer())";
            string insertString = File.ReadAllText(stringPath);
            TextForPosition textObj = new TextForPosition()
            {
                CompareStringFirst = compStr1,
                CompareStringSecond = compStr2,
                ClearAtStringBefore = clearFrontStr,
                InsertString = insertString,
            };

            AddCodeAfterSearchText(textObj, false);
        }

        static void AddCodeAfterSearchText(TextForPosition obj, bool reverse)
        {
            int startPos = CompareReutrnPosition(obj.CompareStringFirst, obj.CompareStringSecond);
            if (startPos == -1)
            {
                Console.WriteLine($"{nameof(AddCodeAfterSearchText)}:找不到的Code");
                return;
            }

            int cleanStrStartPos;
            if (reverse)
            {
                cleanStrStartPos = _str.LastIndexOf(_newLine, startPos) + _newLine.Length;
            }
            else
            {
                cleanStrStartPos = _str.IndexOf(_newLine, startPos) + _newLine.Length;
            }


            if (cleanStrStartPos == -1)
            {
                Console.WriteLine($"{nameof(AddCodeAfterSearchText)}:找不到的換行位置");
                return;
            }

            //是空值就會直接新增行數
            if (!string.IsNullOrEmpty(obj.ClearAtStringBefore))
            {
                int cleanStrEndPos = _str.IndexOf(obj.ClearAtStringBefore);
                _sb.Remove(cleanStrStartPos, cleanStrEndPos - cleanStrStartPos);
            }

            _sb.Insert(cleanStrStartPos, obj.InsertString + _newLine);

            _str = _sb.ToString();
            File.WriteAllText(_jassFilePath, _str);
        }

        static int CompareReutrnPosition(string compareString1, string compareString2)
        {
            if (_str.IndexOf(compareString1) != -1)
            {
                return _str.IndexOf(compareString1);
            }

            if (_str.IndexOf(compareString2) != -1)
            {
                return _str.IndexOf(compareString2);
            }

            return -1;
        }

        static void AdjustPrefix(string compareString1, string compareString2)
        {
            Regex rgx1 = new Regex(compareString1);
            Regex rgx2 = new Regex(compareString2);
            int rgx1Count = rgx1.Matches(_str).Count;
            int rgx2Count = rgx2.Matches(_str).Count;

            //計算符合數量
            Console.WriteLine($"rgx1Count={rgx1Count}, rgx2Count={rgx2Count}");
            if (rgx1Count > rgx2Count)
            {
                _sb.Replace(compareString2, compareString1);
            }
            else if (rgx1Count < rgx2Count)
            {
                _sb.Replace(compareString1, compareString2);
            }
            else if (_str.IndexOf(compareString1) < _str.IndexOf(compareString2))
            {
                _sb.Replace(compareString2, compareString1);
            }
            else if (_str.IndexOf(compareString1) > _str.IndexOf(compareString2))
            {
                _sb.Replace(compareString1, compareString2);
            }
            else
            {
                return;
            }

            _str = _sb.ToString();
            File.WriteAllText(_jassFilePath, _str);
        }
    }
}
