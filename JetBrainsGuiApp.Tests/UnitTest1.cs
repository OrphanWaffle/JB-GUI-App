using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using JetBrainsGuiApp;
using System.Windows.Media;

namespace JetBrainsGuiApp.Tests
{
    public class ShellExecutorTests
    {
        private readonly ShellExecutor _executor = new ShellExecutor();

        [Fact]
        public void Execute_EmptyCommand_ReturnsAppError()
        {
            // Arrange
            string command = "";

            // Act
            var result = _executor.ExecuteShellCommand(command);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.segment);
            Assert.Contains("Command cannot be empty.",result.segment.First().Item2);
            Assert.Equal(OutputType.AppError, result.segment.First().Item1);
            Assert.Equal(-1, result.ExitCode);
        }

        [Fact]
        public void Execute_EchoCommand_ReturnsStdOut()
        {
            // Arrange
            string testString = $"Test Output";
            string command = $"echo {testString}";

            // Act
            var result = _executor.ExecuteShellCommand(command);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.segment);
            Assert.Contains(testString,result.segment.First().Item2,StringComparison.OrdinalIgnoreCase);
            Assert.Equal(OutputType.StandardOutput, result.segment.First().Item1);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void ExecuteCommand_StderrCommand_ReturnsStdErrAndStdOut()
        {
            // Arrange
            string command = "dir nonexistenfile";

            // Act
            var result = _executor.ExecuteShellCommand(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.segment.Count);
            var segmentOut = result.segment[0];
            var segmentErr = result.segment[1];
            Assert.Equal(OutputType.StandardOutput, segmentOut.Item1);
            Assert.Equal(OutputType.StandardError, segmentErr.Item1);
            Assert.Contains("File not found", segmentErr.Item2,StringComparison.OrdinalIgnoreCase);
            Assert.NotEqual(0, result.ExitCode);
        }

        [Fact]
        public void ExecuteCommand_CommandNotFound_ReturnsStdErrAndNonZeroExitCode()
        {
            // Arrange
            string command = $"nonexistentcommand";

            // Act
            var result = _executor.ExecuteShellCommand(command);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.segment);
            Assert.Equal(OutputType.StandardError, result.segment.First().Item1);
            Assert.NotEqual(0, result.ExitCode);
        }

        [Fact]
        public void ExecuteCommand_Timeout_ReturnsAppErrorAndTimeoutMessage()
        {
            // Arrange
             string command = "ping -t 127.0.0.1";

            // Act
            var result = _executor.ExecuteShellCommand(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.segment.Count);

            var segmentOut = result.segment[1];
            var segmentAppErr = result.segment[0];
            Assert.Equal(OutputType.StandardOutput, segmentOut.Item1);
            Assert.Equal(OutputType.AppError, segmentAppErr.Item1);

            Assert.Contains("PROCESS TIMED OUT!", segmentAppErr.Item2, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(-1, result.ExitCode);
        }
    }
}