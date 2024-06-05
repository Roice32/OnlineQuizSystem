import { QuizResultHeader } from "./quiz-result-header.ts";

export type TakenQuizzesHistory = {
    quizNames: { [key: string]: string };
    quizResultHeaders: QuizResultHeader[];
  };