import { QuizResultHeader } from "./quiz-result-header";

export type CreatedQuizStats = {
    quizName: string;
    userNames: Map<string, string>;
    quizResultHeaders: QuizResultHeader[]
}