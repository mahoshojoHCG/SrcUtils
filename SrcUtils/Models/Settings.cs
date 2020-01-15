using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HCGStudio.SrcUtils.Models
{
    public enum AutoThumbnailSize
    {
        Tiny = 64,
        Small = 128,
        Medium = 256,
        Large = 512
    }

    public enum DataBaseProvider
    {
        SqlLite,
        SqlServer
    }
    public class Settings
    {
        public bool UseAutoThumbnail { get; set; } = true;
        public AutoThumbnailSize AutoThumbnailSize { get; set; } = AutoThumbnailSize.Medium;
        //Store this as string in json
        [JsonConverter(typeof(StringEnumConverter))]
        public DataBaseProvider DataBaseProvider { get; set; } = DataBaseProvider.SqlLite;
        public string SqlLiteDataSource { get; set; } = "default.db";
        public string ConnectionStringWhenNotUsingSqlLite { get; set; }

    }
}
