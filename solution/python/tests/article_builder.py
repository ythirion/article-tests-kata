from datetime import datetime

from blog.article import Article, Comment

AUTHOR = "Pablo Escobar"
COMMENT_TEXT = "Amazing article !!!"
CREATION_DATE = datetime(2024, 11, 5, 13, 0, 1)


def time_provider() -> datetime:
    return CREATION_DATE


class ArticleBuilder:
    def __init__(self):
        self._name = "Default Article"
        self._content = "Default content"
        self._comments = []
        self._time_provider = time_provider

    @staticmethod
    def an_article() -> 'ArticleBuilder':
        return ArticleBuilder()

    def commented(self) -> 'ArticleBuilder':
        self._comments.append(Comment(COMMENT_TEXT, AUTHOR, self._time_provider()))
        return self

    def build(self) -> Article:
        article = Article.create(self._name, self._content, self._time_provider)
        for comment in self._comments:
            result = article.add_comment(comment.text, comment.author)
            if isinstance(result, Article):
                article = result
        return article
