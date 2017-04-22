using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace SortMyFiles
{
    public class FileProcessor : 
        ICommandHandler<FilterFile>,
        ICommandHandler<AnalyzeFile>
    {
        public IEnumerable<IEvent> Handle(AnalyzeFile cmd)
        {
            yield return new FileDateDetermined() { FileDate = getDate(cmd.File), CorrelationId = cmd.CorrelationId };
        }

        public IEnumerable<IEvent> Handle(FilterFile cmd)
        {
            yield return new FileFiltered() { CorrelationId = cmd.CorrelationId, KeepFile = isSortableFile(cmd.File) };            
        }

        bool isSortableFile(FileInfo file)
        {
            return (Path.GetExtension(Path.Combine(file.Path, file.Name)).ToUpper() == ".jpg".ToUpper());
        }

        DateTime? getDate(FileInfo file)
        {            
            try
            {
                using (FileStream fs = new FileStream(Path.Combine(file.Path, file.Name), FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    return TryGetTakenDate(myImage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Path.Combine(file.Path, file.Name)}: {e.ToString()}");
                return null;
            }
        }

        static Regex r = new Regex(":");

        static DateTime? TryGetTakenDate(Image src)
        {
            try
            {
                return DateTime.Parse(
                    r.Replace(
                        Encoding.UTF8.GetString(
                            (TryGetPropertyItem(src, 36867) ?? TryGetPropertyItem(src, 36868)).Value), "-", 2)
                    );
            }
            catch
            {
                return null;
            }
        }

        static PropertyItem TryGetPropertyItem(Image src, int propertyId)
        {
            try
            {
                return src.GetPropertyItem(propertyId);
            }
            catch
            {
                return null;
            }
        }
    }
}
