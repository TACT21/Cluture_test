using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bluem_of_youth.Shared
{
    public class VideoLink
    {
        public string title;
        public string url;
        public string thumbnail;
    }
    public class Video
    {
        public string title;
        public List<string> url;
        public string voteid;
    }
    public class LiveLink  : VideoLink
    {
        public bool tag;
        public string title;
        public string url;
        public string thumbnail;
    }

}
