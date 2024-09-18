using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.Services.Interfaces
{
    internal interface ISettingsReader
    {
        public Task<Settings> ReadSettingsAsync();

    }
}
