using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Libber_Manager.Controls;

namespace Libber_Manager.Helpers
{
    public class ImageHandler
    {
        public static ImageHandler _imageHandler;

        public ImageHandler()
        {
            _imageHandler = this;
        }

        public static byte[] imgOutput;

        public static BitmapImage ConvertedByteToBitmap;

        public static async Task ByteToBitmap(byte[] img)
        {
            ConvertedByteToBitmap = new BitmapImage();

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(img);
                    await writer.StoreAsync(); 
                }
                
                if (img != null)
                {
                    await ConvertedByteToBitmap.SetSourceAsync(stream);
                }                
            }
        }

        public static async Task ImageFileToByte(StorageFile imgFile)
        {
            byte[] buffer;

            using (var inputStream = await imgFile.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                buffer = new byte[readStream.Length];
                await readStream.ReadAsync(buffer, 0, buffer.Length);
            }

            if (imgFile != null)
            {
                //Ads.adImage_byte = buffer;
                imgOutput = buffer;
            }            
        }

        public static async Task ChangeImage()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            StorageFile imgfile = await picker.PickSingleFileAsync();
            
            if (imgfile != null)
            {
                await ImageFileToByte(imgfile);
                await ByteToBitmap(imgOutput);
            }
            else
            {
                ConvertedByteToBitmap = null;
            }
        }

    }
}
