namespace Copy9.Models
{
    public class Actor
    {
        public string NewBaseDir { get; set; }
        public string OldBaseDir { get; set; }
        public string SourceType { get; set; }
        public string TargetType { get; set; }
        public Connector SourceConnector { get; set; }
        public DirectoryEngine SourceDirectoryEngine { get; set; }
        public Filer SourceFiler { get; set; }
        public Connector TargetConnector { get; set; }
        public DirectoryEngine TargetDirectoryEngine { get; set; }
        public Filer TargetFiler { get; set; }
        public Interrupt Interrupt
        {
            get
            {
                Interrupt interrupt = new Interrupt("OK");
                if (SourceDirectoryEngine.Interrupt != null)
                {
                    interrupt = SourceDirectoryEngine.Interrupt;
                }
                else if (SourceDirectoryEngine.Interrupt != null)
                {
                    interrupt = SourceDirectoryEngine.Interrupt;
                }
                return interrupt;
            }
            set
            {
                SourceDirectoryEngine.Interrupt = value;
                TargetDirectoryEngine.Interrupt = value;
            }
        }

        public Actor()
        {
            NewBaseDir = string.Empty;
            OldBaseDir = string.Empty;
            SourceType = "L";
            SourceConnector = null;
            SourceDirectoryEngine = new DirectoryEngineL();
            SourceFiler = new FilerL();
            TargetType = "L";
            TargetConnector = null;
            TargetDirectoryEngine = new DirectoryEngineL();
            TargetFiler = new FilerL();
        }

        public Actor(string newBaseDir, string oldBaseDir)
        {
            NewBaseDir = newBaseDir;
            OldBaseDir = oldBaseDir;
            if (newBaseDir.StartsWith("#"))
            {
                SourceType = "A";
                SourceConnector = new Connector(newBaseDir);
                SourceDirectoryEngine = new DirectoryEngineA(SourceConnector);
                SourceFiler = new FilerA(SourceConnector);
            }
            else if (newBaseDir.StartsWith("!") || newBaseDir.StartsWith("%"))
            {
                SourceType = "G";
                SourceConnector = new Connector(newBaseDir);
                SourceDirectoryEngine = new DirectoryEngineG(SourceConnector);
                SourceFiler = new FilerG(SourceConnector);
            }
            else
            {
                SourceType = "L";
                SourceConnector = new Connector(newBaseDir);
                SourceDirectoryEngine = new DirectoryEngineL();
                SourceFiler = new FilerL();
            }
            if (oldBaseDir.StartsWith("#"))
            {
                TargetType = "A";
                TargetConnector = new Connector(oldBaseDir);
                TargetDirectoryEngine = new DirectoryEngineA(TargetConnector);
                TargetFiler = new FilerA(TargetConnector);
            }
            else if (oldBaseDir.StartsWith("!") || oldBaseDir.StartsWith("%"))
            {
                TargetType = "G";
                TargetConnector = new Connector(oldBaseDir);
                TargetDirectoryEngine = new DirectoryEngineG(TargetConnector);
                TargetFiler = new FilerG(TargetConnector);
            }
            else
            {
                TargetType = "L";
                TargetConnector = new Connector(oldBaseDir);
                TargetDirectoryEngine = new DirectoryEngineL();
                TargetFiler = new FilerL();
            }
        }

        public void CopyFile(string sourceFileSpec, string targetFileSpec)
        {
            if (SourceType == "L" && TargetType == "L")
            {
                SourceFiler.CopyFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "L" && TargetType == "G")
            {
                PrepareTargetFile(targetFileSpec);
                TargetFiler.UploadFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "G" && TargetType == "L")
            {
                PrepareTargetFile(targetFileSpec);
                SourceFiler.DownloadFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "G" && TargetType == "G")
            {
                PrepareTargetFile(targetFileSpec);
                SourceFiler.CopyFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "L" && TargetType == "A")
            {
                PrepareTargetFile(targetFileSpec);
                TargetFiler.UploadFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "A" && TargetType == "L")
            {
                PrepareTargetFile(targetFileSpec);
                SourceFiler.DownloadFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "A" && TargetType == "A")
            {
                PrepareTargetFile(targetFileSpec);
                SourceFiler.CopyFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "G" && TargetType == "A")
            {
                //TODO: Won't work at the moment as A upload only works from L.
                PrepareTargetFile(targetFileSpec);
                TargetFiler.UploadFile(sourceFileSpec, targetFileSpec);
            }
            else if (SourceType == "A" && TargetType == "G")
            {
                //TODO: Won't work at the moment as A download only works from L.
                PrepareTargetFile(targetFileSpec);
                SourceFiler.DownloadFile(sourceFileSpec, targetFileSpec);
            }
        }

        public void PrepareTargetFile(string targetFileSpec)
        {
            TargetFiler.PathCheck(targetFileSpec);
            if (TargetFiler.FileExists(targetFileSpec))
            {
                TargetFiler.DeleteFile(targetFileSpec);
            }
        }
    }
}