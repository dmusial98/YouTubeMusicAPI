namespace YouTubeMusicAPI.WorkPlan
{
    public class WorkList
    {
        public PlaylistWorkList[] playlistWorkList { get; }

        public WorkList(PlaylistWorkList[] playlistWorkList)
        {
            this.playlistWorkList = playlistWorkList;
        }
    }
}