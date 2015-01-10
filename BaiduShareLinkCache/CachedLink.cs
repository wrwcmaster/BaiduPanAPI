using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduShareLinkCache
{
    public class CachedLink
    {
        public ObjectId Id { get; set; }

        public string ShareLink { get; set; }

        public List<DirectLink> DirectLinks { get; set; }

        public DateTime CreatedDate { get; set; }

    }

    public class DirectLink
    {
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
