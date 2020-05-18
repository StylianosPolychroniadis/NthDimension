// Requires C# 5.0
#define CS5

namespace NthStudio.Compiler.VisualStudio.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class NuGetApi
    {
#if CS5
        public static async Task<string> GetJson(string uriString)
        {
            Console.WriteLine(string.Format("GetJson: {0}", uriString));
            var client = new HttpClient();
            client.BaseAddress = new Uri(uriString);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync("");
           
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            }
            return null;
        }

        public static async Task<IList<string>> GetPackageVersions(string urlIndex, string packageName)
            //Task<List<string>> GetPackageVersions(string urlIndex, string packageName)
        {
            Console.WriteLine(string.Format("GetPackageVersions: {0}", urlIndex));
            var jsonIndex = await GetJson(urlIndex);
            if (jsonIndex != null)
            {
                JObject objectIndex = JsonConvert.DeserializeObject<JObject>(jsonIndex);
                var urlTemplate = (string)objectIndex["resources"].FirstOrDefault(x => (string)x["@type"] == "PackageBaseAddress/3.0.0")["@id"];
                Console.WriteLine(string.Format("Template: {0}", urlTemplate));

                var urlVersions = string.Format("{0}{1}/index.json", urlTemplate, packageName);
                Console.WriteLine(string.Format("Versions: {0}", urlVersions));
                var jsonVersions = await GetJson(urlVersions);
                if (jsonVersions != null)
                {
                    JObject objectVersions = JsonConvert.DeserializeObject<JObject>(jsonVersions);
                    //List<string> versions = objectVersions["versions"].Select(x => (string)x).ToList();
                    var versions = objectVersions["versions"].Select(x => (string)x).ToList();
                    Console.WriteLine(string.Format("Latest Version: {0}: {1}", packageName, versions.LastOrDefault()));
                    return versions;
                }
            }
            return null;
        }
#endif
    }
}
