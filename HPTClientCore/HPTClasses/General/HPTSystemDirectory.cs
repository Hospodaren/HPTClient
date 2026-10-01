using System.Collections.ObjectModel;

namespace HPTClient
{
    public class HPTSystemDirectory
    {
        public string DirectoryName { get; set; }

        public string DirectoryNameShort { get; set; }

        public ObservableCollection<HPTSystemFile> FileList { get; set; }

        //public List<HPTSystemFile> ATGXMLFileList { get; set; }

        //public List<HPTSystemFile> HPTSystemFileList { get; set; }
    }

    public class HPTSystemFile : Notifier
    {
        public string FileName { get; set; }

        public string FileNameShort { get; set; }

        public string FileType { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreationTime { get; set; }

        public System.Windows.Controls.Image FileTypeImage { get; set; }

        public string IconPath { get; set; }
    }
}
