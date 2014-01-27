using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AnimuTorrentChecker.Models;

using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using AnimuTorrentChecker.Utilities;

namespace AnimuTorrentChecker
{
    /// <summary>
    /// TODO:
    ///     1. Refactor and create settings. Done.
    ///     2. Create dropbox folders and test again. Done.
    ///     3. Create flag for simple error checking. Done.
    ///     4. Parse the rss response to get download link. Done.
    ///     5. Download to torrent path. Done.
    /// </summary>
    class Program
    {
        static string LINK_NOT_FOUND = "link-not-found";

        static Timer ActiveTimer;
        static bool IsActive; 
        static DateTime DateUpdated;
        static AnimuSeries Series;

        static void CheckAnimuSeries(object state)
        {
            //Don't do current if there's already a thread
            // NOTE: I think this is not needed because timer is not concurrent? Go read more
            if (IsActive == true) return;
            else IsActive = true;

            // Check all animu that is exists
            int length = Series.AnimuData.Length;
            for(int i=0;i<length;i++)
            {
                Animu animu = Series.AnimuData[i];
                string link, title;

                bool found = CheckAnimu(animu, out link, out title);
                if (found)
                {
                    bool downloaded = DownloadTorrent(title, link);
                    if (downloaded)
                    {
                        // Iterate episode number
                        Series.AnimuData[i].Episode = animu.Episode + 1;

                        // Update database
                        Series.Save();
                    }
                }

                Console.WriteLine("");
            }

            // Display result
            DateUpdated = DateTime.Now;
            Console.WriteLine("Last Update: " + DateUpdated);
            Console.Write(Series.ToString());
            Console.WriteLine("");

            // Disable lock
            IsActive = false;
        }

        static bool CheckAnimu(Animu animu, out string result_link, out string result_title)
        {
            result_link = LINK_NOT_FOUND;
            result_title = "";

            //Main search engine is nyaa
            StringBuilder sb = new StringBuilder();
            sb.Append("http://www.nyaa.se/?page=rss&term=");
            sb.Append(animu.Subber);
            sb.Append("+");
            string title = animu.Title.Replace(' ', '+');
            sb.Append(title);
            sb.Append("+");
            string episode = (animu.Episode < 10) ? "0" + animu.Episode : animu.Episode.ToString();
            sb.Append(episode);
            string url = sb.ToString();

            try
            {
                Console.WriteLine("Checking animu: " + url);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = Settings.Instance.SecondsToTimeout * 1000;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.Load(reader);

                        string episode_string = animu.FormattedString();

                        XmlNodeList nodes = doc.SelectNodes("/rss/channel/item");
                        XmlNode found_node = null;

                        foreach (XmlNode node in nodes)
                        {
                            XmlNode node_title = node.SelectSingleNode("title");

                            string title_text = node_title.InnerText;
                            title_text = title_text.Replace("_", " ");
                            bool resolution_found = false;

                            if (title_text.Contains(episode_string))
                            {
                                //Find resolution
                                if (title_text.Contains("480") || title_text.Contains("720") || title_text.Contains("1080"))
                                    resolution_found = true;

                                //Go download if there's no resolution (Note: usually Commie) or if 720p
                                if (!resolution_found || (resolution_found && title_text.Contains("720")))
                                {
                                    found_node = node;
                                    break;
                                }
                            }
                        }

                        if (found_node != null)
                        {
                            result_link = found_node.SelectSingleNode("link").InnerText;
                            result_title = found_node.SelectSingleNode("title").InnerText;

                            Console.WriteLine("Torrent Name: " + result_title);
                            Console.WriteLine("Torrent Link: " + result_link);

                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Not found..");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Parsing error: " + e.Message);
                    }

                    //Console.WriteLine("Response: \n" + reader.ReadToEnd());
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Checking error: " + e.Message);
            }

            return false;
        }

        static bool DownloadTorrent(string title, string link)
        {
            using (WebDownload webdownload = new WebDownload(Settings.Instance.SecondsToTimeout * 1000))
            {
                try
                {
                    string path = Settings.Instance.TorrentDownloadFolder + title + ".torrent";
                    webdownload.DownloadFile(link, @path);

                    Console.WriteLine("Downloaded: " + path);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Download error: " + e.Message);
                }
            }

            return false;
        }

        static void Main(string[] args)
        {
            Settings.Instance.Load();

            Series = new AnimuSeries();
            Series.Load();

            IsActive = false;

            ActiveTimer = new Timer(CheckAnimuSeries, null, TimeSpan.Zero, TimeSpan.FromMinutes(Settings.Instance.MinutesToUpdate));
            //ActiveTimer = new Timer(CheckAnimuSeries, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            Console.ReadKey(true);
        }
    }
}
