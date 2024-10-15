from datetime import datetime
from typing import List, Union


class Error(Exception):
    def __init__(self, message: str):
        self.message = message

    def __repr__(self):
        return f"Error({self.message})"

    def __eq__(self, other):
        if isinstance(other, Error):
            return self.message == other.message
        return False


class Comment:
    def __init__(self, text: str, author: str, creation_date: datetime):
        self.text = text
        self.author = author
        self.creation_date = creation_date

    def __eq__(self, other):
        if isinstance(other, Comment):
            return (self.text == other.text and
                    self.author == other.author and
                    self.creation_date == other.creation_date)
        return False


class Article:
    def __init__(self, name: str, content: str, comments: List[Comment]):
        self._name = name
        self._content = content
        self.comments = comments

    @staticmethod
    def create(name: str, content: str) -> 'Article':
        return Article(name, content, [])

    def add_comment(self, text: str, author: str) -> Union['Article', Error]:
        new_comment = Comment(text, author, datetime.now())

        if new_comment in self.comments:
            return Error("This comment already exists in this article")
        return Article(self._name, self._content, self.comments + [new_comment])