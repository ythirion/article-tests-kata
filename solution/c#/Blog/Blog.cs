using LanguageExt;
using TimeProvider = System.Func<System.DateTime>;

namespace Blog;

public class Article
{
    private readonly TimeProvider _time;
    private readonly string _name;
    private readonly string _content;
    public Seq<Comment> Comments { get; }

    private Article(string name, string content, Seq<Comment> comments, TimeProvider timeProvider)
    {
        _name = name;
        _content = content;
        Comments = comments;
        _time = timeProvider;
    }

    private Article(string name, string content, TimeProvider timeProvider)
        : this(name, content, Seq.create<Comment>(), timeProvider)
    {
    }

    public static Article Create(string name, string content, TimeProvider timeProvider) =>
        new(name, content, timeProvider);

    public Either<Error, Article> AddComment(string text, string author)
    {
        var comment = new Comment(text, author, _time());
        return Comments.Contains(comment)
            ? new Error("This comment already exists in this article")
            : new Article(_name, _content, Comments.Add(comment), _time);
    }
}

public record Error(string Message);

public record Comment(string Text, string Author, DateTime CreationDate);