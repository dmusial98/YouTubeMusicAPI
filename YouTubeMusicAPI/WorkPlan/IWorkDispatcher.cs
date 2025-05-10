using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.WorkPlan
{
	public interface IWorkDispatcher
	{
		WorkList PlanWork(SettingsValidationResults validationResults);
	}
}
