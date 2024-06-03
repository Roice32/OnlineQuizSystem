import { QuestionBase } from "./questions";

export type NakedQuiz = {
    id: string;
    name: string;
    timeLimitMinutes: number;
    questions: QuestionBase[];
}