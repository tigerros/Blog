namespace Blog.ViewModels;

using System.ComponentModel.DataAnnotations;

public sealed class CreateViewModel : BaseViewModel {
	[Required]
	public string BlogTitle { get; set; }
	[Required]
	public string BlogContent { get; set; }
}