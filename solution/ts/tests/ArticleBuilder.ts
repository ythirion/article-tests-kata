import {faker} from "@faker-js/faker";
import * as E from "fp-ts/Either";
import {Article} from "../src/article";

export const Author = "Pablo Escobar";
export const CommentText = "Amazing article !!!";
export const CreationDate = new Date(2024, 10, 5, 13, 0, 1);

export class ArticleBuilder {
    private comments: Record<string, string> = {};

    static anArticle = (): ArticleBuilder => new ArticleBuilder();

    private static timeProvider: () => Date = () => CreationDate;

    commented(): ArticleBuilder {
        this.comments[CommentText] = Author;
        return this;
    }

    build(): Article {
        let article = Article.create(faker.lorem.sentence(), faker.lorem.paragraph(), ArticleBuilder.timeProvider);
        Object.entries(this.comments)
            .forEach(([text, author]) => {
                let result = article.addComment(text, author);
                if (E.isRight(result)) article = result.right;
            });
        return article;
    }
}