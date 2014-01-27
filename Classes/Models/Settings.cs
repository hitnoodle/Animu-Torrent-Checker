using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace AnimuTorrentChecker.Models
{
    public class Settings
    {
        static public Settings Instance = new Settings();

        public int MinutesToUpdate;
        public int SecondsToTimeout;
        public string AnimeListPath;
        public string TorrentDownloadFolder;

        public Settings()
        {
            MinutesToUpdate = 15;
            SecondsToTimeout = 30;
            AnimeListPath = "AnimuSeries.xml";
            TorrentDownloadFolder = "Torrents";
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter textWriter = new StreamWriter("Settings.xml");
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        public void Load()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
            TextReader textReader = new StreamReader("Settings.xml");

            Settings loaded;
            loaded = (Settings)deserializer.Deserialize(textReader);
            textReader.Close();

            this.MinutesToUpdate = loaded.MinutesToUpdate;
            this.SecondsToTimeout = loaded.SecondsToTimeout;
            this.AnimeListPath = loaded.AnimeListPath;
            this.TorrentDownloadFolder = loaded.TorrentDownloadFolder;
        }
    }
}
