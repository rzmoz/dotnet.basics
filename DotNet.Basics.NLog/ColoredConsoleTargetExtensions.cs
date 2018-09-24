﻿using NLog;
using NLog.Conditions;
using NLog.Targets;

namespace DotNet.Basics.NLog
{
    public static class ColoredConsoleTargetExtensions
    {
        public static ColoredConsoleTarget WithOutputColors(this ColoredConsoleTarget target,
            ConsoleOutputColor debugColor = ConsoleOutputColor.DarkGray,
            ConsoleOutputColor traceColor = ConsoleOutputColor.Cyan,
            ConsoleOutputColor infoColor = ConsoleOutputColor.White,
            ConsoleOutputColor warnForeColor = ConsoleOutputColor.Yellow,
            ConsoleOutputColor warnBackColor = ConsoleOutputColor.Black,
            ConsoleOutputColor errorForeColor = ConsoleOutputColor.Red,
            ConsoleOutputColor errorBackColor = ConsoleOutputColor.Black,
            ConsoleOutputColor fatalForeColor = ConsoleOutputColor.White,
            ConsoleOutputColor fatalBackColor = ConsoleOutputColor.DarkRed)
        {
            target.RowHighlightingRules.Clear();
            target.AddLogColor(LogLevel.Debug, debugColor)
                .AddLogColor(LogLevel.Trace, traceColor)
                .AddLogColor(LogLevel.Info, infoColor)
                .AddLogColor(LogLevel.Warn, warnForeColor,warnBackColor)
                .AddLogColor(LogLevel.Error, errorForeColor, errorBackColor)
                .AddLogColor(LogLevel.Fatal, fatalForeColor, fatalBackColor);
            return target;
        }

        public static ColoredConsoleTarget AddLogColor(this ColoredConsoleTarget target, LogLevel level, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor = ConsoleOutputColor.NoChange)
        {
            target.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
            {
                Condition = ConditionParser.ParseExpression($"level == LogLevel.{level}"),
                BackgroundColor = backgroundColor,
                ForegroundColor = foregroundColor
            });
            return target;
        }
    }
}
