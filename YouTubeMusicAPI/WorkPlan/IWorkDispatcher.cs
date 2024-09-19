using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.WorkPlan
{
    public interface IWorkDispatcher
    {
        public WorkList PlanWork(SettingsValidationResults validationResults);

    public void SaveUrlsInFile();
    }
}
