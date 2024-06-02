import { AnswerResult } from "./question-review";


export type ReviewNeededQuestionResult = {
    reviewNeededAnswer: string;
    LLMReview: string;
    reviewNeededResult: AnswerResult;
};
