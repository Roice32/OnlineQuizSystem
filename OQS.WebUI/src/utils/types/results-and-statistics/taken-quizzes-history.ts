import { QuizResultHeader } from "./quiz-result-header.ts";

export type TakenQuizStats = {
    quizNames: { [key: string]: string };
    quizResultHeaders: QuizResultHeader[];
  };