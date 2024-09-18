using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.Services
{
    internal class SettingsReader : ISettingsReader
    {
        readonly string settingsFilePath = String.Concat(Directory.GetCurrentDirectory(), "settings.json");

        public async Task<Settings> ReadSettingsAsync()
        {
            var fileStr = await File.ReadAllTextAsync(settingsFilePath);
            return JsonSerializer.Deserialize<Settings>(fileStr);
        }
    }
}
