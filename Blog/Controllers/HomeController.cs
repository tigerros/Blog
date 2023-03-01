using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

using ViewModels;

public sealed class HomeController : Controller {
	public ViewResult Index(int? idToDelete = null) {
		if (idToDelete != null) CMS.Instance.DeleteBlogPost(idToDelete.Value);
		
		return View(new BaseViewModel());
	}
	
	public ViewResult Create() {
		return View(new CreateViewModel());
	}

	[HttpPost]
	public ViewResult Create(CreateViewModel formModel) {
		CMS.Instance.AddBlogPost(formModel.BlogTitle, formModel.BlogContent);
	
		return View(formModel);
	}
	
	public ViewResult Edit(int id) {
		return View(new EditViewModel { Id = id, });
	}

	[HttpPost]
	public ViewResult Edit(EditViewModel formModel) {
		CMS.Instance.EditBlogPost(formModel.Id, formModel.EditedTitle, formModel.EditedContent);
	
		return View(formModel);
	}

	public ViewResult Detail(int id, string? newTitle = null, string? newContent = null) {
		if (newTitle != null) CMS.Instance.EditBlogPost(id, newTitle, newContent);
		
		return View(new DetailViewModel { Id = id, });
	}
}