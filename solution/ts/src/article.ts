import {Either, left, right} from 'fp-ts/Either';

type TimeProvider = () => Date;

export class Article {
    private constructor(
        private readonly name: string,
        private readonly content: string,
        public readonly comments: Comment[],
        private readonly timeProvider: TimeProvider
    ) {
    }

    public static create(
        name: string,
        content: string,
        timeProvider: TimeProvider
    ): Article {
        return new Article(name, content, [], timeProvider);
    }

    public addComment(
        text: string,
        author: string
    ): Either<Error, Article> {
        const newComment = new Comment(text, author, this.timeProvider());
        const commentExists = this.comments.some(
            (comment) => comment.text === newComment.text && comment.author === newComment.author
                && comment.creationDate.getDate() === newComment.creationDate.getDate()
        );

        return commentExists
            ? left(new Error('This comment already exists in this article'))
            : right(
                new Article(this.name, this.content, [...this.comments, newComment], this.timeProvider)
            );
    }
}

export class Error {
    constructor(public readonly message: string) {
    }
}

export class Comment {
    constructor(
        public readonly text: string,
        public readonly author: string,
        public readonly creationDate: Date
    ) {
    }
}
