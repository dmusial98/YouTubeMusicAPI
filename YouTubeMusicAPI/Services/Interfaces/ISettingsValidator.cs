using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.Services.Interfaces
{
    public interface ISettingsValidator
	{
		public Task<SettingsValidationResults> ValidateSettings(Settings settings);
	}
}
