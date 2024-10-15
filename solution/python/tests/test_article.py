from assertpy import assert_that
from hypothesis import given, strategies as st

from article_builder import ArticleBuilder, CREATION_DATE, COMMENT_TEXT, AUTHOR
from blog.article import Error


def time_provider():
    return CREATION_DATE


def test_should_add_comment_to_article():
    result = (ArticleBuilder
              .an_article()
              .build()
              .add_comment(COMMENT_TEXT, AUTHOR))

    assert_that(len(result.comments)).is_equal_to(1)
    assert_that(result.comments[0].text).is_equal_to(COMMENT_TEXT)
    assert_that(result.comments[0].author).is_equal_to(AUTHOR)
    assert_that(result.comments[0].creation_date).is_equal_to(CREATION_DATE)


def test_should_add_comment_to_article_with_existing_comments():
    new_comment_text = "Another comment"
    new_author = "Author2"

    result = (ArticleBuilder.an_article()
              .commented()
              .build()
              .add_comment(new_comment_text, new_author))

    assert_that(len(result.comments)).is_equal_to(2)
    assert_that(result.comments[1].text).is_equal_to(new_comment_text)
    assert_that(result.comments[1].author).is_equal_to(new_author)
    assert_that(result.comments[1].creation_date).is_equal_to(CREATION_DATE)


def test_should_not_add_existing_comment():
    result = (ArticleBuilder
              .an_article()
              .commented()
              .build()
              .add_comment(COMMENT_TEXT, AUTHOR))

    assert_that(result).is_instance_of(Error)
    assert_that(result.message).is_equal_to("This comment already exists in this article")


@given(text=st.text(min_size=1), author=st.text(min_size=1))
def test_property_based_add_comment_should_fail_on_duplicate(text, author):
    result = (ArticleBuilder.an_article().build()
              .add_comment(text, author)
              .add_comment(text, author))
    assert_that(result).is_instance_of(Error)
    assert_that(result.message).is_equal_to("This comment already exists in this article")
