using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    public interface ICommandHandler<ICommand, IEvent>
    {
        IEnumerable<IEvent> Handle(ICommand command);
    }

    public interface IEventHandler<IEvent, ICommand>
    {
        IEnumerable<ICommand> Handle(IEvent evt);
    }

    public class FileStorage : 
        IEventHandler<FileFound, FilterFile>,        
        IEventHandler<SourceFilesRead, CopyFiles>,
        IEventHandler<FileDateDetermined, PlaceFile>,
        IEventHandler<FileFiltered, AnalyzeFile>,
        IEventHandler<FilePlaced, HandleDuplicate>
    {
        Dictionary<Guid, FileInfo> store = new Dictionary<Guid, FileInfo>();

        public IEnumerable<PlaceFile> Handle(FileDateDetermined evt)
        {
            yield return new PlaceFile() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TakenAt = evt.FileDate };
        }

        public IEnumerable<HandleDuplicate> Handle(FilePlaced evt)
        {
            if (!evt.IsDuplicate) { 
                yield return null;
                yield break;
            }

            yield return new HandleDuplicate() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TargetFile = evt.Target };
        }

        public IEnumerable<AnalyzeFile> Handle(FileFiltered evt)
        {
            var file = store[evt.CorrelationId];

            if (evt.KeepFile) { 
                yield return new AnalyzeFile() { CorrelationId = evt.CorrelationId, File = file };
                yield break;
            }

            Console.WriteLine($"File ignored: {Path.Combine(file.Path,file.Name)}");

            store.Remove(evt.CorrelationId);

            yield break;
        }
        
        public IEnumerable<FilterFile> Handle(FileFound cmd)
        {
            store.Add(cmd.CorrelationId, cmd.File);

            yield return new FilterFile { File = cmd.File, CorrelationId = cmd.CorrelationId };
        }

        public IEnumerable<CopyFiles> Handle(SourceFilesRead cmd)
        {
            yield return new CopyFiles() { CorrelationId = cmd.CorrelationId };
        }
    }
}