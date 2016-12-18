using System;
using System.Collections;
using System.Collections.Generic;
namespace Common.Config
{
    public static class Parser
    {
        static Dictionary<Action<string>, string> parsers = new Dictionary<Action<string>, string>
        {
            { WinStackAttrib.Parse, "Config.WinStackAttrib" },
        };
        public static void Start()
        {
            foreach(Action<string> parse in parsers.Keys)
            {
                parse(parsers[parse]);
            }
        }
       
    }
}