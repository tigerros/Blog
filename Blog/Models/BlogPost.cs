namespace Blog.Models;

using System.ComponentModel.DataAnnotations;

public sealed class BlogPost {
	public int Id { get; }
	public DateTime Date { get; }
	[Required]
	public string Title { get; }
	[Required] public string Content { get; }

	public BlogPost(int id, DateTime date, string title, string content) {
		Id = id;
		Date = date;
		Title = title;
		Content = content;
	}
}