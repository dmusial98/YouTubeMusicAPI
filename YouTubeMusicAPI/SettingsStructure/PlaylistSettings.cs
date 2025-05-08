namespace YouTubeMusicAPI.SettingsStructure
{
	public class PlaylistSettings
	{
		public string? name { get; set; }
		public string? path { get; set; }
        public Urls? urls { get; set; }
		public Download? download { get; set; }
		//public DislikeForBadUrls? dislikeForBadUrls { get; set; }
		public bool renameFiles { get; set; }
	}
}
