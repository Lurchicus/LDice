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
            if (args.Length > 0)
            {
                Inp = args[0];
            }
            int RetVal = Process(string? Inp);
            if (RetVal == 0) {
                return RetVal;
            }
            
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
                            diceCount = 1;
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
                            diceCount = Convert.ToInt32(sside);
                        }
                        catch
                        {
                            diceCount = 1;
                        }
                        if (ary.Length == 1)
                        {
                            // Only die count, default to 6 sides die with no adjustment
                            sideCount = 6;
                            adjust = 0;
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
                        //on the die with no adjustment
                        try
                        {
                            sideCount = Convert.ToInt32(ary2[0]);
                        }
                        catch
                        {
                            sideCount = 6;
                        }
                        adjust = 0;
                    }
                    else
                    {
                        //More than a single result, should have die side count and adjustment
                        if (ary2.Length > 1)
                        {
                            try
                            {
                                sideCount = Convert.ToInt32(ary2[0]);
                            }
                            catch
                            {
                                sideCount = 6;
                            }
                            try
                            {
                                adjust = Convert.ToInt32(ary2[1]);
                            }
                            catch
                            {
                                adjust = 0;
                            }
                            if (adjust > 0)
                            {
                                if (arrg.Contains("-"))
                                {
                                    //if the delimiter was a "-", negate the adjustment
                                    adjust = 0 - adjust;
                                }
                            }
                        }
                        if (ary2.Length <= 0)
                        {
                            //Nothing to see here, default six sided die and no adjustment
                            sideCount = 6;
                            adjust = 0;
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