using System.Collections.Generic;

namespace eTurns.DTO
{

    public class FileOrFolderInfo
    {
        public string DestPath { get; set; }
        public string SourcePath { get; set; }
    }
    public class Project
    {
        public List<FileOrFolderInfo> Files { get; set; }
        public List<FileOrFolderInfo> Folders { get; set; }
        public bool IncludeInPackage { get; set; }
        public string Name { get; set; }
        public string WebWinSvcAppName { get; set; }
        public string ProjectType { get; set; }
        public bool IncludeInDeployment { get; set; }

    }

    public class eTrunsPub
    {
        public List<Project> Projects { get; set; }
    }


}
