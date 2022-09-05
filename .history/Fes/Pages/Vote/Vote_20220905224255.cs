using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fes.Shared;
using System.Collections.Generic;
using System.Net.Http;

namespace Fes.Pages.Vote
{
    public class Vote : ComponentBase
    {
        [Parameter]
        public string? id {set;get;}
        public string orgname = "選択してください";
        private string mac;
        public async Task Vote(string award){
            HttpClient client = new HttpClient();
            while(true){
                if(mac != ""){
                    break;
                }
                await Task.Delay(1000);
            }
            var param = new Dictionary<string, object>()
                {
                    ["award"] = award,
                    ["to"] = id,
                    ["name"] = name,
                };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(param);
            var content = new StringContent(jsonString, Encoding.UTF8, @"application/json");
            //POST
            var result = await client.PostAsync(@"https://prod2-10.japaneast.logic.azure.com:443/workflows/69edb18e7b5d4c6e9d2bb99558f6d4b0/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=OvkgP68zZPP6oob6V1rLRPVlK-84qqRVftMpQuQpYhM", content);
            Console.WriteLine($"{(int)result.StatusCode},{ result.StatusCode }");
        }

        protected override async Task OnInitializedAsync()
        {
            mac = await NetworkHelper.Get_Mac();
        }
    }

    public static class NetworkHelper{
        public static async Task<string> Get_Mac()
        {
            PhysicalAddress adapt;
            // MACアドレスを取得する
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 有効なインターフェイスのみ対象とする
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    // 不明なインターフェイスとループバックインターフェイスを除外する
                    if ((adapter.NetworkInterfaceType != NetworkInterfaceType.Unknown) &&
                        (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                    {
                        adapt = adapter.GetPhysicalAddress();
                        break;
                    }
                }
            }
            return adapt.Tostring();
        }
    }
}
