﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Mobile.ConfigurationManager
{
    public partial class ConfigurationManager
    {
        private static bool _initialized;
        private static ReadOnlyDictionary<string, ConnectionStringSettings> _connectionStrings;
        private static NameValueCollection _appSettings;

        // ReSharper disable InconsistentNaming
        private const string APP_SETTINGS = "appSettings";
        private const string CONNECTION_STRING = "connectionString";
        private const string CONNECTION_STRINGS = "connectionStrings";
        private const string KEY = "key";
        private const string NAME = "name";
        private const string PROVIDER_NAME = "providerName";
        private const string VALUE = "value";
        // ReSharper enable InconsistentNaming

        private ConfigurationManager() { /* Can't construct this, static access only. */}

        public static NameValueCollection AppSettings
        {
            get
            {
                EnsureInitialized();
                return _appSettings;
            }
            private set => _appSettings = value;
        }

        public static ReadOnlyDictionary<string, ConnectionStringSettings> ConnectionStrings
        {
            get
            {
                EnsureInitialized();
                return _connectionStrings;
            }
            private set => _connectionStrings = value;
        }

        private static void InitInternal(TextReader streamReader)
        {
            using (var reader = XmlReader.Create(streamReader))
            {
                var xDocument = XDocument.Load(reader);
                var xNodes = xDocument.Nodes();
                if (xNodes.Count() <= 1) return;
                var appSettings = xDocument.Descendants()
                    .Where(t => t.Name == APP_SETTINGS)
                    .Elements()
                    .ToList()
                    .Select(GenerateKeyValueFromItem)
                    .Where(i => !string.IsNullOrWhiteSpace(i.Key))
                    .ToList();

                var connectionStrings = xDocument.Descendants()
                    .Where(t => t.Name == CONNECTION_STRINGS)
                    .Elements()
                    .ToDictionary(xElement => xElement.Attribute(KEY)?.Value.ToString(), GenerateConnectionStringSettingsFromItem);

                AppSettings = new NameValueCollection(appSettings);
                ConnectionStrings = new ReadOnlyDictionary<string, ConnectionStringSettings>(connectionStrings);
            }
        }

        private static ConnectionStringSettings GenerateConnectionStringSettingsFromItem(XElement xElement) => new ConnectionStringSettings(xElement.Attribute(NAME)?.Value,
            xElement.Attribute(PROVIDER_NAME)?.Value,
            xElement.Attribute(CONNECTION_STRING)?.Value);

        private static KeyValuePair<string, string> GenerateKeyValueFromItem(XElement item) => new KeyValuePair<string, string>(
            item?.Attribute(KEY)?.Value,
            item?.Attribute(VALUE)?.Value);

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            
            // if the user hasn't specified their own initialization, we will initialize with defaults.
            Init(); 
            _initialized = true;
        }
    }
}