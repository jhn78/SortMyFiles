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
            var fm = new FilePlaceManager();

            Queue.Register<ReadFiles>().Subscribe(m => Queue.PublishAll(new FileReader().Handle(m)));
            Queue.Register<FileFound>().Subscribe(m => Queue.PublishAll(fs.Handle(m)));
            Queue.Register<FilterFile>().Subscribe(m => Queue.PublishAll(fp.Handle(m)));
            Queue.Register<FileFiltered>().Subscribe(m => Queue.PublishAll(fs.Handle(m)));
            Queue.Register<AnalyzeFile>().Subscribe(m => Queue.PublishAll(fp.Handle(m)));
            Queue.Register<FileDateDetermined>().Subscribe(m => Queue.PublishAll(fs.Handle(m)));
            Queue.Register<PlaceFile>().Subscribe(m => Queue.PublishAll(fm.Handle(m)));
            Queue.Register<FilePlaced>().Subscribe(m => Queue.PublishAll(fs.Handle(m)));
            Queue.Register<HandleDuplicate>().Subscribe(m => Console.WriteLine($"duplicate detected  {m.TargetFile.File.Name}"));
            Queue.Register<SourceFilesRead>().Subscribe(m => Queue.PublishAll(fs.Handle(m)));
            Queue.Register<CopyFiles>().Subscribe(m => Queue.PublishAll(fm.Handle(m)));
            Queue.Register<FilesCopied>().Subscribe(m => Console.WriteLine("done"));

            Queue.Publish(new ReadFiles() { RootPath = SourcePath });
            
            Console.ReadKey();
        }
    }
}
