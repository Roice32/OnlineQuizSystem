import { QuizResultHeader } from "./quiz-results"

export enum AnswerResult
{
    Correct,
    PartiallyCorrect,
    CorrectNotPicked,
    Wrong,
    NotAnswered,
    Pending,
    Other
}

export type ReviewNeededQuestionResult = {
    reviewNeededAnswer: string;
    LLMRevier : string;
    reviewNeededResult: AnswerResult;
}

export type QuestionReview= {
    updatedQuizResultHeader: QuizResultHeader;
    updatedQuestionResults: ReviewNeededQuestionResult;
}

