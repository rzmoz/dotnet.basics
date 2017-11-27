using System.Linq;
using DotNet.Basics.PowerShell.Sdk;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.NetFramework.PowerShell
{
    public class PowerShellCmdletTests
    {
        [Fact]
        public void Parameters_ArrayParam_ValueIsArrayFormatting()
        {
            //arrange
            var paramKey = "myKey";
            var arrayValues = new[] { "myStr1", "myStr2", "myStr3" };
            var cmdlet = new PowerShellCmdlet(string.Empty);

            //act
            cmdlet.AddParameter(paramKey, arrayValues);

            //assert
            for (var i = 0; i < cmdlet.Parameters.Length; i++)
            {
                var value = ((string[])cmdlet.Parameters.Single().Value)[i];
                value.Should().Be($"myStr{i + 1}", value);
            }
        }

        [Fact]
        public void ToString_ToScript_CmdletFormattedAsScriptString()
        {
            //arrange
            var cmdletName = "My-Cmdlet";
            var paramKey = "myKey";
            var paramValue = "myValue";
            var cmdlet = new PowerShellCmdlet(cmdletName);
            cmdlet.AddParameter(paramKey, paramValue); //full value
            cmdlet.AddParameter(paramKey, new[] { "1", "2" }); //string array
            cmdlet.AddParameter("Force"); //full value

            //act
            var script = cmdlet.ToString();

            //assert
            script.Should().Be($"{cmdletName} -{paramKey} \"{paramValue}\" -myKey (\"1\",\"2\") -Force");
        }
    }
}