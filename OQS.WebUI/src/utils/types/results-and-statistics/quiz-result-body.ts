
import { Question } from "./question";
import { QuestionResult } from "./question-result";

export type QuizResultBody = {
  questions: Question[];
  questionResults: QuestionResult[];
};
