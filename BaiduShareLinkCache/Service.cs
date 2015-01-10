using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace BaiduShareLinkCache
{
    [ServiceContract]
    public class Service
    {
        [OperationContract]
        [WebGet]
        public List<DirectLink> GetDirectLink(string shareLink)
        {
            return CachedLinkProvider.Instance.GetDirectLinks(shareLink);
        }
    }
}
