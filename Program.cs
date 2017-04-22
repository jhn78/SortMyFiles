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

            Register<ReadFiles>(new FileReader());
            Register<FileFound>(fs);
            Register<FilterFile>(fp);
            Register<FileFiltered>(fs);
            Register<AnalyzeFile>(fp);
            Register<FileDateDetermined>(fs);
            Register<FileDateNotDetermined>(fs);
            Register<PlaceFile>(fm);
            Register<FilePlaced>(fs);
            Register<HandleDuplicate>(fm);
            Register<SourceFilesRead>(fs);
            Register<CopyFiles>(fm);
            Register<FilesCopied>(fs);

            Queue.Publish(new ReadFiles() { RootPath = SourcePath });
            
            Console.ReadKey();
        }

        public static void Register<TIn>(IMessageHandler<TIn, IMessage> handler) where TIn : IMessage
        {
            Queue.Register<TIn>().Subscribe(m => Queue.PublishAll(handler.Handle(m)));
        }
    }
        
    public interface IMessageHandler<TIn, out TOut> where TIn : IMessage where TOut : IMessage
    {
        IEnumerable<TOut> Handle(TIn message);
    }

    public interface ICommandHandler<TCommand> : IMessageHandler<TCommand, IEvent> where TCommand : ICommand
    {   
    }

    public interface IEventHandler<TEvent> : IMessageHandler<TEvent, ICommand> where TEvent : IEvent
    {
    }
}