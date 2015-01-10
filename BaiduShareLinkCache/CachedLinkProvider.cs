using BaiduPanApi.Agent;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduShareLinkCache
{
    public class CachedLinkProvider
    {
        #region Singleton
        private class SingletonHelper
        {
            static SingletonHelper() { }
            internal static readonly CachedLinkProvider _instance = new CachedLinkProvider();
        }

        public static CachedLinkProvider Instance
        {
            get
            {
                return SingletonHelper._instance;
            }
        }
        private CachedLinkProvider()
        {
            //Pre init codes come here
        }
        #endregion

        private const string BDUSS = "DM4aTBzTTlrS29uUkFMNkY5YmpTMEJlVlVSZn5TY2dWY0t0cm5KbEpJRWE1dFJVQVFBQUFBJCQAAAAAAAAAAAEAAABIEo1NU2NvdHRUZXN0MDgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpZrVQaWa1UR";
        BaiduPanAgent agent = new BaiduPanAgent(BDUSS);
        public static MongoCollection<CachedLink> Collection
        {
            get
            {
                return Database.Instance.GetCollection<CachedLink>("share_link");
            }
        }

        public List<DirectLink> GetDirectLinks(string shareLink)
        {
            DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc); //Convert to utc time;
            CachedLink cl = Collection.FindOne(Query<CachedLink>.EQ(l => l.ShareLink, shareLink));
            if (cl == null || (now - cl.CreatedDate).TotalHours > 8) //direct link expires in 8 hours
            {
                Console.WriteLine("Direct links not found or expired in cache for share link " + shareLink + ", retrieving from Baidu Pan...");
                var response = agent.GetDirectDownloadLink(shareLink);
                if (response.ErrorNumber == 0 && response.FileList != null && response.FileList.Count > 0)
                {
                    List<DirectLink> directLinks = new List<DirectLink>();
                    foreach (var file in response.FileList)
                    {
                        var directLink = new DirectLink() { Url = file.DownloadLink, FileName = file.ServerFileName };
                        directLinks.Add(directLink);
                    }

                    if (cl == null) cl = new CachedLink() { ShareLink = shareLink };
                    cl.CreatedDate = now;
                    cl.DirectLinks = directLinks;
                    Collection.Save(cl);
                    Console.WriteLine("Direct links cached.");
                }else{
                    Console.WriteLine("[Error] Failed to retrieve direct link from baidu pan: " + response.ErrorNumber.ToString() + ".");
                }
            }
            
            if(cl != null)
            {
                return cl.DirectLinks;
            }
            else
            {
                return null;
            }
        }
    }
}
