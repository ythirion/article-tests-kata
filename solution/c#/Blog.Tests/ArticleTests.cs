using FluentAssertions;
using FluentAssertions.LanguageExt;
using FsCheck;
using FsCheck.Xunit;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Xunit;
using static Blog.Tests.ArticleBuilder;

namespace Blog.Tests;

public class ArticleTests
{
    private Either<Error, Article> _result;
    private readonly Bogus.Randomizer _random = new();

    /*
     Test generated using Github Copilot...
     What's the problem here?
       [Fact]
       public void Create_ShouldInitializeArticle()
       {
           // Arrange
           var name = "Test Article";
           var content = "This is a test article.";

           // Act
           var article = Article.Create(name, content);

           // Assert
           Assert.NotNull(article);
           Assert.Equal(name, article.GetType().GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(article));
           Assert.Equal(content, article.GetType().GetField("_content", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(article));
           Assert.Empty(article.Comments);
       }
     */

    [Fact]
    public void Should_Add_Comment_In_An_Article()
    {
        Given(AnArticle());
        When(article => article.AddComment(CommentText, Author));
        Then(result
            => result
                .Should()
                .BeRight(article =>
                    {
                        article.Comments.Should().HaveCount(1);
                        AssertComment(article.Comments[0], CommentText, Author);
                    }
                ));
    }

    [Fact]
    public void Should_Add_Comment_In_An_Article_Containing_Already_A_Comment()
    {
        var newComment = _random.String(10);
        var newAuthor = _random.String(3);

        Given(AnArticle().Commented());
        When(article => article.AddComment(newComment, newAuthor));
        Then(result
            => result.Should()
                .BeRight(article =>
                {
                    article.Comments.Should().HaveCount(2);
                    AssertComment(article.Comments[1], newComment, newAuthor);
                })
        );
    }

    private static void AssertComment(Comment comment, string expectedComment, string expectedAuthor)
    {
        comment.Text.Should().Be(expectedComment);
        comment.Author.Should().Be(expectedAuthor);
        comment.CreationDate.Should().Be(CreationDate);
    }

    public class Fail : ArticleTests
    {
        [Fact]
        public void When_Adding_An_Existing_Comment()
        {
            Given(AnArticle().Commented());
            When(article => article.AddComment(CommentText, Author));
            Then(result
                => result.Should()
                    .BeLeft(error =>
                        error.Should()
                            .Be(new Error("This comment already exists in this article"))
                    ));
        }

        private static readonly Arbitrary<string> NonEmptyString = Arb.Default
            .String()
            .MapFilter(s => s, s => !string.IsNullOrWhiteSpace(s));

        [Property]
        // Never trust a test you have not seen failed
        // Should return the Property here
        public void When_Adding_An_Existing_Comment_Then_It_Should_Fail()
            => Prop.ForAll(NonEmptyString, NonEmptyString,
                (comment, author) =>
                    AnArticle().Build()
                        .AddComment(comment, author)
                        .Bind(a => a.AddComment(comment, author))
                        .IsLeft
                    );
    }

    private void Given(ArticleBuilder articleBuilder) => _result = articleBuilder.Build();
    private void When(Func<Article, Either<Error, Article>> act) => _result = act(_result.ValueUnsafe());
    private void Then(Action<Either<Error, Article>> act) => act(_result);
}