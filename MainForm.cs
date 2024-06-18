using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AstroImageTracker
{
    public partial class MainForm : Form
    {
        private List<string> csvData;
        private const double TargetHours = 20;
        private const double TargetSeconds = TargetHours * 3600;
        private string currentTargetName;
        private string currentSessionDate;

        public MainForm()
        {
            InitializeComponent();
            txtFolderPath.Text = @"D:\AstroCloud\Brutes\"; // Default path
            csvData = new List<string>();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            var rootFolderPath = txtFolderPath.Text;
            if (Directory.Exists(rootFolderPath))
            {
                csvData.Clear();
                AnalyzeRootDirectory(rootFolderPath);
            }
            else
            {
                MessageBox.Show("Please select a valid directory.");
            }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
                saveFileDialog.Title = "Save analysis results as CSV";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(saveFileDialog.FileName, csvData);
                    MessageBox.Show("CSV file saved successfully.");
                }
            }
        }

        private void AnalyzeRootDirectory(string rootFolderPath)
        {
            var sessionDirectories = Directory.GetDirectories(rootFolderPath);
            var totalImagingTimeLights = 0.0;
            var totalImageCountLights = 0;
            var filterStatsLights = new Dictionary<string, (double time, int count)>();

            var totalImageCountFlats = 0;
            var filterStatsFlats = new Dictionary<string, int>();

            foreach (var sessionDir in sessionDirectories)
            {
                var sessionName = Path.GetFileName(sessionDir);
                currentTargetName = GetTargetNameWithoutDate(sessionName);
                currentSessionDate = GetSessionDateFromName(sessionName);
                AnalyzeSessionDirectory(sessionDir, ref totalImagingTimeLights, ref totalImageCountLights, filterStatsLights, ref totalImageCountFlats, filterStatsFlats, currentTargetName, currentSessionDate);
            }

            DisplayAggregateResults(totalImagingTimeLights, totalImageCountLights, filterStatsLights, totalImageCountFlats, filterStatsFlats);

            // Update the progress bar
            UpdateProgressBar(totalImagingTimeLights);
        }

        private string GetTargetNameWithoutDate(string folderName)
        {
            var match = Regex.Match(folderName, @"(.*?)(_\d{4}-\d{2}-\d{2})?$");
            return match.Success ? match.Groups[1].Value : folderName;
        }

        private string GetSessionDateFromName(string folderName)
        {
            var match = Regex.Match(folderName, @"_(\d{4}-\d{2}-\d{2})$");
            return match.Success ? match.Groups[1].Value : "Unknown Date";
        }

        private void AnalyzeSessionDirectory(string sessionDir, ref double totalImagingTimeLights, ref int totalImageCountLights, Dictionary<string, (double time, int count)> filterStatsLights, ref int totalImageCountFlats, Dictionary<string, int> filterStatsFlats, string sessionName, string sessionDate)
        {
            var sessionFiles = Directory.GetFiles(sessionDir, "*.fits", SearchOption.AllDirectories);
            var sessionImagingTimeLights = 0.0;
            var sessionImageCountLights = 0;
            var sessionFilterStatsLights = new Dictionary<string, (double time, int count)>();

            var sessionImageCountFlats = 0;
            var sessionFilterStatsFlats = new Dictionary<string, int>();

            foreach (var file in sessionFiles)
            {
                var exposureTime = ExtractExposureFromFileName(file);
                var filter = ExtractFilterFromFileName(file) ?? "Unknown";
                var isLight = file.ToUpper().Contains("LIGHT");
                var isFlat = file.ToUpper().Contains("FLAT");

                if (isLight && exposureTime > 0)
                {
                    sessionImagingTimeLights += exposureTime;
                    sessionImageCountLights++;
                    if (!sessionFilterStatsLights.ContainsKey(filter))
                    {
                        sessionFilterStatsLights[filter] = (0, 0);
                    }
                    sessionFilterStatsLights[filter] = (sessionFilterStatsLights[filter].time + exposureTime, sessionFilterStatsLights[filter].count + 1);
                }
                else if (isFlat)
                {
                    sessionImageCountFlats++;
                    if (!sessionFilterStatsFlats.ContainsKey(filter))
                    {
                        sessionFilterStatsFlats[filter] = 0;
                    }
                    sessionFilterStatsFlats[filter]++;
                }
            }

            totalImagingTimeLights += sessionImagingTimeLights;
            totalImageCountLights += sessionImageCountLights;
            foreach (var kvp in sessionFilterStatsLights)
            {
                if (!filterStatsLights.ContainsKey(kvp.Key))
                {
                    filterStatsLights[kvp.Key] = (0, 0);
                }
                filterStatsLights[kvp.Key] = (filterStatsLights[kvp.Key].time + kvp.Value.time, filterStatsLights[kvp.Key].count + kvp.Value.count);
            }

            totalImageCountFlats += sessionImageCountFlats;
            foreach (var kvp in sessionFilterStatsFlats)
            {
                if (!filterStatsFlats.ContainsKey(kvp.Key))
                {
                    filterStatsFlats[kvp.Key] = 0;
                }
                filterStatsFlats[kvp.Key] += kvp.Value;
            }

            if (sessionImageCountLights > 0)
            {
                DisplaySessionResults(sessionName, sessionDate, sessionImagingTimeLights, sessionFilterStatsLights, true);
            }

            if (sessionImageCountFlats > 0)
            {
                DisplaySessionResults(sessionName, sessionDate, sessionImageCountFlats, sessionFilterStatsFlats, false);
            }
        }

        private void DisplaySessionResults(string sessionName, string sessionDate, double sessionImagingTime, Dictionary<string, (double time, int count)> sessionFilterStats, bool isLight)
        {
            AppendText($"Session {sessionName} on {sessionDate}:\n", Color.DarkBlue, new Font("Arial", 12, FontStyle.Bold));
            if (isLight)
            {
                AppendText($"Total Imaging Time: {FormatTime(sessionImagingTime)}\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
                csvData.Add($"Session {sessionName} on {sessionDate},Total Imaging Time,{FormatTime(sessionImagingTime)}");
            }
            AppendText("Filters:\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            csvData.Add("Filter,Time,Count");
            foreach (var kvp in sessionFilterStats)
            {
                if (isLight)
                {
                    AppendText($"- {kvp.Key}: {FormatTime(kvp.Value.time)} ({kvp.Value.count} images)\n", Color.Green, new Font("Arial", 10, FontStyle.Regular));
                    csvData.Add($"{kvp.Key},{FormatTime(kvp.Value.time)},{kvp.Value.count}");
                }
                else
                {
                    AppendText($"- {kvp.Key}: {kvp.Value.count} images\n", Color.Green, new Font("Arial", 10, FontStyle.Regular));
                    csvData.Add($"{kvp.Key},,{kvp.Value.count}");
                }
            }
            AppendText("\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
        }

        private void DisplaySessionResults(string sessionName, string sessionDate, int sessionImageCount, Dictionary<string, int> sessionFilterStats, bool isLight)
        {
            AppendText($"Session {sessionName} on {sessionDate}:\n", Color.DarkBlue, new Font("Arial", 12, FontStyle.Bold));
            if (!isLight)
            {
                AppendText($"Total Number of Images: {sessionImageCount}\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
                csvData.Add($"Session {sessionName} on {sessionDate},Total Number of Images,{sessionImageCount}");
            }
            AppendText("Filters:\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            csvData.Add("Filter,Count");
            foreach (var kvp in sessionFilterStats)
            {
                AppendText($"- {kvp.Key}: {kvp.Value} images\n", Color.Green, new Font("Arial", 10, FontStyle.Regular));
                csvData.Add($"{kvp.Key},{kvp.Value}");
            }
            AppendText("\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
        }

        private void DisplayAggregateResults(double totalImagingTimeLights, int totalImageCountLights, Dictionary<string, (double time, int count)> filterStatsLights, int totalImageCountFlats, Dictionary<string, int> filterStatsFlats)
        {
            AppendText("LIGHTS Aggregated Results:\n", Color.DarkRed, new Font("Arial", 12, FontStyle.Bold));
            AppendText($"Total Imaging Time: {FormatTime(totalImagingTimeLights)}\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            AppendText($"Total Number of Images: {totalImageCountLights}\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            csvData.Add("LIGHTS Aggregated Results");
            csvData.Add($"Total Imaging Time,{FormatTime(totalImagingTimeLights)}");
            csvData.Add($"Total Number of Images,{totalImageCountLights}");
            AppendText("Filters:\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            csvData.Add("Filter,Time,Count");
            foreach (var kvp in filterStatsLights)
            {
                AppendText($"- {kvp.Key}: {FormatTime(kvp.Value.time)} ({kvp.Value.count} images)\n", Color.Green, new Font("Arial", 10, FontStyle.Regular));
                csvData.Add($"{kvp.Key},{FormatTime(kvp.Value.time)},{kvp.Value.count}");
            }
            AppendText("\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));

            AppendText("FLATS Aggregated Results:\n", Color.DarkRed, new Font("Arial", 12, FontStyle.Bold));
            AppendText($"Total Number of Images: {totalImageCountFlats}\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            csvData.Add("FLATS Aggregated Results");
            csvData.Add($"Total Number of Images,{totalImageCountFlats}");
            AppendText("Filters:\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
            csvData.Add("Filter,Count");
            foreach (var kvp in filterStatsFlats)
            {
                AppendText($"- {kvp.Key}: {kvp.Value} images\n", Color.Green, new Font("Arial", 10, FontStyle.Regular));
                csvData.Add($"{kvp.Key},{kvp.Value}");
            }
            AppendText("\n", Color.Black, new Font("Arial", 10, FontStyle.Regular));
        }

        private void UpdateProgressBar(double totalImagingTimeLights)
        {
            var percentage = (totalImagingTimeLights / TargetSeconds) * 100;
            progressBar.Value = Math.Min((int)percentage, 100);
            lblProgress.Text = $"{currentTargetName}: {percentage:F2}% of target complete";
        }

        private string FormatTime(double seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return $"{(int)timeSpan.TotalHours:D2} Hours and {timeSpan.Minutes:D2} Minutes";
        }

        private double ExtractExposureFromFileName(string fileName)
        {
            var onlyFileName = Path.GetFileName(fileName);
            var match = Regex.Match(onlyFileName, @"_(\d+(\.\d+)?)sec_", RegexOptions.IgnoreCase);
            if (match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var exposureTime))
            {
                return exposureTime;
            }
            return 0;
        }

        private string ExtractFilterFromFileName(string fileName)
        {
            var onlyFileName = Path.GetFileName(fileName);
            var match = Regex.Match(onlyFileName, @"_FILTER_([A-Za-z0-9\-]+)_", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        private void AppendText(string text, Color color, Font font)
        {
            rtxtResults.SelectionStart = rtxtResults.TextLength;
            rtxtResults.SelectionLength = 0;

            rtxtResults.SelectionColor = color;
            rtxtResults.SelectionFont = font;
            rtxtResults.AppendText(text);
            rtxtResults.SelectionColor = rtxtResults.ForeColor;
        }
    }
}
