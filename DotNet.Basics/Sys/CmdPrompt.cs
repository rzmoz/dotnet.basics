﻿using System;

namespace DotNet.Basics.Sys
{
    public static class CmdPrompt
    {
        public static int Run(string commandString, Action<string> writeOutput = null, Action<string> writeError = null)
        {
            return ExternalProcess.Run("cmd.exe", $"/c {commandString}", writeOutput: writeOutput, writeError: writeError);
        }
    }
}
