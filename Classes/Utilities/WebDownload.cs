using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace AnimuTorrentChecker.Utilities
{
    public class WebDownload : WebClient
    {
        /// <summary>
        /// Milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownload() : this(60000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }

            return request;
        }
    }
}