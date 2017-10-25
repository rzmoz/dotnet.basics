﻿namespace DotNet.Basics.Cli
{
    public static class CommandPrompt
    {
        public static (string Input, int ExitCode, string Output) Run(string commandString)
        {
            return Executable.Run("cmd.exe", $"/c {commandString}");
        }
    }
}
