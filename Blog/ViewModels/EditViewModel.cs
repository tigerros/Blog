namespace Blog.ViewModels; 

public sealed class EditViewModel : BaseViewModel {
	public int Id { get; set; }
	public string EditedTitle { get; set; }
	public string EditedContent { get; set; }
}