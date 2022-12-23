using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Bloom.Shared.Setting
{
    /// <summary>
    /// Media.cs への設定
    /// </summary>
    public static class Media
    {
        public static JsonSerializerOptions options1 = new JsonSerializerOptions();
    }
    /// <summary>
    /// 日時関係の設定
    /// </summary>
    public static class Date
    {
        /// <summary>
        /// 開催日時
        /// </summary>
        public static DateTime held = new DateTime(2022,09,23);
    }
}
