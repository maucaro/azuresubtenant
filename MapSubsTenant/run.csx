#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{ 
    log.Info($"Webhook was triggered!");
    string jsonContent = await req.Content.ReadAsStringAsync();
    List<SubscriptionObject> data = JsonConvert.DeserializeObject<List<SubscriptionObject>>(jsonContent);
    if (data == null || data.Count == 0) {
        return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject("Please pass non-null SubsriptionGuid array in the request body."));
    }
    log.Info($"Processing {data.Count.ToString()} subscription(s).");
    foreach (SubscriptionObject sg in data) {
        sg.AADTenant = await ProcessTenant.GetSubscriptionTenantIdAsync(sg.SubscriptionGuid); 
    }
    return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(data));
}

public class SubscriptionObject
{
    public string SubscriptionGuid { get; set; }
    public string AADTenant { get; set; }
}

public class ProcessTenant {
    static HttpClient httpClient;
    
    static ProcessTenant() {
        httpClient = new System.Net.Http.HttpClient();
    }
    
    public static async Task<string> GetSubscriptionTenantIdAsync(string subscriptionId)
    {
        string tenantId = string.Empty;
        string url = $"https://management.azure.com/subscriptions/{subscriptionId}?api-version=2016-01-01";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        var headers = response.Headers;
        IEnumerable<string> values;
        if (headers.TryGetValues("WWW-Authenticate", out values))
        {
            string header = values.First();
            tenantId = header.Substring(52,36); //This seems brittle but works as implemented today; tested with over 500 subscriptions.
        }
        return tenantId; 
    }
}
