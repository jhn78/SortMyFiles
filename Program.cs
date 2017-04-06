using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    class Program
    {
        //static string SourcePath = @"e:\_test_\in";
        static string SourcePath = @"e:\_privat_\_bilder_\";

        static void Main(string[] args)
        {
            var id = Guid.NewGuid();

            var found = new FileReader().Handle(new ReadFiles() { CorrelationId = id, RootPath = SourcePath });

            if (found.Count() == 0)
                Console.WriteLine($"no files found at {SourcePath}");
            else
            { 
                var fs = new FileStorage();
                var fp = new FileProcessor();
            
                var placed = found
                    .Select(f => fs.Handle(f))
                    .Select(f => fp.Handle(f))
                    .Select(f => fs.Handle(f)).Where(f => f != null)
                    .Select(f => fp.Handle(f))
                    .Select(f => fs.Handle(f))
                    .Select(f => FilePlaceManager.Handle(f))
                    .Select(f => fs.Handle(f))
                    .ToList();
                                
                FilePlaceManager.Handle(fs.Handle(new SourceFilesRead() { CorrelationId = id }));
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
