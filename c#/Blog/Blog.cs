using LanguageExt;

namespace Blog;

public class Article
{
    private readonly string _name;
    private readonly string _content;
    public Seq<Comment> Comments { get; }

    private Article(string name, string content, Seq<Comment> comments)
    {
        _name = name;
        _content = content;
        Comments = comments;
    }

    private Article(string name, string content)
        : this(name, content, Seq.create<Comment>())
    {
    }

    public static Article Create(string name, string content) => new(name, content);

    public Either<Error, Article> AddComment(string text, string author)
    {
        var comment = new Comment(text, author, DateTime.Now);
        return Comments.Contains(comment)
            ? new Error("This comment already exists in this article")
            : new Article(_name, _content, Comments.Add(comment));
    }
}

public record Error(string Message);

public record Comment(string Text, string Author, DateTime CreationDate);