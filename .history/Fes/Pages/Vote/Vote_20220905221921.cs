using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Fes.Shared;
using System.Collections.Generic;

namespace Fes.Pages.Vote
{
    public class Vote : ComponentBase
    {
        public async Task<string> Get_Mac()
        {
            var list = new List<PhysicalAddress>();

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
                        list.Add(adapter.GetPhysicalAddress());
                    }
                }
            }

            // 取得したMACアドレスの一覧を表示する
            foreach (var obj in list)
            {
                Console.WriteLine(obj.ToString());
            }
        }
    }
}
