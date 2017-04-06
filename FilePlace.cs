﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    class FilePlaceManager
    {
        public static FilePlaced Handle(PlaceFile cmd)
        {
            return Get(cmd.TakenAt.HasValue ? cmd.TakenAt.Value : new DateTime(0)).Handle(cmd);
        }

        public static FilesCopied Handle(CopyFiles cmd)
        {
            var moved = places.Values
                .Select(p => p.Handle(cmd))
                .SelectMany(p => p.Files)
                .ToList();

            return new FilesCopied() { Files = moved, CorrelationId = cmd.CorrelationId };
        }

        static Dictionary<DateTime, FilePlace> places = new Dictionary<DateTime, FilePlace>();

        public static FilePlace Get(DateTime date)
        {
            var key = new DateTime(date.Year, date.Month, 1);

            if (!places.ContainsKey(key))
                places.Add(key, new FilePlace(key));

            return places[key];
        }
    }

    class FilePlace :
        ICommandHandler<PlaceFile, FilePlaced>,
        ICommandHandler<CopyFiles, FilesCopied>
    {
        static string BasePath = @"e:\_test_\out";

        DateTime key;
        string folder;
        Dictionary<Guid, Tuple<Guid, FileInfo, TargetFileInfo, string>> store = new Dictionary<Guid, Tuple<Guid, FileInfo, TargetFileInfo, string>>();


        public FilePlace(DateTime _key)
        {
            key = _key;
            folder = Path.Combine(BasePath, $"{key.Year.ToString("0000")}_{key.Month.ToString("00")}");
        }

        public FilesCopied Handle(CopyFiles command)
        {
            Directory.CreateDirectory(folder);

            var moved = store.Values
                .Select(f => {
                    File.Copy(
                        Path.Combine(f.Item2.Path, f.Item2.Name),
                        Path.Combine(f.Item3.File.Path, f.Item3.File.Name)
                    );
                    return f.Item1;
                });

            return new FilesCopied() { Files = moved, CorrelationId = command.CorrelationId };
        }

        public FilePlaced Handle(PlaceFile command)
        {
            var md5 = getMD5(command.File);

            var duplicate = store.Values.FirstOrDefault(v => v.Item4 == md5);
            if (duplicate != null)
                return new FilePlaced() { CorrelationId = command.CorrelationId, Target = duplicate.Item3, IsDuplicate = true };

            var target = new TargetFileInfo() { File = new FileInfo() { Name = getNextName(command.File.Name), Path = folder }, SortDate = key };

            store.Add(command.CorrelationId, Tuple.Create(command.CorrelationId, command.File, target, md5));

            return new FilePlaced() { Target = target, CorrelationId = command.CorrelationId, IsDuplicate = false };
        }

        int names = 1;

        string getNextName(string name)
        {
            return $"{(++names).ToString("0000")}_{name}";
        }

        string getMD5(FileInfo file)
        { 
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(Path.Combine(file.Path, file.Name)))
                {       
                    return Encoding.UTF8.GetString(md5.ComputeHash(stream));
                }
            }
        }
    }    
}