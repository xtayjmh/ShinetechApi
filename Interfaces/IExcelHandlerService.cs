using System.Collections.Generic;
using System.IO;

namespace API.Interfaces
{
    public interface IExcelHandlerService : IService
    {
        Stream ExportToStream<PageView>(List<PageView> dataList);
        IEnumerable<T> LoadFromExcel<T>(string FileName) where T : new();
    }
}