using System;
using System.Collections.Generic;
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
            _jassFilePath = $"{_directoryPath}/war3map.j";

            if (!File.Exists(_jassFilePath))
            {
                Console.WriteLine("J檔案不存在開啟，請按任意鍵關閉。");
                return;
            }
            //初始化誤刪
            _str = File.ReadAllText(_jassFilePath);
            _sb = new StringBuilder(_str);


            //AddRefeshScoreBoard();
            //AddSkin();
            //AddSurrenderSystem();
            //ModifyScoreAndAmpDefault();
            AddPraticeSystem();
            _str = _sb.ToString();
            File.WriteAllText(_jassFilePath, _str);
            Console.WriteLine("任務已完成");
            Console.ReadKey();
            /*string rgx1 = "Table__ht";
            string rgx2 = "Table___ht";
            AdjustPrefix(rgx1, rgx2);*/
        }

        static void AddPraticeSystem()
        {
            string stringPathFunc = $"{_directoryPath}/AddPraticeSystem.txt";
            if (!File.Exists(stringPathFunc))
            {
                Console.WriteLine("AddPraticeSystem.txt 不存在，所以不修改");
                return;
            }

            TextForPosition obj = new TextForPosition();
            //白亞的韓文跟結束後的那一段文字
            obj.CompareStringFirst = "세이버";
            obj.CompareStringSecond = "";
            obj.ClearAtStringBefore = "call TriggerAddCondition";
            obj.InsertString = File.ReadAllText(stringPathFunc);
            AddCodeAfterSearchText(obj, true);
        }

        static void AddSurrenderSystem()
        {
            string stringPathFunc = $"{_directoryPath}/AddSurrenderSystemPart1.txt";
            string stringPathFunc2 = $"{_directoryPath}/AddSurrenderSystemPart2.txt";
            if (!File.Exists(stringPathFunc))
            {
                Console.WriteLine("AddSurrenderSystemPart1.txt 不存在，所以不修改");
                return;
            }

            if (!File.Exists(stringPathFunc2))
            {
                Console.WriteLine("AddSurrenderSystemPart2.txt 不存在，所以不修改");
                return;
            }

            TextForPosition obj = new TextForPosition();
            //讀取變數
            obj.CompareStringFirst = "endglobals";
            obj.CompareStringSecond = "";
            obj.ClearAtStringBefore = "";
            obj.InsertString = File.ReadAllText(stringPathFunc);
            AddCodeAfterSearchText(obj, true);

            obj.CompareStringFirst = "call s__Game_start_hold()";
            obj.CompareStringSecond = "";
            obj.ClearAtStringBefore = "";
            obj.InsertString = "set flag_surrender=true";
            AddCodeAfterSearchText(obj, false);

            obj.CompareStringFirst = "8000元以下的玩家會獲得3000元的支援。回合失敗方會多獲得500元的支援。";
            obj.CompareStringSecond = "";
            obj.ClearAtStringBefore = "";
            obj.InsertString = "set flag_surrender=true\r" + "call EnableTrigger(Trg_SurrenderSystem)";
            AddCodeAfterSearchText(obj, false);

            obj.CompareStringFirst = "GoldTransfer__OnType";
            obj.CompareStringSecond = "GoldTransfer___OnType";
            obj.ClearAtStringBefore = "";
            obj.InsertString = File.ReadAllText(stringPathFunc2);
            AddCodeAfterSearchText(obj, true);

            obj.CompareStringFirst = "call BootSystem__OnInit()";
            obj.CompareStringSecond = "call BootSystem___OnInit()";
            obj.ClearAtStringBefore = "";
            obj.InsertString = "call SurrenderSystem__OnInit()";
            AddCodeAfterSearchText(obj, false);

        }

        static void ModifyScoreAndAmpDefault()
        {
            string baseString = @"call s__ModeSelectionButton_addOption(s__ModeSelectSystem_ModeSelectUI__scoredSelectButton[this],1,";
            string insertString = @"call s__ModeSelectionButton__set_value(s__ModeSelectSystem_ModeSelectUI__scoredSelectButton[this],1)";
            //先偷查
            if (_str.IndexOf(baseString) == -1)
            {
                baseString = @"call s__ModeSelectionButton_addOption(s__ModeSelectSystem_ModeSelectUI___scoredSelectButton[this],1,";
                insertString = @"call s__ModeSelectionButton__set_value(s__ModeSelectSystem_ModeSelectUI__scoredSelectButton[this],1)";
            }

            TextForPosition obj = new TextForPosition();
            obj.CompareStringFirst = baseString;
            obj.CompareStringSecond = "";
            obj.ClearAtStringBefore = "";
            obj.InsertString = insertString;
            AddCodeAfterSearchText(obj, false);

            string ampStr = @"call s__ModeSelectionButton__set_value(s__ModeSelectSystem_ModeSelectUI__amButton[this],1)";
            string ampStr2 = @"call s__ModeSelectionButton__set_value(s__ModeSelectSystem_ModeSelectUI___amButton[this],1)";
            string ampStrC = @"call s__ModeSelectionButton__set_value(s__ModeSelectSystem_ModeSelectUI__amButton[this],0)";
            string ampStr2C = @"call s__ModeSelectionButton__set_value(s__ModeSelectSystem_ModeSelectUI___amButton[this],0)";
            if (_str.IndexOf(ampStr) != -1)
            {
                _sb.Replace(ampStr, ampStrC);
            }
            else
            {
                _sb.Replace(ampStr2, ampStr2C);
            }
            UpdateString();
        }

        static void AddRefeshScoreBoard()
        {
            string stringPathFunc = $"{_directoryPath}/AddRefeshScoreBoardFunc.txt";
            string stringPath = $"{_directoryPath}/AddRefeshScoreBoard.txt";
            string stringUpdateFunc = $"call UpdateTeamScoreBoard()";

            if (!File.Exists(stringPathFunc))
            {
                Console.WriteLine("AddRefeshScoreBoardFunc.txt 不存在，所以不修改");
                return;
            }

            if (!File.Exists(stringPath))
            {
                Console.WriteLine("AddRefeshScoreBoard.txt 不存在，所以不修改");
                return;
            }

            //補上Function
            TextForPosition obj = new TextForPosition();
            obj.CompareStringFirst = "s__BloodBath__DoT_periodic";
            obj.CompareStringSecond = "s__BloodBath___DoT_periodic";
            obj.ClearAtStringBefore = "";
            obj.InsertString = File.ReadAllText(stringPathFunc);
            AddCodeAfterSearchText(obj, true);

            //替換成FUNCTION名稱
            obj.InsertString = stringUpdateFunc;

            using (StreamReader sr = new StreamReader(stringPath))
            {
                while (sr.Peek() >= 0)
                {
                    obj.CompareStringFirst = sr.ReadLine();
                    obj.CompareStringSecond = sr.ReadLine();
                    AddCodeAfterSearchText(obj, false);
                }
            }
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

            //找不到是-1，+1之後變0
            if (cleanStrStartPos == 0)
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

            UpdateString();
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

            UpdateString();
        }

        static void UpdateString()
        {
            _str = _sb.ToString();
        }
    }
}
