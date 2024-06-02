import { ReviewNeededQuestionResult } from "./review-needed-question-result";
import { QuizResultHeader } from "./quiz-result-header";

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

export type QuestionReview= {
    updatedQuizResultHeader: QuizResultHeader;
    updatedQuestionResult: ReviewNeededQuestionResult;
}

