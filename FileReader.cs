using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    class FileReader : ICommandHandler<ReadFiles>
    {
        public IEnumerable<IEvent> Handle(ReadFiles cmd)
        {
            foreach (var file in Directory.GetFiles(cmd.RootPath, "*.*").ToList())
                yield return new FileFound() { File = new FileInfo() { Name = Path.GetFileName(file), Path = cmd.RootPath }, CorrelationId = Guid.NewGuid() };
                            
            foreach (var dir in Directory.GetDirectories(cmd.RootPath))
                foreach (var found in Handle(new ReadFiles() { RootPath = dir, CorrelationId = cmd.CorrelationId }))
                    yield return found;            
        }
    }
}
