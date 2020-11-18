using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace DemoPDFXamarin.Common
{
    public class Common
    {
        public static string GetAppPath(Context context)
        {
            File dir = new File(Android.OS.Environment.ExternalStorageDirectory
                + File.Separator
                + context.GetString(Resource.String.app_name)
                + File.Separator);
            if (!dir.Exists())
            {
                dir.Mkdir();
            }

            return dir.Path + File.Separator;
        }

        public static async Task WriteFileStorageAsync(Context context, string filename)
        {
            AssetManager assets = context.Assets;
            if (new File(GetFilePath(context, filename)).Exists())
            {
                return;

            }

            try
            {
                var input = assets.Open(filename);
                var output = new FileOutputStream(GetFilePath(context, filename));
                byte[] buffer = new byte[8 * 1024];
                int lenght;
                while ((lenght = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, lenght);
                }
            }
            catch (FileNotFoundException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }


        }

        public static string GetFilePath(Context context, string fileName)
        {
            return context.FilesDir + File.Separator + fileName;
        }
    }
}