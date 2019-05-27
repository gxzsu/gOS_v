using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;

namespace VirtualOS
{
    class Program
    {
        string blankMessageIdentifier = "ɝ";
        string[] systemMessages = new string[byte.MaxValue];
        byte systemMessagesPointer = 0;
        bool loggedIn = false;

        string[] notes = new string[byte.MaxValue];
        int noteIndex  = 0;

        void Setup()
        {
            for (byte index = 0; index < byte.MaxValue; index++)
            {
                systemMessages[index] = blankMessageIdentifier;
            }

            /// Setup some default messages.
            systemMessages[0] = "GAZ V-OS ALPHA";
            systemMessages[1] = "著作権 (C) ガレス・エッグストーン ２０１８年";
            systemMessages[2] = "全著作権所有\n";

            /// Correct the systemMessagesPointer.
            systemMessagesPointer = 3;

            DrawScreen();
        }

        void PauseFor(float secondsToPause)
        {
            Thread.Sleep((int)(secondsToPause * 1000));
        }

        static void Main(string[] args)
        {
            Program _ = new Program();

            _.Setup();
        }

        void DrawScreen()
        {
            Console.Clear();

            for (byte index = 0; index < byte.MaxValue; index++)
            {
                if (systemMessages[index] != blankMessageIdentifier)
                    Console.WriteLine(systemMessages[index]);
            }

            HandleUserInput();
        }

        void CreateSystemMessage(string messageToAdd, ConsoleColor consoleColor = ConsoleColor.White)
        {
            systemMessages[systemMessagesPointer] = messageToAdd;
            systemMessagesPointer++;
        }

        void HandleUserInput()
        {
            Console.Write("\nG:\\>");

            var userInput = Console.ReadLine();
            CreateSystemMessage("\nG:\\>" + userInput);

            var arrayOfInputData = userInput.Split(' ');

            var userCommmand = arrayOfInputData[0];
            var userCommandArguments = new string[arrayOfInputData.Length];

            for (byte index = 1; index < arrayOfInputData.Length; index++)
            {
                userCommandArguments[index] = arrayOfInputData[index];
            }

            switch (userCommmand)
            {
                case "cls":
                    Setup(); break;

                case "dir":
                    DirectoryListing(userCommandArguments); break;

                case "display":
                    Display(userCommandArguments); break;

                case "shutdown":
                    Shutdown(userCommandArguments); break;

                case "evaluate":
                    Evaluate(userCommandArguments); break;

                case "mkfile":
                    MKFile(userCommandArguments); break;

                case "mkdir":
                    MKDir(userCommandArguments); break;

                case "help":
                    HelpPage(userCommandArguments);  break;

                case "login":
                    Login(userCommandArguments); break;

                case "addnote":
                    AddNote(userCommandArguments); break;

                case "shownotes":
                    DisplayNotes(userCommandArguments); break;

                case "gbasic":
                    GBasicInterpreter(userCommandArguments); break;

                default:
                    CreateSystemMessage("> 不明なまたは悪いコマンド" + userCommmand);
                    break;
            }

            DrawScreen();
        }

        private void HelpPage(string[] userCommandArguments)
        {
            CreateSystemMessage("> コマンドリスト <");
            CreateSystemMessage("> cls : 画面をクリアする");
            CreateSystemMessage("> dir : デイレクトリリスチング");
            CreateSystemMessage("> display <file> : ファイルを表示する");
            CreateSystemMessage("> evaluate <sin | cos | tan expression> : 結果を返す（ラジアン）");
            CreateSystemMessage("> shutdown : シャットダウン");
        }

        private void Login(string[] userCommandArguments)
        {
            if (userCommandArguments[1] == "root" && userCommandArguments[2] == "root")
            {
                CreateSystemMessage("> 今ログインしています <\n> V-OSの全機能をお楽しみください");
                loggedIn = true;
            }
        }

        void Display(string[] userCommandArguments)
        {
            if (!loggedIn)
            {
                CreateSystemMessage("> ログインしてください");
                return;
            }

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/root/");

            foreach (string file in files)
            {
                try
                {
                    var trueFileName = file.Substring((Directory.GetCurrentDirectory() + "/root").Length + 1);
                    var longDirectory = Directory.GetCurrentDirectory() + "/root/" + userCommandArguments[1];

                    if (userCommandArguments[1] == trueFileName)
                    {
                        FileInfo fI = new FileInfo(longDirectory);
                        StreamReader fileReader = new StreamReader(longDirectory);

                        if (fI.Length > 0)
                        {
                            CreateSystemMessage("\n> " + trueFileName + "のデータ：");
                            CreateSystemMessage(fileReader.ReadToEnd());
                        }

                        else
                            CreateSystemMessage("> ファイルは空です :(");

                        return;
                    }
                }

                catch {  }
            }

            CreateSystemMessage("> コマンドに与えられたファイル名が見つかりませんでした - チェックしてください");
        }

        void DirectoryListing(string[] userCommandArguments)
        {
            if (!loggedIn)
            {
                CreateSystemMessage("> ログインしてください");
                return;
            }

            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + "/root/");

