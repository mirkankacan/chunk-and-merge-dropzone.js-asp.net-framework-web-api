using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ChunkingTest.Controllers
{
    [RoutePrefix("fileupload")]
    public class FileUploadController : ApiController
    {
        [HttpPost, Route("uploadchunk")]
        public async Task<IHttpActionResult> UploadChunk()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var dzChunkIndex = int.Parse(request.Form["dzChunkIndex"]);
                var dzTotalChunkCount = int.Parse(request.Form["dzTotalChunkCount"]);
                var dzUuid = request.Form["dzUuid"];
                var dzFilename = request.Form["dzFilename"];
                var totalFileSize = long.Parse(request.Form["dzTotalFileSize"]);
                var fileExtension = Path.GetExtension(dzFilename);
                var clientCode = request.Form["clientCode"];
                var merchantCode = request.Form["merchantCode"];
                var fileDescription = request.Form["fileDescription"];
                var categoryID = int.Parse(request.Form["categoryID"]);
                var chunk = request.Files[0];

                var baseUploadPath = @"C:\Uploads";
                var tempPath = Path.Combine(baseUploadPath, dzUuid); 

                if (!Directory.Exists(baseUploadPath))
                    Directory.CreateDirectory(baseUploadPath);

                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                var chunkFileName = Path.Combine(tempPath, dzChunkIndex.ToString());
                chunk.SaveAs(chunkFileName);

                if (Directory.GetFiles(tempPath).Length == dzTotalChunkCount)
                {
                    var finalPath = Path.Combine(baseUploadPath, dzFilename);
                    using (var fs = new FileStream(finalPath, FileMode.Create))
                    {
                        for (int i = 0; i < dzTotalChunkCount; i++)
                        {
                            var tempChunkFileName = Path.Combine(tempPath, i.ToString());
                            if (File.Exists(tempChunkFileName))
                            {
                                var chunkData = File.ReadAllBytes(tempChunkFileName);
                                fs.Write(chunkData, 0, chunkData.Length);
                                File.Delete(tempChunkFileName); 
                            }
                        }
                    }
                    Directory.Delete(tempPath);
                  
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

    }
}
