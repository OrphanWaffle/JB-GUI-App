using System.Diagnostics;

namespace JetBrainsGuiApp
{
    public enum OutputType
    {
        StandardOutput, StandardError, AppError
    }

    public class ProcessOutput
    {
        public List<Tuple <OutputType, string>> segment;
 
        public int ExitCode;

        public ProcessOutput()
        {
            segment = new List<Tuple<OutputType, string>>();
            ExitCode = -1;
        }

        public ProcessOutput(OutputType type, string text)
        {
            segment = [new Tuple<OutputType,string>(type, text ?? string.Empty)];
            ExitCode = -1;
        }

        public ProcessOutput(OutputType type, string text, int exitCode)
        {
            segment = [new Tuple<OutputType,string>(type, text ?? string.Empty)];
            ExitCode = exitCode;
        }
        
    }
}