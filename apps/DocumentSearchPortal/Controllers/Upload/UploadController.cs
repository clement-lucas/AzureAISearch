using DocumentSearchPortal.Data;
using DocumentSearchPortal.Models.ViewModels;
using DocumentSearchPortal.Services.Upload;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSearchPortal.Controllers.Upload
{
    /// <summary>
    /// Controller for Upload Page
    /// </summary>
    public class UploadController : Controller
    {
        private readonly IUploadService _uploadService;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for UploadController.
        /// </summary>
        /// <param name="uploadService"></param>
        /// <param name="context"></param>
        public UploadController(IUploadService uploadService, ApplicationDbContext context)
        {
            _uploadService = uploadService;
            _context = context;
        }

        /// <summary>
        /// Landing function for Upload Page.
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Index()
        {
            UploadViewModel viewModel = await _uploadService.PrepareIndexViewModelAsync();
            //viewModel.LoggedInUser = User.Identity.Name;
            return View(viewModel);
        }

        /// <summary>
        /// Function that gets called when upload button is clicked.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="viewModel"></param>
        /// <param name="newInformationName"></param>
        /// <returns>RedirectToAction</returns>
        [HttpPost]
        public async Task<IActionResult> Upload(IList<IFormFile> files, UploadViewModel viewModel, string newInformationName = null)
        {            
            //viewModel.LoggedInUser = User.Identity.Name;

            //Check the upload file count.
            if (files.Count == 0)
            {
                TempData["Message"] = "アップロードするファイルを選択してください。";
                return RedirectToAction("Index");
            }

            //Upload the documents and metadata.
            var uploadResults = await _uploadService.UploadDocumentsAsync(files, viewModel, newInformationName);

            // If file upload fails, display error message, else display success message.
            var failedUploads = uploadResults.Where(r => !r.Success).ToList();
            if (failedUploads.Any())
            {
                TempData["Error"] = $"{failedUploads.Count}のファイルのアップロードに失敗しました。 もう一度試してください。";
            }
            else
            {
                TempData["Message"] = "すべてのファイルが正常にアップロードされました。";
            }

            return RedirectToAction("Index");
        }
    }
}
