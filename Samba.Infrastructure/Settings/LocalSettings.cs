﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Samba.Infrastructure.Settings
{
    public class SettingsObject
    {
        public int MessagingServerPort { get; set; }
        public string MessagingServerName { get; set; }
        public string TerminalName { get; set; }
        public string ConnectionString { get; set; }
        public bool StartMessagingClient { get; set; }
        public string LogoPath { get; set; }
        public string DefaultHtmlReportHeader { get; set; }
        public string CurrentLanguage { get; set; }

        public SettingsObject()
        {
            MessagingServerPort = 8080;
            ConnectionString = "";
            DefaultHtmlReportHeader =
                @"
<style type='text/css'> 
html
{
  font-family: 'Courier New', monospace;
} 
</style>";
        }
    }

    public static class LocalSettings
    {
        private static SettingsObject _settingsObject;

        public static int Decimals { get { return 2; } }

        public static int MessagingServerPort
        {
            get { return _settingsObject.MessagingServerPort; }
            set { _settingsObject.MessagingServerPort = value; }
        }

        public static string MessagingServerName
        {
            get { return _settingsObject.MessagingServerName; }
            set { _settingsObject.MessagingServerName = value; }
        }

        public static string TerminalName
        {
            get { return _settingsObject.TerminalName; }
            set { _settingsObject.TerminalName = value; }
        }

        public static string ConnectionString
        {
            get { return _settingsObject.ConnectionString; }
            set { _settingsObject.ConnectionString = value; }
        }

        public static bool StartMessagingClient
        {
            get { return _settingsObject.StartMessagingClient; }
            set { _settingsObject.StartMessagingClient = value; }
        }

        public static string LogoPath
        {
            get { return _settingsObject.LogoPath; }
            set { _settingsObject.LogoPath = value; }
        }

        public static string DefaultHtmlReportHeader
        {
            get { return _settingsObject.DefaultHtmlReportHeader; }
            set { _settingsObject.DefaultHtmlReportHeader = value; }
        }

        private static CultureInfo _cultureInfo;
        public static string CurrentLanguage
        {
            get { return _settingsObject.CurrentLanguage; }
            set
            {
                _settingsObject.CurrentLanguage = value;
                _cultureInfo = CultureInfo.GetCultureInfo(value);
                UpdateThreadLanguage();
            }
        }

        public static string AppPath { get; set; }
        public static string DocumentPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SambaPOS2"; } }
        public static string DataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Ozgu Tech\\SambaPOS2"; } }
        public static string SettingsFileName { get { return DataPath + "\\SambaSettings.txt"; } }

        public static string DefaultCurrencyFormat { get; set; }

        public static int DbVersion { get { return 7; } }
        public static string AppVersion { get { return "2.10"; } }
        public static string[] SupportedLanguages { get { return new[] { "tr", "en" }; } }

        public static long CurrentDbVersion { get; set; }
        public static void SaveSettings()
        {
            var serializer = new XmlSerializer(_settingsObject.GetType());
            var writer = new XmlTextWriter(SettingsFileName, null);
            try
            {
                serializer.Serialize(writer, _settingsObject);
            }
            finally
            {
                writer.Close();
            }
        }

        public static void LoadSettings()
        {
            _settingsObject = new SettingsObject();
            string fileName = SettingsFileName;
            if (File.Exists(fileName))
            {
                var serializer = new XmlSerializer(_settingsObject.GetType());
                var reader = new XmlTextReader(fileName);
                try
                {
                    _settingsObject = serializer.Deserialize(reader) as SettingsObject;
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        static LocalSettings()
        {
            if (!Directory.Exists(DocumentPath))
                Directory.CreateDirectory(DocumentPath);
            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);
            LoadSettings();
        }

        public static void UpdateThreadLanguage()
        {
            Thread.CurrentThread.CurrentCulture = _cultureInfo;
            Thread.CurrentThread.CurrentUICulture = _cultureInfo;
        }
    }
}
