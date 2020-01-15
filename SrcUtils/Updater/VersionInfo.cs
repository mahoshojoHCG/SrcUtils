using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HCGStudio.SrcUtils.Updater
{

    public class VersionInfo
    {
        public string Channel { get; set; }
        public string Version { get; set; }
        public string ChangeLog { get; set; }
        public string DownloadLink { get; set; }


        public static async Task<VersionInfo> GetLeastVersionAsync()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdater");
            var response = await http
                .GetAsync("https://raw.githubusercontent.com/mahoshojoHCG/SrcUtils/master/least.json")
                .ConfigureAwait(false);
            var str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<VersionInfo>(str);
        }
    }

}
