// OnPostprocessBuild() gets called after build has completed
// usage: copy to Editor/ folder in your project

using System.Diagnostics;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace CheesyUtils.EditorTools
{
    public class PostProcessBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        
        public void OnPostprocessBuild(BuildReport report)
        {
            var target = report.summary.platform;
            var path = report.summary.outputPath;
            
            Debug.Log("OnPostprocessBuild for target " + target + " at path " + path);

            // Run some process after successful build
            Process proc = new Process();
            proc.StartInfo.FileName = "C:/WINDOWS/system32/notepad.exe";
            proc.Start();
        }
    }
}
