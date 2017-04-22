using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    class Program
    {
        static string SourcePath = @"e:\_test_\in";
        //static string SourcePath = @"e:\_privat_\_bilder_\";
        public static Queue Queue;

        static void Main(string[] args)
        {
            Queue = new Queue();

            var id = Guid.NewGuid();
            
            var fs = new FileStorage();
            var fp = new FileProcessor();

            Queue.Register<ReadFiles>().Subscribe(m =>
            {
                var ff = new FileReader().Handle(m);

                if (ff.Count() == 0) { 
                    Console.WriteLine($"no files found at {SourcePath}");
                    return;
                }
                
                ff.ToList().ForEach(f => Queue.Publish(f));

                Queue.Publish(new SourceFilesRead() { CorrelationId = id });                
            });
            Queue.Register<FileFound>().Subscribe(m => Queue.Publish(fs.Handle(m)));
            Queue.Register<FilterFile>().Subscribe(m => Queue.Publish(fp.Handle(m)));
            Queue.Register<FileFiltered>().Subscribe(m => Queue.Publish(fs.Handle(m)));
            Queue.Register<AnalyzeFile>().Subscribe(m => Queue.Publish(fp.Handle(m)));
            Queue.Register<FileDateDetermined>().Subscribe(m => Queue.Publish(fs.Handle(m)));
            Queue.Register<PlaceFile>().Subscribe(m => Queue.Publish(FilePlaceManager.Handle(m)));
            Queue.Register<FilePlaced>().Subscribe(m => Queue.Publish(fs.Handle(m)));
            Queue.Register<HandleDuplicate>().Subscribe(m => Console.WriteLine($"duplicate detected  {m.TargetFile.File.Name}"));
            Queue.Register<SourceFilesRead>().Subscribe(m => Queue.Publish(fs.Handle(m)));
            Queue.Register<CopyFiles>().Subscribe(m => Queue.Publish(FilePlaceManager.Handle(m)));
            Queue.Register<FilesCopied>().Subscribe(m => Console.WriteLine("done"));

            Queue.Publish(new ReadFiles() { RootPath = SourcePath });

            Console.ReadKey();
        }
    }
}
