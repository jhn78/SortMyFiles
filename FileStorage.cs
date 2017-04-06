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
        IEvent Handle(ICommand command);
    }

    public interface IEventHandler<IEvent, ICommand>
    {
        ICommand Handle(IEvent evt);
    }

    public class FileStorage : 
        IEventHandler<FileFound, FilterFile>,        
        IEventHandler<SourceFilesRead, CopyFiles>,
        IEventHandler<FileDateDetermined, PlaceFile>,
        IEventHandler<FileFiltered, AnalyzeFile>,
        IEventHandler<FilePlaced, HandleDuplicate>
    {
        Dictionary<Guid, FileInfo> store = new Dictionary<Guid, FileInfo>();

        public PlaceFile Handle(FileDateDetermined evt)
        {
            return new PlaceFile() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TakenAt = evt.FileDate };
        }

        public HandleDuplicate Handle(FilePlaced evt)
        {
            if (!evt.IsDuplicate)
                return null;

            Console.WriteLine("duplicate detected");

            return new HandleDuplicate() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TargetFile = evt.Target };
        }

        public AnalyzeFile Handle(FileFiltered evt)
        {
            var file = store[evt.CorrelationId];

            if (evt.KeepFile)
                return new AnalyzeFile() { CorrelationId = evt.CorrelationId, File = file };

            Console.WriteLine($"File ignored: {Path.Combine(file.Path,file.Name)}");

            store.Remove(evt.CorrelationId);

            return null;
        }
        
        public FilterFile Handle(FileFound cmd)
        {
            store.Add(cmd.CorrelationId, cmd.File);

            return new FilterFile { File = cmd.File, CorrelationId = cmd.CorrelationId };
        }

        public CopyFiles Handle(SourceFilesRead cmd)
        {
            return new CopyFiles() { CorrelationId = cmd.CorrelationId };
        }
    }
}