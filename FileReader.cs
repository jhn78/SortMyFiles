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
            var found = ReadFiles(cmd.RootPath);

            if (found.Count() == 0)  
                Console.WriteLine($"no files found at {cmd.RootPath}");

            foreach (var f in found)
                yield return f;

            yield return new SourceFilesRead() { CorrelationId = cmd.CorrelationId };
        }

        IEnumerable<FileFound> ReadFiles(string root)
        {
            foreach (var file in Directory.GetFiles(root, "*.*").ToList())
                yield return new FileFound() { File = new FileInfo() { Name = Path.GetFileName(file), Path = root }, CorrelationId = Guid.NewGuid() };

            foreach (var dir in Directory.GetDirectories(root))
                foreach (var found in ReadFiles(dir))
                    yield return found;
        }
    }
}
