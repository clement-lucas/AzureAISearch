using DocumentSearchPortal.Models;
using DocumentSearchPortal.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DocumentSearchPortal.Services.Upload
{
    /// <summary>
    /// IUploadService
    /// </summary>
    public interface IUploadService
    {
        Task<UploadViewModel> PrepareIndexViewModelAsync();
        Task<IEnumerable<UploadResult>> UploadDocumentsAsync(IList<IFormFile> files, UploadViewModel uploadViewModel, string newInformationName = null);
    }
}