            foreach (var dir in dirs)
            {
                var fileNameToDisplay = dir.Substring((Directory.GetCurrentDirectory() + "/root").Length + 1);

                CreateSystemMessage("<DIR>  | " + fileNameToDisplay);
            }

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/root/");

            foreach (var file in files)
            {
                var fileNameToDisplay = file.Substring((Directory.GetCurrentDirectory() + "/root").Length + 1);

                FileInfo fi = new FileInfo(file);

                var fileSizeB = fi.Length;
                var fileSizeb = fileSizeB * 8;

                CreateSystemMessage("<FILE> | " + fileNameToDisplay + " (" + fileSizeB + " bytes [" + fileSizeb + " bits])");
            }
        }

        void Shutdown(string[] userCommandArguments)
        {
            SystemSounds.Hand.Play();
            CreateSystemMessage("> システムがシャットダウンしています。。。");

            DrawScreen();

            PauseFor(3);
            Environment.Exit(0);
        }

        void Evaluate(string[] userCommandArguments)
        {
            if (!loggedIn)
            {
                CreateSystemMessage("> ログインしてください");
                return;
            }

            if (userCommandArguments[2].Trim() == "+")
                CreateSystemMessage((Int32.Parse(userCommandArguments[1]) + Int32.Parse(userCommandArguments[3])).ToString());

            else if (userCommandArguments[2].Trim() == "-")
                CreateSystemMessage((Int32.Parse(userCommandArguments[1]) - Int32.Parse(userCommandArguments[3])).ToString());

            else if (userCommandArguments[1].Trim().ToLower() == "sin")
                CreateSystemMessage(Math.Sin(Double.Parse(userCommandArguments[2])).ToString());

            else if (userCommandArguments[1].Trim().ToLower() == "cos")
                CreateSystemMessage(Math.Cos(Double.Parse(userCommandArguments[2])).ToString());

            else if (userCommandArguments[1].Trim().ToLower() == "tan")
                CreateSystemMessage(Math.Tan(Double.Parse(userCommandArguments[2])).ToString());
        }

        void MKFile(string[] userCommandArguments)
        {
            if (!loggedIn)
            {
                CreateSystemMessage("> ログインしてください");
                return;
            }

            if (!(File.Exists((Directory.GetCurrentDirectory() + "/root/" + userCommandArguments[1]))))
                File.Create(Directory.GetCurrentDirectory() + "/root/" + userCommandArguments[1]);
        }

        void MKDir(string[] userCommandArguments)
        {
            if (!loggedIn)
            {
                CreateSystemMessage("> ログインしてください");
                return;
            }

            if (!(Directory.Exists((Directory.GetCurrentDirectory() + "/root/" + userCommandArguments[1]))))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/root/" + userCommandArguments[1]);
            }
        }

        void AddNote(string[] userCommandArguments)
        {
            string noteToAdd = String.Empty;

            foreach(string word in userCommandArguments)
                noteToAdd = noteToAdd + word + " ";

            if (noteIndex < byte.MaxValue)
            {
                notes[noteIndex] = noteToAdd.Trim();

                CreateSystemMessage("> メモを作成された。 ポジション： " + noteIndex);

                noteIndex++;
            }

            else
                CreateSystemMessage("> スペースがないから、メモを追加できませんでした。");
        }

        void DisplayNotes(string[] userCommandArguments)
        {
            for (int index = 0; index < noteIndex; index++)
                CreateSystemMessage("> " + notes[index]);
        }

        void GBasicInterpreter(string[] userCommandArguments)
        {
            GBasicParse(GBasicLexicalAnalysis(GBasicOpenFile(Directory.GetCurrentDirectory() + "/root/" + userCommandArguments[1])));
        }

        string GBasicOpenFile(string v)
        {
            try
            {
                using (StreamReader reader = new StreamReader(v))
                {
                    return reader.ReadToEnd();
                }
            }

            catch { return String.Empty; }
        }

        List<string> GBasicLexicalAnalysis(string fileContents)
        {
            List<string> TOKENS = new List<string>();

            byte state = 0;
            string token = string.Empty;
            string varString = string.Empty;

            char[] fileAsChars = fileContents.ToCharArray();

            foreach (char character in fileAsChars)
            {
                token += character;

                if (token == " " && state == 0)
                    token = string.Empty;

                else if (token == Environment.NewLine)
                    token = string.Empty;

                else if (token == "PRINT")
                {
                    TOKENS.Add(token);

                    token = string.Empty;
                }

                else if (token == "\"")
                {
                    if (state == 0)
                        state = 1;

                    else if (state == 1)
                    {
                        TOKENS.Add("STRING:" + varString + "\"");

                        state = 0;

                        varString = string.Empty;
                        token = string.Empty;
                    }
                }

                else if (state == 1)
                {
                    varString += token;
                    token = string.Empty;
                }
            }

            return TOKENS;
        }

        void GBasicParse(List<string> tokens)
        {
            byte i = 0;

            while (i < tokens.Count)
            {
                if (tokens[i] + " " + tokens[i + 1].Substring(0, 6) == "PRINT STRING")
                {
                    CreateSystemMessage(">>> " + tokens[i + 1].Substring(7));
                    i++;
                    i++;
                }
            }
        }
    }
}
