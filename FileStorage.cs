using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    public class FileStorage : 
        IEventHandler<FileFound>,        
        IEventHandler<SourceFilesRead>,
        IEventHandler<FileDateDetermined>,
        IEventHandler<FileDateNotDetermined>,
        IEventHandler<FileFiltered>,
        IEventHandler<FilePlaced>,
        IEventHandler<FilesCopied>
    {
        Dictionary<Guid, FileInfo> store = new Dictionary<Guid, FileInfo>();

        public IEnumerable<ICommand> Handle(FileDateDetermined evt)
        {
            yield return new PlaceFile() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TakenAt = evt.FileDate };
        }

        public IEnumerable<ICommand> Handle(FileDateNotDetermined evt)
        {
            yield return new PlaceFile() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TakenAt = null };
        }

        public IEnumerable<ICommand> Handle(FilesCopied evt)
        {
            evt.Files
                .ToList()
                .ForEach(f => store.Remove(f));

            if (store.Count() != 0)
                Console.WriteLine($"Files remain: {store.Count()}");

            Console.WriteLine("done");

            yield break;
        }
        
        public IEnumerable<ICommand> Handle(FilePlaced evt)
        {
            if (!evt.IsDuplicate) { 
                yield return null;
                yield break;
            }

            yield return new HandleDuplicate() { CorrelationId = evt.CorrelationId, File = store[evt.CorrelationId], TargetFile = evt.Target };
        }

        public IEnumerable<ICommand> Handle(FileFiltered evt)
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
        
        public IEnumerable<ICommand> Handle(FileFound cmd)
        {
            store.Add(cmd.CorrelationId, cmd.File);

            yield return new FilterFile { File = cmd.File, CorrelationId = cmd.CorrelationId };
        }

        public IEnumerable<ICommand> Handle(SourceFilesRead cmd)
        {
            yield return new CopyFiles() { CorrelationId = cmd.CorrelationId };
        }        
    }
}