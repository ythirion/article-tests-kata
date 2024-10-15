import {ArticleBuilder, Author, CommentText, CreationDate} from './ArticleBuilder';
import * as E from 'fp-ts/Either';
import {chain, left, right} from 'fp-ts/Either';
import * as fc from 'fast-check';
import {pipe} from "fp-ts/function";
import {Error} from "../src/article"

describe('Article', () => {
    test('should add a comment to an article', () => {
        const result = ArticleBuilder
            .anArticle()
            .build()
            .addComment(CommentText, Author);

        expect(result).toEqual(
            right(
                expect.objectContaining({
                    comments: [
                        expect.objectContaining({
                            text: CommentText,
                            author: Author,
                            creationDate: CreationDate,
                        }),
                    ],
                })
            )
        );
    });

    test('should add another comment to an article already containing a comment', () => {
        const article = ArticleBuilder
            .anArticle()
            .commented()
            .build();

        const newCommentText = 'New Comment';
        const newAuthor = 'Author2';

        const result = article.addComment(newCommentText, newAuthor);

        expect(result).toEqual(
            right(
                expect.objectContaining({
                    comments: expect.arrayContaining([
                        expect.objectContaining({
                            text: CommentText,
                            author: Author,
                            creationDate: CreationDate,
                        }),
                        expect.objectContaining({
                            text: newCommentText,
                            author: newAuthor,
                            creationDate: CreationDate,
                        }),
                    ]),
                })
            )
        );
    });

    test('should not add an existing comment', () => {
        const article = ArticleBuilder.anArticle()
            .commented()
            .build();

        const result = article.addComment(CommentText, Author);

        expect(result).toEqual(left(new Error('This comment already exists in this article')));
    });

    test('property-based: should fail when adding the same comment twice', () => {
        fc.assert(
            fc.property(fc.string(), fc.string(), (text, author) =>
                E.isLeft(pipe(
                    ArticleBuilder
                        .anArticle()
                        .build()
                        .addComment(text, author),
                    chain((article) => article.addComment(text, author))
                )))
        );
    });
});
