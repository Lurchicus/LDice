using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LDice
{
    class Dice
    {
        static private long _Result;
        static private int _NumberOfDice;
        static private int _NumberOfSides;
        static private int _Adjustment;
        static private bool _Debug;
        static private bool _Quit;

        public string Nl = Environment.NewLine;

        public static int Main(string[] args)
        {
            string? Inp = null;
            if (args.Length > 0)
            {
                Inp = args[0];
            }
            int RetVal = Process(Inp);
            if (RetVal == 0) {
                return RetVal;
            }
            return 0;
        }

        private static int Process(string? Request)
        {
            string? ReqSave = "";
            int Ret = 0;
            ParseHelp();
            while (!Quit) 
            {
                ReqSave = Request;
                if (Request == null) 
                {
                    W("> ");
                    Request = R();
                }
                if (Request != null) 
                {
                    if ((Request.ToLower().Trim() == "q") ||
                        (Request.ToLower().Trim() == "x")) 
                    {
                            return 0;
                    }
                    Parse(Request);
                }
                
            }
            return Ret;
        }

        public static long Result 
        {
            get {return _Result;}
            set {_Result = value;}
        }
        public static int NumberOfDice 
        { 
            get {return _NumberOfDice;} 
            set {_NumberOfDice = value;}
        }

        public static int NumberOfSides 
        {
            get {return _NumberOfSides;}
            set {_NumberOfSides = value;}
        }

        public static int Adjustment 
        {
            get {return _Adjustment;}
            set {_Adjustment = value;}
        }

        public static bool Debug
        {
            get {return _Debug;}
            set {_Debug = value;}
        }

        public static bool Quit
        {
            get {return _Quit;}
            set {_Quit = value;}
        }

        public static void ParseHelp()
        {
            Wl("[[[qty]D]sides][+|-][adj] (default \"1D6+0\") sides of 1 or 2 is a coin flip.");
            Wl("-  \"-d\" Debug toggle (shows each die result)");
            Wl("-  \"q\" or \"x\" to quit");
            Wl("-  \"l\" to display the license file");
            Wl("No input repeats last dice throw or defaults to \"1D6+0\" for first throw.");
            Wl("D1 does a 0 or 1 coin toss, D2 does a 1 or 2 coin toss and D0 is 1D6+0.");
        }

        /// <summary>
        /// Parse and process individual CLI inputs one at a time. By default
        /// parse out the individual dice command or default to 1d6+0
        /// </summary>
        /// <param name="arg">string Command line input</param>
        public static void Parse(string arg)
        {
            string arrg = arg.Trim().ToUpper();
            string[] parm1 = { "D" }; //Dies/sides delimiter
            string[] parm2 = { "+", "-" }; //Adjustment delimiter
            string sside = "";
            switch (arg)
            {
                case "-d": //Toggle debug mode on and off
                case "/d":
                    ParseDebug();
                    break;

                case "q": //Set quit flag
                case "-q":
                case "/q":
                case "x": //Set quit flag
                case "-x":
                case "/x":
                    ParseQuit();
                    break;

                case "?": //Display help
                case "-?":
                case "/?":
                    ParseHelp();
                    break;

                case "l": //Display license text
                case "-l":
                case "/l":
                    ShowFile("\\gnu_gpl3.txt");
                    break;

                default: //Parse individual dice roll command or default to 1d6+0
                    if (arrg.Length == 0)
                    {
                        ParseNoArgument();
                        break;
                    }
                    if (arrg.StartsWith("D"))
                    {
                        //Does the command start with "D"
                        if (arrg.Length == 1)
                        {
                            ParseDefault();
                            break;
                        }
                        else
                        {
                            //More to do here, default to one die
                            NumberOfDice = 1;
                            arrg = arrg.Substring(1, arrg.Length - 1);
                        }
                    }
                    else
                    {
                        //Didn't start with a "D" split using "D" as a delimiter. The first argument
                        //should be the die count
                        string[] ary = arrg.Split(parm1, StringSplitOptions.RemoveEmptyEntries);
                        sside = ary[0];
                        try
                        {
                            NumberOfDice = Convert.ToInt32(sside);
                        }
                        catch
                        {
                            NumberOfDice = 1;
                        }
                        if (ary.Length == 1)
                        {
                            // Only die count, default to 6 sides die with no Adjustmentment
                            NumberOfSides = 6;
                            Adjustment = 0;
                            break;
                        }
                        else
                        {
                            //More to do after "D", pass it along
                            arrg = ary[1];
                        }
                    }
                    //Split the remaining parts using "+" or "-" as a delimiter
                    string[] ary2 = arrg.Split(parm2, StringSplitOptions.RemoveEmptyEntries);
                    if (ary2.Length == 1)
                    {
                        //We only got a single result which should be the number of sides
                        //on the die with no Adjustmentment
                        try
                        {
                            NumberOfSides = Convert.ToInt32(ary2[0]);
                        }
                        catch
                        {
                            NumberOfSides = 6;
                        }
                        Adjustment = 0;
                    }
                    else
                    {
                        //More than a single result, should have die side count and Adjustmentment
                        if (ary2.Length > 1)
                        {
                            try
                            {
                                NumberOfSides = Convert.ToInt32(ary2[0]);
                            }
                            catch
                            {
                                NumberOfSides = 6;
                            }
                            try
                            {
                                Adjustment = Convert.ToInt32(ary2[1]);
                            }
                            catch
                            {
                                Adjustment = 0;
                            }
                            if (Adjustment > 0)
                            {
                                if (arrg.Contains("-"))
                                {
                                    //if the delimiter was a "-", negate the Adjustmentment
                                    Adjustment = 0 - Adjustment;
                                }
                            }
                        }
                        if (ary2.Length <= 0)
                        {
                            //Nothing to see here, default six sided die and no Adjustmentment
                            NumberOfSides = 6;
                            Adjustment = 0;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Toggle the debug flag
        /// </summary>
        public static void ParseDebug()
        {
            if (Debug)
            {
                Wl("Debug off");
                Debug = false;
            }
            else
            {
                Wl("Debug on");
                Debug = true;
            }
        }

        /// <summary>
        /// Set the global Quit flag
        /// </summary>
        public static void ParseQuit()
        {
            Wl("Goodbye...");
            Quit = true;
        }

        /// <summary>
        /// Set dice for a no argument default of 1d6+0
        /// </summary>
        public static void ParseNoArgument()
        {
            //No argument so default 1d6+0
            if (NumberOfDice == 0) { NumberOfDice = 1; }
            if (NumberOfSides == 0) { NumberOfSides = 6; }
            if (Adjustment == 0) { Adjustment = 0; }
        }

        /// <summary>
        /// Set a default of 1d6+0
        /// </summary>
        public static void ParseDefault()
        {
            //Only a "D" so default 1d6+0
            NumberOfDice = 1;
            NumberOfSides = 6;
            Adjustment = 0;
        }

        /// <summary>
        /// Will display a paginated file to the Console. If a path
        /// is not supplied, the same path where the program resides
        /// is used
        /// </summary>
        /// <param name="File">File to display</param>
        /// <param name="Path">Path the file resides in (optional)</param>
        public static void ShowFile(String FileName, String PathName = null)
        {
            Int32 ScreenHeight = Console.WindowHeight;
            Int32 ScreenWidth = Console.WindowWidth;
            Int32 Row = 0;
            String? Inp = "";
            String AppPath = "";

            AppPath = SetPath(FileName, PathName);
            try
            {
                using (StreamReader sr = new StreamReader(AppPath))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Wl(line);
                        if (line.Length > ScreenWidth - 1)
                        {
                            Row = Row + (Int32)(line.Length / ScreenWidth - 1);
                        }
                        else
                        {
                            Row++;
                        }
                        if (Row >= ScreenHeight - 1)
                        {
                            Row = 0;
                            W("Press Enter to continue (enter q to quit):");
                            Inp = R();
                            if (Inp == "q" || Inp == "Q")
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Wl("Could not read the text in file " + FileName + ".");
                Wl(e.Message);
            }
        }

        /// <summary>
        /// Make sure we have a properly formed path. If initial path is empty,
        /// set it to the application location.
        /// </summary>
        /// <param name="FileName">Name of file to show</param>
        /// <param name="PathName">Path where file is located</param>
        /// <returns>Fully formed path with filename</returns>
        public static string SetPath(String? FileName, String? PathName = null)
        {
            String Nam = "";
            String AppPath = "";

            if (PathName == null)
            {
                AppPath = Assembly.GetExecutingAssembly().Location;
                Nam = Path.GetFileName(AppPath);
                AppPath = AppPath.Substring(0, AppPath.Length - Nam.Length);
                if (FileName != null)
                {
                    if (FileName.Substring(0, 2) != "\\")
                    {
                        AppPath += "\\" + FileName;
                    }
                    else
                    {
                        AppPath += FileName;
                    }
            }
            else
            {
                if (FileName.Substring(0, 2) != "\\")
                {
                    AppPath += PathName + "\\" + FileName;
                }
                else
                {
                    AppPath = PathName + FileName;
                }
            }
            return AppPath;
        }

        /// <summary>
        /// wrapper function for Console.WriteLine()
        /// </summary>
        /// <param name="Msg">string to output</param>
        public static void Wl(string Msg)
        {
            if (Msg != null)
            {
                Console.WriteLine(Msg);
            }
            else
            {
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Wrapper function for Console.Write()
        /// </summary>
        /// <param name="Msg">string to output</param>
        public static void W(string Msg)
        {
            if (Msg != null)
            {
                Console.Write(Msg);
            }
        }

        /// <summary>
        /// Wrapper function for Console.ReadLine
        /// </summary>
        /// <returns>string input</returns>
        public static string? R()
        {
            return Console.ReadLine();
        }
    }
}