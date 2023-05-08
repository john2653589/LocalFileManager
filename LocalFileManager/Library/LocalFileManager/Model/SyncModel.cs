namespace Rugal.Net.LocalFileManager.Extention
{
    public class SyncDirectoryModel
    {
        public string Path { get; set; }
        public string FullPath { get; set; }
        public List<SyncFileModel> Files { get; set; }
        public List<SyncDirectoryModel> Directories { get; set; }

        internal bool TryGetDirectory(string FindPath, out SyncDirectoryModel OutDirectory)
        {
            OutDirectory = null;

            if (string.IsNullOrWhiteSpace(FindPath))
            {
                OutDirectory = this;
                return true;
            }

            FindPath = FindPath.ToLower();

            var GetDirectory = Directories
                .FirstOrDefault(Item => Item.Path.ToLower() == FindPath);

            if (GetDirectory is not null)
            {
                OutDirectory = GetDirectory;
                return true;
            }

            foreach (var Item in Directories)
            {
                if (Item.TryGetDirectory(FindPath, out var GetModel))
                {
                    OutDirectory = GetModel;
                    return true;
                }
            }

            return false;
        }
        internal bool IsFileExist(string FileName, string FindPath)
        {
            if (!TryGetDirectory(FindPath, out var OutDirectory))
                return false;

            var IsExist = OutDirectory.Files.Any(Item => Item.FileName == FileName);
            return IsExist;
        }
    }
    public class SyncFileModel
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
    }
    public class GetFileModel
    {
        public byte[] Buffer { get; set; }
    }
}