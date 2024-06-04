import { QuestionType } from "../questions";
import { AnswerResult } from "./answer-result";

export type QuestionResult = {
  userId: string;
  questionId: string;
  score: number;
  type: QuestionType;

  trueFalseAnswerResult?: AnswerResult;

  pseudoDictionaryChoicesResults?: string;

  writtenAnswer?: string;
  writtenAnswerResult?: AnswerResult;

  reviewNeededAnswer?: string;
  LLMReview?: string;
  reviewNeededResult?: AnswerResult;
};
