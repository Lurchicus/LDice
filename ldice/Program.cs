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
            int RetVal = Dice.Process();
            return RetVal;
        }

        private static int Process()
        {
            string? ReqSave = "";
            string? Request = "";
            int Ret = 0;
            ParseHelp();
            while (!Quit) 
            {
                ReqSave = Request;
                 W("> ");
                Request = R();
                if (Request != null) {
                    if ((Request.ToLower().Trim() == "q") ||
                        (Request.ToLower().Trim() == "x")) 
                        {
                            return 0;
                        }
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