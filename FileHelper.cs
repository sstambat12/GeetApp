using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GeetApp
{
    class FileHelper
    {
        public async static Task WriteTextFileAsync(string fileName, string content)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var textFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite);
            var textWriter = new DataWriter(textStream);
            textWriter.WriteString("\n" + content);
            await textWriter.StoreAsync();
            await textStream.FlushAsync();
            textStream.Dispose();
        }

        public async static Task<string> ReadTextFileAsync(string fileName)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile;
            try
            {
                 textFile = await storageFolder.GetFileAsync(fileName);
            }
            catch(Exception e)
            {
                return "";
            }
            var textStream = await textFile.OpenReadAsync();
            var textReader = new DataReader(textStream);
            var textLength = textStream.Size;
            await textReader.LoadAsync((uint)textLength);
            return textReader.ReadString((uint)textLength);
        }

        public async static void DeleteFile(string fileName)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var textFile = await storageFolder.GetFileAsync(fileName);
            await textFile.DeleteAsync();
        }
    }
}
