using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace JetBrainsGuiApp
{
    public partial class MainWindow : Window
    {
        private readonly ShellExecutor _shellCommandExecutor = new ShellExecutor();
        public MainWindow()
        {
            InitializeComponent();
            outputRichTextBox.Background = Brushes.Black;
        }

        private async void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            await RunCommand();
        }

        private async Task RunCommand()
        {
            string commandToExecute = inputTextBox.Text;
            inputTextBox.Clear();

            try
            {
                var result = await Task.Run(() => _shellCommandExecutor.ExecuteShellCommand(commandToExecute));

                AppendFormattedText(outputRichTextBox, $"Executed Command: {commandToExecute}\n------------------------------------", 0);

                foreach (var segment in result.segment)
                {
                    AppendFormattedText(outputRichTextBox, segment.Item2, segment.Item1);
                }
            }
            catch (Exception ex)
            {
                AppendFormattedText(outputRichTextBox, $"\n--- APPLICATION ERROR ---\n{ex.Message}", OutputType.AppError);
            }
            finally
            {
                outputRichTextBox.ScrollToEnd();
            }
        }

        private void AppendFormattedText(RichTextBox rtb, string text, OutputType type)
        {
            Brush foregroundBrush = Brushes.White;
            FontWeight fontWeight = FontWeights.Normal;

            switch (type)
            {
                case OutputType.StandardOutput: foregroundBrush = Brushes.LightGray; break;
                case OutputType.StandardError:  foregroundBrush = Brushes.Red; fontWeight = FontWeights.Bold; break;
                case OutputType.AppError:       foregroundBrush = Brushes.Magenta; fontWeight = FontWeights.Bold; break;
            }
            text ??= string.Empty;

            var paragraph = new Paragraph();
            var run = new Run(text) { Foreground = foregroundBrush, FontWeight = fontWeight };
            paragraph.Inlines.Add(run);
            rtb.Document.Blocks.Add(paragraph);
        }
    }
}