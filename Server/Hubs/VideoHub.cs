using Microsoft.AspNetCore.SignalR;
using bluem_of_youth.Shared;

namespace bluem_of_youth.Server.Hubs
{
    public class VideoHub : Hub
    {
        public async Task GetLinks(string id)
        {
            List<VideoLink> result = new List<VideoLink>();

            await Clients.Caller.SendAsync("SetLink", result);
        }
        public async Task GetVideos(string id)
        {
            Dictionary<string,List<VideoLink>> result = new Dictionary<string, List<VideoLink>>();

            await Clients.Caller.SendAsync("SetVideo", result);
        }
    }
}
