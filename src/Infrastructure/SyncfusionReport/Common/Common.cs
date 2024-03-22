using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD.WebApi.Infrastructure.SyncfusionReport.Common;
public class Common
{
    public static MemoryStream GetFileStreamFromUrl(string fileUrl)
    {
        try
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(fileUrl);

            return new MemoryStream(imageData);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
