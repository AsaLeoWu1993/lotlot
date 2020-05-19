using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Operation.Common
{
    public class BlobHelper
    {
        static CloudBlobClient blobClient = null;
        static BlobHelper()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
        "dsdd233b3ef64511e984b7e",
        "I/3iRxXVu2iGNe4bI/dQEh+PtzZaiy2pjUsw3FkWoZBt9Gk/zTWG6CaFeZR+ECfxu6sMduVKucJQKvpqoYddgQ=="), true);
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="mycontainer">容器名</param>
        /// <param name="stream">文件流</param>
        /// <returns></returns>
        public static async Task<string> UploadToBlob(string fileName, string mycontainer, Stream stream)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(mycontainer.ToLower());
            //创建一个容器（如果该容器不存在）
            await container.CreateIfNotExistsAsync();
            //设置该容器为公共容器，也就是说网络上能访问容器中的文件，但不能修改、删除
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            //将Blob（文件）上载到容器中，如果已存在同名Blob，则覆盖它
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);//获取块 Blob 引用
            var bytes = StreamToBytes(stream);
            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            return blockBlob.Uri.ToString();
        }

        /// <summary>
        /// 转换格式
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 上传图像文件   为空则格式不正确
        /// </summary>
        /// <param name="fileinput"></param>
        /// <param name="mycontainer"></param>
        /// <returns></returns>
        public static async Task<string> UploadImageToBlob(IFormFile fileinput, string mycontainer)
        {
            if (fileinput == null) return null;
            var fileName = fileinput.FileName;
            var format = fileName.Split(".").Last().ToLower();
            if (format != "png" && format != "jpg" && format != "jpeg" && format != "gif")
                return null;
            if (fileinput.OpenReadStream().Length > 20971520)
                return "1";
            var updateName = Guid.NewGuid().ToString().Replace("-", "") + "." + format;
            var url = await UploadToBlob(updateName, mycontainer, fileinput.OpenReadStream());
            return url;
        }
    }
}
