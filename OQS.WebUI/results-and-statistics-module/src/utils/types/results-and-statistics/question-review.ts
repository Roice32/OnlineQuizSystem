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
    llmReview : string;
    reviewNeededResult: AnswerResult;
    questionId : string;
    score : number;
    userId : string;
}

export type QuestionReview= {
    updatedQuizResultHeader: QuizResultHeader;
    updatedQuestionResult: ReviewNeededQuestionResult;
}

