using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Caching;
using System.IO;
using System.Data;

using MobyDickShowWords.Models;

namespace MobyDickShowWords.Controllers
{
    public class HomeController : Controller
    {
        private const string MainMenuCacheKey = "MobyDickText";

        /// <summary>
        /// auto reload flag to automatically reload the cached item on callback
        /// </summary>
        private const bool AutoLoadExpiry = true;

        /// <summary>
        /// the XML Document location (path)
        /// </summary>
        private static string XMLDocLocation = System.Web.HttpContext.Current.Server.MapPath("~/XML/MobyDickText.xml");

        /// <summary>
        /// expire the cache after x minutes
        /// </summary>
        private static int CacheExpiryMinutes = 10;

        public ActionResult Index()
        {
			string path= XMLDocLocation;//default
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
					path = Path.Combine(Server.MapPath("~/XML/"), fileName);
                    file.SaveAs(path);

                }
            }
           
            List<Words> words = ListofWords(path);

            return View(words);

        }

       public static List<Words> ListofWords(string path)  
		{  
			var ds=MainMenuXML();

			List<Words> words = new List<Words>();
            int i = 0;
			for (i = 0; i <= ds.Tables[0].Rows.Count-1;i++)
			{
             
				Words word= new Words();
                word.text = ds.Tables[0].Rows[i].ItemArray[0].ToString();
				word.count=Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[1]);
				words.Add(word);

			}
			return words.OrderByDescending(o=>o.count).Take(10).ToList();
		}

        private static DataSet MainMenuXML()
        {
            var objXml = System.Web.HttpContext.Current.Cache.Get(MainMenuCacheKey);
            if (objXml == null)
                return LoadXDocToCache();
            else if (!objXml.GetType().Equals(typeof(DataSet)))
                return LoadXDocToCache();
            else
                return (DataSet)objXml;
		}

        private static DataSet LoadXDocToCache()
        {
            var ds = new DataSet();
            ds.ReadXml(XMLDocLocation);
            System.Web.HttpContext.Current.Cache.Insert(
                MainMenuCacheKey,
                ds,
                new CacheDependency(XMLDocLocation),
                DateTime.Now.AddMinutes(CacheExpiryMinutes),
                TimeSpan.Zero,
                CacheItemPriority.Default,
                new CacheItemRemovedCallback(XMLDocumentRemoved));
            return ds;
        }

        private static void XMLDocumentRemoved(string Key, object Value, CacheItemRemovedReason Reason)
        {
            if (Key.Equals(MainMenuCacheKey, StringComparison.InvariantCultureIgnoreCase) && AutoLoadExpiry)
                LoadXDocToCache();
        }
	
    }
}
