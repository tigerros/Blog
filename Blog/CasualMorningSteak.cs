namespace Blog;

using System.Globalization;
using System.Text;
using Models;

public sealed class CMS {
	private static int _highestId;
	private readonly FileInfo _csvFile;

	public static CMS Instance { get; set; }

	public CMS(string csvFilePath) {
		_csvFile = new FileInfo(csvFilePath);

		bool first = true;
		
		foreach (string line in File.ReadLines(csvFilePath)) {
			if (first) {
				first = false;
				continue;
			}
			
			// Only need ID, faster than splitting the entire string
			ulong idul = 0;
			ulong multiplier = 1000000000;
			
			foreach (char c in line) {
				if (c == '`') break;

				ulong digit = (ulong)(c - 48);
				
				idul += digit * multiplier;
				multiplier /= 10;
			}

			idul /= multiplier * 10;

			int id = (int)idul;

			if (id > _highestId) _highestId = id;
		}
	}

	public BlogPost? GetBlogPost(int id) {
		bool first = true;
		
		foreach (string line in File.ReadLines(_csvFile.FullName)) {
			if (first) {
				first = false;
				continue;
			}

			string[] values = line.Split('`');
			int itId = int.Parse(values[0]);

			if (itId == id) {
				DateTime date = DateTime.Parse(values[1]);

				return new BlogPost(id, date, values[2], values[3]);
			}
		}

		return null;
	}

	public IEnumerable<BlogPost> GetBlogPostsEnumerable() {
		bool first = true;

		foreach (string line in File.ReadLines(_csvFile.FullName)) {
			if (first) {
				first = false;
				continue;
			}

			string[] values = line.Split('`'); // The backtick is a very uncommon character
			int id = int.Parse(values[0]);
			DateTime date = DateTime.Parse(values[1]);

			yield return new BlogPost(id, date, values[2], values[3]);
		}
	}

	private static string CleanContent(string content) {
		string clean = content.Replace(@"
", "<br/>");

		return clean;
	}

	public string RecoverContent(string databaseContent) => databaseContent.Replace(@"<br/>", "\n");

	public void AddBlogPost(string title, string content) {
		using StreamWriter writer = _csvFile.AppendText();

		content = CleanContent(content);
		
		writer.Write($"\n{++_highestId}`{DateTime.Now.ToString(CultureInfo.CurrentCulture)}`{title}`{content}");
	}

	public void DeleteBlogPost(int id) {
		string[] lines = File.ReadAllLines(_csvFile.FullName);

		if (lines.Length <= 1) return;
		
		using StreamWriter writer = _csvFile.CreateText();
		// x << 5 = x * 32 and 32 is basically the minimum amount of characters a line is going to have. Probably more like 64
		var result = new StringBuilder("id`date`title`content") {
			Capacity = lines.Length << 5,
		};
		
		bool found = false;

		for (int i = 1; i < lines.Length; i++) {
			string[] values = lines[i].Split('`');
			
			if (id == int.Parse(values[0])) {
				found = true;
				continue;
			}

			result.Append('\n').Append(lines[i]);
		}

		if (found && (id == _highestId)) _highestId--;

		writer.Write(result);
	}

	public void EditBlogPost(int id, string editedTitle, string editedContent) {
		string[] lines = File.ReadAllLines(_csvFile.FullName);

		if (lines.Length <= 1) return;

		using StreamWriter writer = _csvFile.CreateText();
		var result = new StringBuilder("id`date`title`content") {
			Capacity = lines.Length << 5,
		};

		for (int i = 1; i < lines.Length; i++) {
			string[] values = lines[i].Split('`');
			string toAppend = lines[i];

			if (id == int.Parse(values[0]))
				toAppend = $"{id}`{DateTime.Now.ToString(CultureInfo.CurrentCulture)}`{editedTitle}`{CleanContent(editedContent)}";

			result.Append('\n').Append(toAppend);
		}

		writer.Write(result);
	}
}