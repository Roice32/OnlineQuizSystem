export type QuizResultHeader = {
    quizId: string;
    userId: string;
    submittedAtUtc: Date;
    completionTime: number;
    score: number;
    reviewPending: boolean;
    username?: string;
    quizName?: string;
};