using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    public abstract class IMessage
    {
        public Guid CorrelationId { get; set; }
    }

    public abstract class IEvent : IMessage
    {

    }

    public abstract class ICommand : IMessage
    {

    }
    
    public class FileFound : IEvent
    {
        public FileInfo File { get; set; }
    }

    public class FileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class TargetFileInfo
    {
        public FileInfo File { get; set; }
        public DateTime SortDate { get; set; }                        
    }

    public class ReadFiles : ICommand
    {
        public string RootPath { get; set; }
    }
    
    public class FileDateDetermined : IEvent
    {
        public DateTime? FileDate { get; set; }
    }

    public class AnalyzeFile : ICommand
    {
        public FileInfo File { get; set; }
    }

    public class PlaceFile : ICommand
    {
        public FileInfo File { get; set; }
        public DateTime? TakenAt { get; set; }
    }

    public class FilePlaced : IEvent
    {
        public TargetFileInfo Target { get; set; }
        public bool IsDuplicate { get; set; }
    }

    public class SourceFilesRead : IEvent
    {   
    }

    public class CopyFiles : ICommand
    {
        public DateTime Date { get; set; }
    }

    public class FilesCopied : IEvent
    {
        public IEnumerable<Guid> Files { get; set; }
    }

    public class FilterFile : ICommand
    {
        public FileInfo File { get; set; }
    }

    public class FileFiltered : IEvent
    {
        public bool KeepFile { get; set; }
    }

    public class HandleDuplicate : ICommand
    {
        public FileInfo File { get; set; }
        public TargetFileInfo TargetFile { get; set; }
    }
}
