import { QuizResultHeader } from "./quiz-result-header";
export type TakenQuizStats = {
    quizNames: { [key: string]: string };
    quizResultHeaders: QuizResultHeader[];
  };