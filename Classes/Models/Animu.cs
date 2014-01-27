using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace AnimuTorrentChecker.Models
{
    public class Animu
    {
        public string Title;
        public int Episode; // Assume max episode is 99 (two digits)
        public string Subber;

        public Animu()
        {
            this.Title = "";
            this.Episode = -1;
            this.Subber = "";
        }

        public Animu(string title, int episode, string subber)
        {
            this.Title = title;
            this.Episode = episode;
            this.Subber = subber;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Subber);
            sb.Append(" ");
            sb.Append(this.Title);
            sb.Append(" ");
            string episode = (this.Episode < 10) ? "0" + this.Episode : this.Episode.ToString();
            sb.Append(episode);

            return sb.ToString();
        }

        public string FormattedString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(this.Subber);
            sb.Append("] ");
            sb.Append(this.Title);
            sb.Append(" - ");
            string episode = (this.Episode < 10) ? "0" + this.Episode : this.Episode.ToString();
            sb.Append(episode);

            return sb.ToString();
        }
    }

    public class AnimuSeries
    {
        public Animu[] AnimuData;

        public AnimuSeries()
        {
            AnimuData = new Animu[1];
            AnimuData[0] = new Animu("Animu Title Here", 0, "Animu Subber Here");
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AnimuSeries));
            TextWriter textWriter = new StreamWriter(@Settings.Instance.AnimeListPath);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        public void Load()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(AnimuSeries));
            TextReader textReader = new StreamReader(@Settings.Instance.AnimeListPath);

            AnimuSeries loaded;
            loaded = (AnimuSeries)deserializer.Deserialize(textReader);
            textReader.Close();

            this.AnimuData = loaded.AnimuData;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("Anime List:");
            sb.Append(" (updated every ");
            sb.Append(Settings.Instance.MinutesToUpdate);
            sb.Append(" minutes)\n");

            foreach (Animu a in AnimuData)
            {
                sb.Append("\t");
                sb.Append(a.Title);
                sb.Append(" ");
                sb.Append(a.Episode);
                sb.Append(" ");
                sb.Append(a.Subber);
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
