using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DocumentSearchPortal.Models.ViewModels
{
    /// <summary>
    /// UploadViewModel
    /// </summary>
    public class UploadViewModel
    {
        public SelectList? Categories { get; set; }
        public SelectList? Protocols { get; set; }
        public List<SelectListItem>? DisclosureScopes { get; set; }
        public SelectList? InformationIds { get; set; }

        [Required(ErrorMessage = "説明の入力は必須です。")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "カテゴリーの選択は必須です。")]
        public int SelectedCategoryId { get; set; }

        [Required(ErrorMessage = "プロトコルの選択は必須です。")]
        public int SelectedProtocolId { get; set; }

        [Required(ErrorMessage = "公開範囲の選択は必須です。")]
        public int SelectedDisclosureScopeId { get; set; }

        [Required(ErrorMessage = "情報名の選択は必須です。")]
        public int SelectedInformationId { get; set; }

        public string? SelectedCategoryName { get; set; }
        public string? SelectedProtocolName { get; set; }
        public string? SelectedDisclosureScopeName { get; set; }
        public string? SelectedInformationName { get; set; }
    }
}
